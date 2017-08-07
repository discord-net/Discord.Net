#pragma warning disable CS1591
using Discord.API;
using Discord.API.Voice;
using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Discord.Serialization;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class DiscordVoiceApiClient
    {
        public const int MaxBitrate = 128 * 1024;
        public const string Mode = "xsalsa20_poly1305";

        public event Func<string, string, double, Task> SentRequest { add { _sentRequestEvent.Add(value); } remove { _sentRequestEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new AsyncEvent<Func<string, string, double, Task>>();
        public event Func<VoiceOpCode, Task> SentGatewayMessage { add { _sentGatewayMessageEvent.Add(value); } remove { _sentGatewayMessageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<VoiceOpCode, Task>> _sentGatewayMessageEvent = new AsyncEvent<Func<VoiceOpCode, Task>>();
        public event Func<Task> SentDiscovery { add { _sentDiscoveryEvent.Add(value); } remove { _sentDiscoveryEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _sentDiscoveryEvent = new AsyncEvent<Func<Task>>();
        public event Func<int, Task> SentData { add { _sentDataEvent.Add(value); } remove { _sentDataEvent.Remove(value); } }
        private readonly AsyncEvent<Func<int, Task>> _sentDataEvent = new AsyncEvent<Func<int, Task>>();

        public event Func<VoiceOpCode, ReadOnlyBuffer<byte>, Task> ReceivedEvent { add { _receivedEvent.Add(value); } remove { _receivedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<VoiceOpCode, ReadOnlyBuffer<byte>, Task>> _receivedEvent = new AsyncEvent<Func<VoiceOpCode, ReadOnlyBuffer<byte>, Task>>();
        public event Func<byte[], int, int, Task> ReceivedPacket { add { _receivedPacketEvent.Add(value); } remove { _receivedPacketEvent.Remove(value); } }
        private readonly AsyncEvent<Func<byte[], int, int, Task>> _receivedPacketEvent = new AsyncEvent<Func<byte[], int, int, Task>>();
        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        
        private readonly Serializer _serializer;
        private readonly SemaphoreSlim _connectionLock;
        private readonly MemoryStream _decompressionStream;
        protected readonly ConcurrentQueue<ArrayFormatter> _formatters;

        private CancellationTokenSource _connectCancelToken;
        private IUdpSocket _udp;
        private bool _isDisposed;
        private ulong _nextKeepalive;

        public ulong GuildId { get; }
        internal IWebSocketClient WebSocketClient { get; }
        public ConnectionState ConnectionState { get; private set; }

        public ushort UdpPort => _udp.Port;

        internal DiscordVoiceApiClient(ulong guildId, WebSocketProvider webSocketProvider, UdpSocketProvider udpSocketProvider, Serializer serializer)
        {
            GuildId = guildId;
            _connectionLock = new SemaphoreSlim(1, 1);
            _udp = udpSocketProvider();
            _udp.ReceivedDatagram += (data, index, count) => _receivedPacketEvent.InvokeAsync(data, index, count);
            _serializer = serializer;
            _formatters = new ConcurrentQueue<ArrayFormatter>();

            _decompressionStream = new MemoryStream(10 * 1024); //10 KB

            WebSocketClient = webSocketProvider();
            //_gatewayClient.SetHeader("user-agent", DiscordConfig.UserAgent); //(Causes issues in .Net 4.6+)
            WebSocketClient.Message += async (data, isText) =>
            {
                if (!isText)
                {
                    using (var compressed = new MemoryStream(data.ToArray(), 2, data.Length - 2))
                    {
                        _decompressionStream.Position = 0;
                        using (var zlib = new DeflateStream(compressed, CompressionMode.Decompress))
                            zlib.CopyTo(_decompressionStream);
                        _decompressionStream.SetLength(_decompressionStream.Position);

                        _decompressionStream.Position = 0;
                        var msg = _serializer.Read<SocketFrame>(_decompressionStream.ToReadOnlyBuffer());
                        if (msg != null)
                            await _receivedEvent.InvokeAsync((VoiceOpCode)msg.Operation, msg.Payload).ConfigureAwait(false);
                    }
                }
                else
                {
                    var msg = _serializer.Read<SocketFrame>(data);
                    if (msg != null)
                        await _receivedEvent.InvokeAsync((VoiceOpCode)msg.Operation, msg.Payload).ConfigureAwait(false);
                }
            };
            WebSocketClient.Closed += async ex =>
            {
                await DisconnectAsync().ConfigureAwait(false);
                await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
            };
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _connectCancelToken?.Dispose();
                    (_udp as IDisposable)?.Dispose();
                    (WebSocketClient as IDisposable)?.Dispose();
                }
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);

        public async Task SendAsync(VoiceOpCode opCode, object payload, RequestOptions options = null)
        {
            if (_formatters.TryDequeue(out var data1))
                data1 = new ArrayFormatter(128, SymbolTable.InvariantUtf8);
            if (_formatters.TryDequeue(out var data2))
                data2 = new ArrayFormatter(128, SymbolTable.InvariantUtf8);
            try
            {
                payload = new SocketFrame { Operation = (int)opCode, Payload = SerializeJson(data1, payload) };
                await WebSocketClient.SendAsync(SerializeJson(data2, payload), true).ConfigureAwait(false);
                await _sentGatewayMessageEvent.InvokeAsync(opCode).ConfigureAwait(false);
            }
            finally
            {
                _formatters.Enqueue(data1);
                _formatters.Enqueue(data2);
            }
        }
        public async Task SendAsync(byte[] data, int offset, int bytes)
        {
            await _udp.SendAsync(data, offset, bytes).ConfigureAwait(false);                
            await _sentDataEvent.InvokeAsync(bytes).ConfigureAwait(false);
        }

        //WebSocket
        public async Task SendHeartbeatAsync(RequestOptions options = null)
        {
            await SendAsync(VoiceOpCode.Heartbeat, DateTimeUtils.ToUnixMilliseconds(DateTimeOffset.UtcNow), options: options).ConfigureAwait(false);
        }
        public async Task SendIdentityAsync(ulong userId, string sessionId, string token)
        {
            await SendAsync(VoiceOpCode.Identify, new IdentifyParams
            {
                GuildId = GuildId,
                UserId = userId,
                SessionId = sessionId,
                Token = token
            }).ConfigureAwait(false);
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
            }).ConfigureAwait(false);
        }
        public async Task SendSetSpeaking(bool value)
        {
            await SendAsync(VoiceOpCode.Speaking, new SpeakingParams
            {
                IsSpeaking = value,
                Delay = 0
            }).ConfigureAwait(false);
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
                var cancelToken = _connectCancelToken.Token;

                WebSocketClient.SetCancelToken(cancelToken);
                await WebSocketClient.ConnectAsync(url).ConfigureAwait(false);

                _udp.SetCancelToken(cancelToken);
                await _udp.StartAsync().ConfigureAwait(false);

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
            await _udp.StopAsync().ConfigureAwait(false);
            await WebSocketClient.DisconnectAsync().ConfigureAwait(false);

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
            await SendAsync(packet, 0, 70).ConfigureAwait(false);
            await _sentDiscoveryEvent.InvokeAsync().ConfigureAwait(false);
        }
        public async Task<ulong> SendKeepaliveAsync()
        {
            ulong value = _nextKeepalive++;
            var packet = new byte[8];
            packet[0] = (byte)(value >> 0);
            packet[1] = (byte)(value >> 8);
            packet[2] = (byte)(value >> 16);
            packet[3] = (byte)(value >> 24);
            packet[4] = (byte)(value >> 32);
            packet[5] = (byte)(value >> 40);
            packet[6] = (byte)(value >> 48);
            packet[7] = (byte)(value >> 56);
            await SendAsync(packet, 0, 8).ConfigureAwait(false);
            return value;
        }

        public void SetUdpEndpoint(string ip, int port)
        {
            _udp.SetDestination(ip, port);
        }

        //Helpers
        private static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
        protected ReadOnlyBuffer<byte> SerializeJson(ArrayFormatter data, object value)
        {
            _serializer.Write(data, value);
            return new ReadOnlyBuffer<byte>(data.Formatted.Array, 0, data.Formatted.Count);
        }
        protected T DeserializeJson<T>(ReadOnlyBuffer<byte> data)
            where T : class, new()
        {
            return _serializer.Read<T>(data);
        }
    }
}
