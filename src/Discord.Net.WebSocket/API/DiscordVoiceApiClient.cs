#pragma warning disable CS1591
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
using System.Net.Sockets;
using System.Net;

namespace Discord.Audio
{
    public class DiscordVoiceAPIClient
    {
        public const int MaxBitrate = 128;
        public const string Mode = "xsalsa20_poly1305";

        public event Func<string, string, double, Task> SentRequest { add { _sentRequestEvent.Add(value); } remove { _sentRequestEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new AsyncEvent<Func<string, string, double, Task>>();
        public event Func<VoiceOpCode, Task> SentGatewayMessage { add { _sentGatewayMessageEvent.Add(value); } remove { _sentGatewayMessageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<VoiceOpCode, Task>> _sentGatewayMessageEvent = new AsyncEvent<Func<VoiceOpCode, Task>>();
        public event Func<Task> SentDiscovery { add { _sentDiscoveryEvent.Add(value); } remove { _sentDiscoveryEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _sentDiscoveryEvent = new AsyncEvent<Func<Task>>();
        public event Func<int, Task> SentData { add { _sentDataEvent.Add(value); } remove { _sentDataEvent.Remove(value); } }
        private readonly AsyncEvent<Func<int, Task>> _sentDataEvent = new AsyncEvent<Func<int, Task>>();

        public event Func<VoiceOpCode, object, Task> ReceivedEvent { add { _receivedEvent.Add(value); } remove { _receivedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<VoiceOpCode, object, Task>> _receivedEvent = new AsyncEvent<Func<VoiceOpCode, object, Task>>();
        public event Func<byte[], Task> ReceivedPacket { add { _receivedPacketEvent.Add(value); } remove { _receivedPacketEvent.Remove(value); } }
        private readonly AsyncEvent<Func<byte[], Task>> _receivedPacketEvent = new AsyncEvent<Func<byte[], Task>>();
        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        
        private readonly JsonSerializer _serializer;
        private readonly IWebSocketClient _webSocketClient;
        private readonly SemaphoreSlim _connectionLock;
        private CancellationTokenSource _connectCancelToken;
        private UdpClient _udp;
        private IPEndPoint _udpEndpoint;
        private Task _udpRecieveTask;
        private bool _isDisposed;

        public ulong GuildId { get; }
        public ConnectionState ConnectionState { get; private set; }

        internal DiscordVoiceAPIClient(ulong guildId, WebSocketProvider webSocketProvider, JsonSerializer serializer = null)
        {
            GuildId = guildId;
            _connectionLock = new SemaphoreSlim(1, 1);
            _udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            _webSocketClient = webSocketProvider();
            //_gatewayClient.SetHeader("user-agent", DiscordConfig.UserAgent); (Causes issues in .Net 4.6+)
            _webSocketClient.BinaryMessage += async (data, index, count) =>
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
            _webSocketClient.TextMessage += async text =>
            {
                var msg = JsonConvert.DeserializeObject<WebSocketMessage>(text);
                await _receivedEvent.InvokeAsync((VoiceOpCode)msg.Operation, msg.Payload).ConfigureAwait(false);
            };
            _webSocketClient.Closed += async ex =>
            {
                await DisconnectAsync().ConfigureAwait(false);
                await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
            };

            _serializer = serializer ?? new JsonSerializer { ContractResolver = new DiscordContractResolver() };
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _connectCancelToken?.Dispose();
                    (_webSocketClient as IDisposable)?.Dispose();
                }
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);

        public async Task SendAsync(VoiceOpCode opCode, object payload, RequestOptions options = null)
        {
            byte[] bytes = null;
            payload = new WebSocketMessage { Operation = (int)opCode, Payload = payload };
            if (payload != null)
                bytes = Encoding.UTF8.GetBytes(SerializeJson(payload));
            await _webSocketClient.SendAsync(bytes, 0, bytes.Length, true).ConfigureAwait(false);
            await _sentGatewayMessageEvent.InvokeAsync(opCode);
        }
        public async Task SendAsync(byte[] data, int bytes)
        {
            if (_udpEndpoint != null)
            {
                await _udp.SendAsync(data, bytes, _udpEndpoint).ConfigureAwait(false);                
                await _sentDataEvent.InvokeAsync(bytes).ConfigureAwait(false);
            }

        }

        //WebSocket
        public async Task SendHeartbeatAsync(RequestOptions options = null)
        {
            await SendAsync(VoiceOpCode.Heartbeat, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), options: options).ConfigureAwait(false);
        }
        public async Task SendIdentityAsync(ulong userId, string sessionId, string token)
        {
            await SendAsync(VoiceOpCode.Identify, new IdentifyParams
            {
                GuildId = GuildId,
                UserId = userId,
                SessionId = sessionId,
                Token = token
            });
        }
        public async Task SendSelectProtocol(string externalIp, int externalPort)
        {
            await SendAsync(VoiceOpCode.SelectProtocol, new SelectProtocolParams
            {
                Protocol = "udp",
                Data = new UdpProtocolInfo
                {
                    Address = externalIp,
                    Port = externalPort,
                    Mode = Mode
                }
            });
        }
        public async Task SendSetSpeaking(bool value)
        {
            await SendAsync(VoiceOpCode.Speaking, new SpeakingParams
            {
                IsSpeaking = value,
                Delay = 0
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
                _webSocketClient.SetCancelToken(_connectCancelToken.Token);
                await _webSocketClient.ConnectAsync(url).ConfigureAwait(false);
                _udpRecieveTask = ReceiveAsync(_connectCancelToken.Token);

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

            //Wait for tasks to complete
            await _udpRecieveTask.ConfigureAwait(false);

            await _webSocketClient.DisconnectAsync().ConfigureAwait(false);

            ConnectionState = ConnectionState.Disconnected;
        }

        //Udp
        public async Task SendDiscoveryAsync(uint ssrc)
        {
            var packet = new byte[70];
            packet[0] = (byte)(ssrc >> 24);
            packet[1] = (byte)(ssrc >> 16);
            packet[2] = (byte)(ssrc >> 8);
            packet[3] = (byte)(ssrc >> 0);
            await SendAsync(packet, 70).ConfigureAwait(false);
            await _sentDiscoveryEvent.InvokeAsync().ConfigureAwait(false);
        }

        public void SetUdpEndpoint(IPEndPoint endpoint)
        {
            _udpEndpoint = endpoint;
        }
        private async Task ReceiveAsync(CancellationToken cancelToken)
        {
            var closeTask = Task.Delay(-1, cancelToken);
            while (!cancelToken.IsCancellationRequested)
            {
                var receiveTask = _udp.ReceiveAsync();
                var task = await Task.WhenAny(closeTask, receiveTask).ConfigureAwait(false);
                if (task == closeTask)
                    break;
                
                await _receivedPacketEvent.InvokeAsync(receiveTask.Result.Buffer).ConfigureAwait(false);
            }
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
