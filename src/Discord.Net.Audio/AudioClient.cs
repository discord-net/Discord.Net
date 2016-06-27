using Discord.API;
using Discord.API.Voice;
using Discord.Net.Converters;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public class AudioClient
    {
        public const int MaxBitrate = 128;

        private const string Mode = "xsalsa20_poly1305";

        private readonly JsonSerializer _serializer;
        private readonly IWebSocketClient _gatewayClient;
        private readonly SemaphoreSlim _connectionLock;
        private CancellationTokenSource _connectCancelToken;

        public ConnectionState ConnectionState { get; private set; }

        internal AudioClient(WebSocketProvider provider, JsonSerializer serializer = null)
        {
            _connectionLock = new SemaphoreSlim(1, 1);

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

        //Gateway
        public async Task SendHeartbeatAsync(int lastSeq, RequestOptions options = null)
        {
            await SendAsync(VoiceOpCode.Heartbeat, lastSeq, options: options).ConfigureAwait(false);
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
