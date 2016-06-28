using Discord.API;
using Discord.API.Voice;
using Discord.Net.Converters;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public class DiscordVoiceAPIClient
    {
        public const int MaxBitrate = 128;
        private const string Mode = "xsalsa20_poly1305";

        public event Func<string, string, double, Task> SentRequest { add { _sentRequestEvent.Add(value); } remove { _sentRequestEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new AsyncEvent<Func<string, string, double, Task>>();
        public event Func<int, Task> SentGatewayMessage { add { _sentGatewayMessageEvent.Add(value); } remove { _sentGatewayMessageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<int, Task>> _sentGatewayMessageEvent = new AsyncEvent<Func<int, Task>>();

        public event Func<VoiceOpCode, object, Task> ReceivedEvent { add { _receivedEvent.Add(value); } remove { _receivedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<VoiceOpCode, object, Task>> _receivedEvent = new AsyncEvent<Func<VoiceOpCode, object, Task>>();
        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();

        private readonly ulong _userId;
        private readonly string _token;
        private readonly JsonSerializer _serializer;
        private readonly IWebSocketClient _gatewayClient;
        private readonly SemaphoreSlim _connectionLock;
        private CancellationTokenSource _connectCancelToken;

        public ulong GuildId { get; }        
        public string SessionId { get; }
        public ConnectionState ConnectionState { get; private set; }

        internal DiscordVoiceAPIClient(ulong guildId, ulong userId, string sessionId, string token, WebSocketProvider webSocketProvider, JsonSerializer serializer = null)
        {
            GuildId = guildId;
            _userId = userId;
            SessionId = sessionId;
            _token = token;
            _connectionLock = new SemaphoreSlim(1, 1);

            _gatewayClient = webSocketProvider();
            //_gatewayClient.SetHeader("user-agent", DiscordConfig.UserAgent); (Causes issues in .Net 4.6+)
            _gatewayClient.BinaryMessage += async (data, index, count) =>
            {
                using (var compressed = new MemoryStream(data, index + 2, count - 2))
                using (var decompressed = new MemoryStream())
                {
                    using (var zlib = new DeflateStream(compressed, CompressionMode.Decompress))
                        zlib.CopyTo(decompressed);
                    decompressed.Position = 0;
                    using (var reader = new StreamReader(decompressed))
                    {
                        var msg = JsonConvert.DeserializeObject<WebSocketMessage>(reader.ReadToEnd());
                        await _receivedEvent.InvokeAsync((VoiceOpCode)msg.Operation, msg.Payload).ConfigureAwait(false);
                    }
                }
            };
            _gatewayClient.TextMessage += async text =>
            {
                var msg = JsonConvert.DeserializeObject<WebSocketMessage>(text);
                await _receivedEvent.InvokeAsync((VoiceOpCode)msg.Operation, msg.Payload).ConfigureAwait(false);
            };
            _gatewayClient.Closed += async ex =>
            {
                await DisconnectAsync().ConfigureAwait(false);
                await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
            };

            _serializer = serializer ?? new JsonSerializer { ContractResolver = new DiscordContractResolver() };
        }

        public Task SendAsync(VoiceOpCode opCode, object payload, RequestOptions options = null)
        {
            byte[] bytes = null;
            payload = new WebSocketMessage { Operation = (int)opCode, Payload = payload };
            if (payload != null)
                bytes = Encoding.UTF8.GetBytes(SerializeJson(payload));
            //TODO: Send
            return Task.CompletedTask;
        }

        //WebSocket
        public async Task SendHeartbeatAsync(RequestOptions options = null)
        {
            await SendAsync(VoiceOpCode.Heartbeat, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), options: options).ConfigureAwait(false);
        }
        public async Task SendIdentityAsync(ulong guildId, ulong userId, string sessionId, string token)
        {
            await SendAsync(VoiceOpCode.Identify, new IdentifyParams
            {
                GuildId = guildId,
                UserId = userId,
                SessionId = sessionId,
                Token = token
            });
        }

        public async Task ConnectAsync(string url)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync(url).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync(string url)
        {
            ConnectionState = ConnectionState.Connecting;
            try
            {
                _connectCancelToken = new CancellationTokenSource();
                _gatewayClient.SetCancelToken(_connectCancelToken.Token);                
                await _gatewayClient.ConnectAsync(url).ConfigureAwait(false);

                await SendIdentityAsync(GuildId, _userId, SessionId, _token).ConfigureAwait(false);

                ConnectionState = ConnectionState.Connected;
            }
            catch (Exception)
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync()
        {
            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            
            try { _connectCancelToken?.Cancel(false); }
            catch { }

            await _gatewayClient.DisconnectAsync().ConfigureAwait(false);

            ConnectionState = ConnectionState.Disconnected;
        }

        //Helpers
        private static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
        private string SerializeJson(object value)
        {
            var sb = new StringBuilder(256);
            using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (JsonWriter writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, value);
            return sb.ToString();
        }
        private T DeserializeJson<T>(Stream jsonStream)
        {
            using (TextReader text = new StreamReader(jsonStream))
            using (JsonReader reader = new JsonTextReader(text))
                return _serializer.Deserialize<T>(reader);
        }
    }
}
