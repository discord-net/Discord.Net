using Discord.API.Voice;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class AudioClient : IAudioClient, IDisposable
    {
        public event Func<Task> Connected
        {
            add { _connectedEvent.Add(value); }
            remove { _connectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        public event Func<Exception, Task> Disconnected
        {
            add { _disconnectedEvent.Add(value); }
            remove { _disconnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        public event Func<int, int, Task> LatencyUpdated
        {
            add { _latencyUpdatedEvent.Add(value); }
            remove { _latencyUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();

        private readonly ILogger _audioLogger;
#if BENCHMARK
        private readonly ILogger _benchmarkLogger;
#endif
        internal readonly SemaphoreSlim _connectionLock;
        private readonly JsonSerializer _serializer;

        private TaskCompletionSource<bool> _connectTask;
        private CancellationTokenSource _cancelToken;
        private Task _heartbeatTask;
        private long _heartbeatTime;
        private string _url;
        private bool _isDisposed;
        private uint _ssrc;
        private byte[] _secretKey;

        public SocketGuild Guild { get; }
        public DiscordVoiceAPIClient ApiClient { get; private set; }
        public ConnectionState ConnectionState { get; private set; }
        public int Latency { get; private set; }

        private DiscordSocketClient Discord => Guild.Discord;

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public AudioClient(SocketGuild guild, int id)
        {
            Guild = guild;

            _audioLogger = Discord.LogManager.CreateLogger($"Audio #{id}");
#if BENCHMARK
            _benchmarkLogger = logManager.CreateLogger("Benchmark");
#endif

            _connectionLock = new SemaphoreSlim(1, 1);

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _audioLogger.WarningAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };
            
            ApiClient = new DiscordVoiceAPIClient(guild.Id, Discord.WebSocketProvider);

            ApiClient.SentGatewayMessage += async opCode => await _audioLogger.DebugAsync($"Sent {opCode}").ConfigureAwait(false);
            ApiClient.SentDiscovery += async () => await _audioLogger.DebugAsync($"Sent Discovery").ConfigureAwait(false);
            ApiClient.SentData += async bytes => await _audioLogger.DebugAsync($"Sent {bytes} Bytes").ConfigureAwait(false);
            ApiClient.ReceivedEvent += ProcessMessageAsync;
            ApiClient.ReceivedPacket += ProcessPacketAsync;
            ApiClient.Disconnected += async ex =>
            {
                if (ex != null)
                    await _audioLogger.WarningAsync($"Connection Closed", ex).ConfigureAwait(false);
                else
                    await _audioLogger.WarningAsync($"Connection Closed").ConfigureAwait(false);
            };

            LatencyUpdated += async (old, val) => await _audioLogger.VerboseAsync($"Latency = {val} ms").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ConnectAsync(string url, ulong userId, string sessionId, string token)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync(url, userId, sessionId, token).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync(string url, ulong userId, string sessionId, string token)
        {
            var state = ConnectionState;
            if (state == ConnectionState.Connecting || state == ConnectionState.Connected)
                await DisconnectInternalAsync(null).ConfigureAwait(false);

            ConnectionState = ConnectionState.Connecting;
            await _audioLogger.InfoAsync("Connecting").ConfigureAwait(false);
            try
            {
                _url = url;
                _connectTask = new TaskCompletionSource<bool>();
                _cancelToken = new CancellationTokenSource();

                await ApiClient.ConnectAsync("wss://" + url).ConfigureAwait(false);
                await ApiClient.SendIdentityAsync(userId, sessionId, token).ConfigureAwait(false);
                await _connectTask.Task.ConfigureAwait(false);

                await _connectedEvent.InvokeAsync().ConfigureAwait(false);
                ConnectionState = ConnectionState.Connected;
                await _audioLogger.InfoAsync("Connected").ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DisconnectInternalAsync(null).ConfigureAwait(false);
                throw;
            }
        }
        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(null).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectAsync(Exception ex)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(ex).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync(Exception ex)
        {
            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            await _audioLogger.InfoAsync("Disconnecting").ConfigureAwait(false);

            //Signal tasks to complete
            try { _cancelToken.Cancel(); } catch { }

            //Disconnect from server
            await ApiClient.DisconnectAsync().ConfigureAwait(false);

            //Wait for tasks to complete
            var heartbeatTask = _heartbeatTask;
            if (heartbeatTask != null)
                await heartbeatTask.ConfigureAwait(false);
            _heartbeatTask = null;

            ConnectionState = ConnectionState.Disconnected;
            await _audioLogger.InfoAsync("Disconnected").ConfigureAwait(false);

            await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
        }

        public void Send(byte[] data, int count)
        {
            //TODO: Queue these?
            ApiClient.SendAsync(data, count).ConfigureAwait(false);
        }

        public RTPWriteStream CreateOpusStream(int samplesPerFrame, int bufferSize = 4000)
        {
            return new RTPWriteStream(this, _secretKey, samplesPerFrame, _ssrc, bufferSize = 4000);
        }
        public OpusEncodeStream CreatePCMStream(int samplesPerFrame, int? bitrate = null,
            OpusApplication application = OpusApplication.MusicOrMixed, int bufferSize = 4000)
        {
            return new OpusEncodeStream(this, _secretKey, samplesPerFrame, _ssrc, bitrate, application, bufferSize);
        }

        private async Task ProcessMessageAsync(VoiceOpCode opCode, object payload)
        {
#if BENCHMARK
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
#endif
            try
            {
                switch (opCode)
                {
                    case VoiceOpCode.Ready:
                        {
                            await _audioLogger.DebugAsync("Received Ready").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<ReadyEvent>(_serializer);

                            _ssrc = data.SSRC;

                            if (!data.Modes.Contains(DiscordVoiceAPIClient.Mode))
                                throw new InvalidOperationException($"Discord does not support {DiscordVoiceAPIClient.Mode}");

                            _heartbeatTime = 0;
                            _heartbeatTask = RunHeartbeatAsync(data.HeartbeatInterval, _cancelToken.Token);

                            var entry = await Dns.GetHostEntryAsync(_url).ConfigureAwait(false);

                            ApiClient.SetUdpEndpoint(new IPEndPoint(entry.AddressList[0], data.Port));
                            await ApiClient.SendDiscoveryAsync(_ssrc).ConfigureAwait(false);
                        }
                        break;
                    case VoiceOpCode.SessionDescription:
                        {
                            await _audioLogger.DebugAsync("Received SessionDescription").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<SessionDescriptionEvent>(_serializer);

                            if (data.Mode != DiscordVoiceAPIClient.Mode)
                                throw new InvalidOperationException($"Discord selected an unexpected mode: {data.Mode}");

                            _secretKey = data.SecretKey;
                            await ApiClient.SendSetSpeaking(true).ConfigureAwait(false);

                            var _ = _connectTask.TrySetResultAsync(true);
                        }
                        break;
                    case VoiceOpCode.HeartbeatAck:
                        {
                            await _audioLogger.DebugAsync("Received HeartbeatAck").ConfigureAwait(false);

                            var heartbeatTime = _heartbeatTime;
                            if (heartbeatTime != 0)
                            {
                                int latency = (int)(Environment.TickCount - _heartbeatTime);
                                _heartbeatTime = 0;

                                int before = Latency;
                                Latency = latency;

                                await _latencyUpdatedEvent.InvokeAsync(before, latency).ConfigureAwait(false);
                            }
                        }
                        break;
                    default:
                        await _audioLogger.WarningAsync($"Unknown OpCode ({opCode})").ConfigureAwait(false);
                        return;
                }
            }
            catch (Exception ex)
            {
                await _audioLogger.ErrorAsync($"Error handling {opCode}", ex).ConfigureAwait(false);
                return;
            }
#if BENCHMARK
            }
            finally
            {
                stopwatch.Stop();
                double millis = Math.Round(stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
                await _benchmarkLogger.DebugAsync($"{millis} ms").ConfigureAwait(false);
            }
#endif
        }
        private async Task ProcessPacketAsync(byte[] packet)
        {
            if (!_connectTask.Task.IsCompleted)
            {
                if (packet.Length == 70)
                {
                    string ip;
                    int port;
                    try
                    {
                        ip = Encoding.UTF8.GetString(packet, 4, 70 - 6).TrimEnd('\0');
                        port = packet[69] | (packet[68] << 8);
                    }
                    catch { return; }
                    
                    await _audioLogger.DebugAsync("Received Discovery").ConfigureAwait(false);
                    await ApiClient.SendSelectProtocol(ip, port);
                }
            }
        }

        private async Task RunHeartbeatAsync(int intervalMillis, CancellationToken cancelToken)
        {
            //Clean this up when Discord's session patch is live
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);

                    if (_heartbeatTime != 0) //Server never responded to our last heartbeat
                    {
                        if (ConnectionState == ConnectionState.Connected)
                        {
                            await _audioLogger.WarningAsync("Server missed last heartbeat").ConfigureAwait(false);
                            await DisconnectInternalAsync(new Exception("Server missed last heartbeat")).ConfigureAwait(false);
                            return;
                        }
                    }
                    else
                        _heartbeatTime = Environment.TickCount;
                    await ApiClient.SendHeartbeatAsync().ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { }
        }

        internal virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
                _isDisposed = true;
            ApiClient.Dispose();
        }
        /// <inheritdoc />
        public void Dispose() => Dispose(true);
    }
}
