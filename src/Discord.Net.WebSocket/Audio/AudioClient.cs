using Discord.API.Voice;
using Discord.Audio.Streams;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    //TODO: Add audio reconnecting
    internal partial class AudioClient : IAudioClient, IDisposable
    {
        internal struct StreamPair
        {
            public AudioInStream Reader;
            public AudioOutStream Writer;

            public StreamPair(AudioInStream reader, AudioOutStream writer)
            {
                Reader = reader;
                Writer = writer;
            }
        }

        private readonly Logger _audioLogger;
        private readonly JsonSerializer _serializer;
        private readonly ConnectionManager _connection;
        private readonly SemaphoreSlim _stateLock;
        private readonly ConcurrentQueue<long> _heartbeatTimes;
        private readonly ConcurrentDictionary<uint, ulong> _ssrcMap;
        private readonly ConcurrentDictionary<ulong, StreamPair> _streams;

        private Task _heartbeatTask;
        private long _lastMessageTime;
        private string _url, _sessionId, _token;
        private ulong _userId;
        private uint _ssrc;
        private byte[] _secretKey;

        public SocketGuild Guild { get; }
        public DiscordVoiceAPIClient ApiClient { get; private set; }
        public int Latency { get; private set; }
        public ulong ChannelId { get; internal set; }

        private DiscordSocketClient Discord => Guild.Discord;
        public ConnectionState ConnectionState => _connection.State;

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        internal AudioClient(SocketGuild guild, int clientId, ulong channelId)
        {
            Guild = guild;
            ChannelId = channelId;
            _audioLogger = Discord.LogManager.CreateLogger($"Audio #{clientId}");

            ApiClient = new DiscordVoiceAPIClient(guild.Id, Discord.WebSocketProvider, Discord.UdpSocketProvider);
            ApiClient.SentGatewayMessage += async opCode => await _audioLogger.DebugAsync($"Sent {opCode}").ConfigureAwait(false);
            ApiClient.SentDiscovery += async () => await _audioLogger.DebugAsync($"Sent Discovery").ConfigureAwait(false);
            //ApiClient.SentData += async bytes => await _audioLogger.DebugAsync($"Sent {bytes} Bytes").ConfigureAwait(false);
            ApiClient.ReceivedEvent += ProcessMessageAsync;
            ApiClient.ReceivedPacket += ProcessPacketAsync;

            _stateLock = new SemaphoreSlim(1, 1);
            _connection = new ConnectionManager(_stateLock, _audioLogger, 30000, 
                OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
            _connection.Connected += () => _connectedEvent.InvokeAsync();
            _connection.Disconnected += (ex, recon) => _disconnectedEvent.InvokeAsync(ex);
            _heartbeatTimes = new ConcurrentQueue<long>();
            _ssrcMap = new ConcurrentDictionary<uint, ulong>();
            _streams = new ConcurrentDictionary<ulong, StreamPair>();
            
            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _audioLogger.WarningAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };

            LatencyUpdated += async (old, val) => await _audioLogger.DebugAsync($"Latency = {val} ms").ConfigureAwait(false);
        }

        internal async Task StartAsync(string url, ulong userId, string sessionId, string token) 
        {
            _url = url;
            _userId = userId;
            _sessionId = sessionId;
            _token = token;
            await _connection.StartAsync().ConfigureAwait(false);
        }
        public async Task StopAsync()
        { 
            await _connection.StopAsync().ConfigureAwait(false);
        }

        private async Task OnConnectingAsync()
        {
            await _audioLogger.DebugAsync("Connecting ApiClient").ConfigureAwait(false);
            await ApiClient.ConnectAsync("wss://" + _url).ConfigureAwait(false);
            await _audioLogger.DebugAsync("Sending Identity").ConfigureAwait(false);
            await ApiClient.SendIdentityAsync(_userId, _sessionId, _token).ConfigureAwait(false);

            //Wait for READY
            await _connection.WaitAsync().ConfigureAwait(false);
        }
        private async Task OnDisconnectingAsync(Exception ex)
        {
            await _audioLogger.DebugAsync("Disconnecting ApiClient").ConfigureAwait(false);
            await ApiClient.DisconnectAsync().ConfigureAwait(false);

            //Wait for tasks to complete
            await _audioLogger.DebugAsync("Waiting for heartbeater").ConfigureAwait(false);
            var heartbeatTask = _heartbeatTask;
            if (heartbeatTask != null)
                await heartbeatTask.ConfigureAwait(false);
            _heartbeatTask = null;

            long time;
            while (_heartbeatTimes.TryDequeue(out time)) { }
            _lastMessageTime = 0;

            await ClearInputStreamsAsync().ConfigureAwait(false);

            await _audioLogger.DebugAsync("Sending Voice State").ConfigureAwait(false);
            await Discord.ApiClient.SendVoiceStateUpdateAsync(Guild.Id, null, false, false).ConfigureAwait(false);
        }

        public AudioOutStream CreateOpusStream(int samplesPerFrame, int bufferMillis)
        {
            CheckSamplesPerFrame(samplesPerFrame);
            var outputStream = new OutputStream(ApiClient);
            var sodiumEncrypter = new SodiumEncryptStream(outputStream, _secretKey);
            var rtpWriter = new RTPWriteStream(sodiumEncrypter, samplesPerFrame, _ssrc);
            return new BufferedWriteStream(rtpWriter, samplesPerFrame, bufferMillis, _connection.CancelToken, _audioLogger);
        }
        public AudioOutStream CreateDirectOpusStream(int samplesPerFrame)
        {
            CheckSamplesPerFrame(samplesPerFrame);
            var outputStream = new OutputStream(ApiClient);
            var sodiumEncrypter = new SodiumEncryptStream(outputStream, _secretKey);
            return new RTPWriteStream(sodiumEncrypter, samplesPerFrame, _ssrc);
        }
        public AudioOutStream CreatePCMStream(AudioApplication application, int samplesPerFrame, int channels, int? bitrate, int bufferMillis)
        {
            CheckSamplesPerFrame(samplesPerFrame);
            var outputStream = new OutputStream(ApiClient);
            var sodiumEncrypter = new SodiumEncryptStream(outputStream, _secretKey);
            var rtpWriter = new RTPWriteStream(sodiumEncrypter, samplesPerFrame, _ssrc);
            var bufferedStream = new BufferedWriteStream(rtpWriter, samplesPerFrame, bufferMillis, _connection.CancelToken, _audioLogger);
            return new OpusEncodeStream(bufferedStream, channels, samplesPerFrame, bitrate ?? (96 * 1024), application);
        }
        public AudioOutStream CreateDirectPCMStream(AudioApplication application, int samplesPerFrame, int channels, int? bitrate)
        {
            CheckSamplesPerFrame(samplesPerFrame);
            var outputStream = new OutputStream(ApiClient);
            var sodiumEncrypter = new SodiumEncryptStream(outputStream, _secretKey);
            var rtpWriter = new RTPWriteStream(sodiumEncrypter, samplesPerFrame, _ssrc);
            return new OpusEncodeStream(rtpWriter, channels, samplesPerFrame, bitrate ?? (96 * 1024), application);
        }
        private void CheckSamplesPerFrame(int samplesPerFrame)
        {
            if (samplesPerFrame != 120 && samplesPerFrame != 240 && samplesPerFrame != 480 &&
                samplesPerFrame != 960 && samplesPerFrame != 1920 && samplesPerFrame != 2880)
                throw new ArgumentException("Value must be 120, 240, 480, 960, 1920 or 2880", nameof(samplesPerFrame));
        }

        internal async Task CreateInputStreamAsync(ulong userId)
        {
            //Assume Thread-safe
            if (!_streams.ContainsKey(userId))
            {
                var readerStream = new InputStream();
                var writerStream = new OpusDecodeStream(new RTPReadStream(readerStream, _secretKey));
                _streams.TryAdd(userId, new StreamPair(readerStream, writerStream));
                await _streamCreatedEvent.InvokeAsync(userId, readerStream);
            }
        }
        internal AudioInStream GetInputStream(ulong id)
        {
            StreamPair streamPair;
            if (_streams.TryGetValue(id, out streamPair))
                return streamPair.Reader;
            return null;
        }
        internal async Task RemoveInputStreamAsync(ulong userId)
        {
            if (_streams.TryRemove(userId, out var pair))
            {
                await _streamDestroyedEvent.InvokeAsync(userId).ConfigureAwait(false);
                pair.Reader.Dispose();
            }
        }
        internal async Task ClearInputStreamsAsync()
        {
            foreach (var pair in _streams)
            {
                await _streamDestroyedEvent.InvokeAsync(pair.Key).ConfigureAwait(false);
                pair.Value.Reader.Dispose();
            }
            _ssrcMap.Clear();
            _streams.Clear();
        }

        private async Task ProcessMessageAsync(VoiceOpCode opCode, object payload)
        {
            _lastMessageTime = Environment.TickCount;

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

                            _heartbeatTask = RunHeartbeatAsync(data.HeartbeatInterval, _connection.CancelToken);
                            
                            ApiClient.SetUdpEndpoint(data.Ip, data.Port);
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

                            var _ = _connection.CompleteAsync();
                        }
                        break;
                    case VoiceOpCode.HeartbeatAck:
                        {
                            await _audioLogger.DebugAsync("Received HeartbeatAck").ConfigureAwait(false);

                            long time;
                            if (_heartbeatTimes.TryDequeue(out time))
                            {
                                int latency = (int)(Environment.TickCount - time);
                                int before = Latency;
                                Latency = latency;

                                await _latencyUpdatedEvent.InvokeAsync(before, latency).ConfigureAwait(false);
                            }
                        }
                        break;
                    case VoiceOpCode.Speaking:
                        {
                            await _audioLogger.DebugAsync("Received Speaking").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<SpeakingEvent>(_serializer);
                            _ssrcMap[data.Ssrc] = data.UserId; //TODO: Memory Leak: SSRCs are never cleaned up

                            await _speakingUpdatedEvent.InvokeAsync(data.UserId, data.Speaking);
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
        }
        private async Task ProcessPacketAsync(byte[] packet)
        {
            if (_connection.State == ConnectionState.Connecting)
            {
                if (packet.Length != 70)
                {
                    await _audioLogger.DebugAsync($"Malformed Packet").ConfigureAwait(false);
                    return;
                }
                string ip;
                int port;
                try
                {
                    ip = Encoding.UTF8.GetString(packet, 4, 70 - 6).TrimEnd('\0');
                    port = (packet[69] << 8) | packet[68];
                }
                catch (Exception ex)
                {
                    await _audioLogger.DebugAsync($"Malformed Packet", ex).ConfigureAwait(false);
                    return; 
                }
                
                await _audioLogger.DebugAsync("Received Discovery").ConfigureAwait(false);
                await ApiClient.SendSelectProtocol(ip, port).ConfigureAwait(false);
            }
            else if (_connection.State == ConnectionState.Connected)
            {
                uint ssrc;
                ulong userId;
                StreamPair pair;

                if (!RTPReadStream.TryReadSsrc(packet, 0, out ssrc))
                {
                    await _audioLogger.DebugAsync($"Malformed Frame").ConfigureAwait(false);
                    return;
                }
                if (!_ssrcMap.TryGetValue(ssrc, out userId))
                {
                    await _audioLogger.DebugAsync($"Unknown SSRC {ssrc}").ConfigureAwait(false);
                    return;
                }
                if (!_streams.TryGetValue(userId, out pair))
                {
                    await _audioLogger.DebugAsync($"Unknown User {userId}").ConfigureAwait(false);
                    return;
                }
                try
                {
                    await pair.Writer.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _audioLogger.DebugAsync($"Malformed Frame", ex).ConfigureAwait(false);
                    return;
                }
                await _audioLogger.DebugAsync($"Received {packet.Length} bytes from user {userId}").ConfigureAwait(false);
            }
        }

        private async Task RunHeartbeatAsync(int intervalMillis, CancellationToken cancelToken)
        {
            //TODO: Clean this up when Discord's session patch is live
            try
            {
                await _audioLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
                while (!cancelToken.IsCancellationRequested)
                {
                    var now = Environment.TickCount;

                    //Did server respond to our last heartbeat?
                    if (_heartbeatTimes.Count != 0 && (now - _lastMessageTime) > intervalMillis && 
                        ConnectionState == ConnectionState.Connected)
                    {
                        _connection.Error(new Exception("Server missed last heartbeat"));
                        return;
                    }

                    _heartbeatTimes.Enqueue(now);
                    try
                    {
                        await ApiClient.SendHeartbeatAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await _audioLogger.WarningAsync("Heartbeat Errored", ex).ConfigureAwait(false);
                    }

                    await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);
                }
                await _audioLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await _audioLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _audioLogger.ErrorAsync("Heartbeat Errored", ex).ConfigureAwait(false);
            }
        }

        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAsync().GetAwaiter().GetResult();
                ApiClient.Dispose();
            }
        }
        /// <inheritdoc />
        public void Dispose() => Dispose(true);
    }
}
