using Discord.API.Voice;
using Discord.Audio.Streams;
using Discord.Logging;
using Discord.Net;
using Discord.Net.Converters;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal partial class AudioClient : IAudioClient
    {
        private static readonly int ConnectionTimeoutMs = 30000; // 30 seconds
        private static readonly int KeepAliveIntervalMs = 5000; // 5 seconds

        private static readonly int[] BlacklistedResumeCodes = new int[]
        {
            4001, 4002, 4003, 4004, 4005, 4006, 4009, 4012, 1014, 4016
        };

        private struct StreamPair
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
        private readonly ConcurrentQueue<KeyValuePair<ulong, int>> _keepaliveTimes;
        private readonly ConcurrentDictionary<uint, ulong> _ssrcMap;
        private readonly ConcurrentDictionary<ulong, StreamPair> _streams;

        private Task _heartbeatTask, _keepaliveTask;
        private int _heartbeatInterval;
        private long _lastMessageTime;
        private string _url, _sessionId, _token;
        private ulong _userId;
        private uint _ssrc;
        private bool _isSpeaking;
        private StopReason _stopReason;
        private bool _resuming;

        public SocketGuild Guild { get; }
        public DiscordVoiceAPIClient ApiClient { get; private set; }
        public int Latency { get; private set; }
        public int UdpLatency { get; private set; }
        public ulong ChannelId { get; internal set; }
        internal byte[] SecretKey { get; private set; }
        internal bool IsFinished { get; private set; }

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
            ApiClient.SentDiscovery += async () => await _audioLogger.DebugAsync("Sent Discovery").ConfigureAwait(false);
            //ApiClient.SentData += async bytes => await _audioLogger.DebugAsync($"Sent {bytes} Bytes").ConfigureAwait(false);
            ApiClient.ReceivedEvent += ProcessMessageAsync;
            ApiClient.ReceivedPacket += ProcessPacketAsync;

            _stateLock = new SemaphoreSlim(1, 1);
            _connection = new ConnectionManager(_stateLock, _audioLogger, ConnectionTimeoutMs,
                OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
            _connection.Connected += () => _connectedEvent.InvokeAsync();
            _connection.Disconnected += (exception, _) => _disconnectedEvent.InvokeAsync(exception);
            _heartbeatTimes = new ConcurrentQueue<long>();
            _keepaliveTimes = new ConcurrentQueue<KeyValuePair<ulong, int>>();
            _ssrcMap = new ConcurrentDictionary<uint, ulong>();
            _streams = new ConcurrentDictionary<ulong, StreamPair>();

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _audioLogger.WarningAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };

            LatencyUpdated += async (old, val) => await _audioLogger.DebugAsync($"Latency = {val} ms").ConfigureAwait(false);
            UdpLatencyUpdated += async (old, val) => await _audioLogger.DebugAsync($"UDP Latency = {val} ms").ConfigureAwait(false);
        }

        internal Task StartAsync(string url, ulong userId, string sessionId, string token)
        {
            _url = url;
            _userId = userId;
            _sessionId = sessionId;
            _token = token;
            return _connection.StartAsync();
        }

        public IReadOnlyDictionary<ulong, AudioInStream> GetStreams()
        {
            return _streams.ToDictionary(pair => pair.Key, pair => pair.Value.Reader);
        }

        public Task StopAsync()
            => StopAsync(StopReason.Normal);

        internal Task StopAsync(StopReason stopReason)
        {
            _stopReason = stopReason;
            return _connection.StopAsync();
        }

        private async Task OnConnectingAsync()
        {
            await _audioLogger.DebugAsync($"Connecting ApiClient. Voice server: wss://{_url}").ConfigureAwait(false);
            await ApiClient.ConnectAsync($"wss://{_url}?v={DiscordConfig.VoiceAPIVersion}").ConfigureAwait(false);
            await _audioLogger.DebugAsync($"Listening on port {ApiClient.UdpPort}").ConfigureAwait(false);

            if (!_resuming)
            {
                await _audioLogger.DebugAsync("Sending Identity").ConfigureAwait(false);
                await ApiClient.SendIdentityAsync(_userId, _sessionId, _token).ConfigureAwait(false);
            }
            else
            {
                await _audioLogger.DebugAsync("Sending Resume").ConfigureAwait(false);
                await ApiClient.SendResume(_token, _sessionId).ConfigureAwait(false);
            }

            //Wait for READY
            await _connection.WaitAsync().ConfigureAwait(false);
        }
        private async Task OnDisconnectingAsync(Exception ex)
        {
            await _audioLogger.DebugAsync("Disconnecting ApiClient").ConfigureAwait(false);
            await ApiClient.DisconnectAsync().ConfigureAwait(false);

            if (_stopReason == StopReason.Unknown && ex.InnerException is WebSocketException exception)
            {
                await _audioLogger.WarningAsync(
                $"Audio connection terminated with unknown reason. Code: {exception.ErrorCode} - {exception.Message}",
                exception);

                if (_resuming)
                {
                    await _audioLogger.WarningAsync("Resume failed");

                    _resuming = false;

                    await FinishDisconnect(ex, true);
                    return;
                }

                if (BlacklistedResumeCodes.Contains(exception.ErrorCode))
                {
                    await FinishDisconnect(ex, true);
                    return;
                }

                await ClearHeartBeaters();

                _resuming = true;
                return;
            }

            await FinishDisconnect(ex, _stopReason != StopReason.Moved);

            if (_stopReason == StopReason.Normal)
            {
                await _audioLogger.DebugAsync("Sending Voice State").ConfigureAwait(false);
                await Discord.ApiClient.SendVoiceStateUpdateAsync(Guild.Id, null, false, false).ConfigureAwait(false);
            }

            _stopReason = StopReason.Unknown;
        }

        private async Task FinishDisconnect(Exception ex, bool wontTryReconnect)
        {
            await _audioLogger.DebugAsync("Finishing audio connection").ConfigureAwait(false);

            await ClearHeartBeaters().ConfigureAwait(false);

            if (wontTryReconnect)
            {
                await _connection.StopAsync().ConfigureAwait(false);

                await ClearInputStreamsAsync().ConfigureAwait(false);

                IsFinished = true;
            }
        }

        private async Task ClearHeartBeaters()
        {
            //Wait for tasks to complete
            await _audioLogger.DebugAsync("Waiting for heartbeater").ConfigureAwait(false);

            if (_heartbeatTask != null)
                await _heartbeatTask.ConfigureAwait(false);
            _heartbeatTask = null;

            if (_keepaliveTask != null)
                await _keepaliveTask.ConfigureAwait(false);
            _keepaliveTask = null;

            while (_heartbeatTimes.TryDequeue(out _))
            { }
            _lastMessageTime = 0;

            while (_keepaliveTimes.TryDequeue(out _))
            { }
        }

        #region Streams
        public AudioOutStream CreateOpusStream(int bufferMillis)
        {
            var outputStream = new OutputStream(ApiClient); //Ignores header
            var sodiumEncrypter = new SodiumEncryptStream(outputStream, this); //Passes header
            var rtpWriter = new RTPWriteStream(sodiumEncrypter, _ssrc); //Consumes header, passes
            return new BufferedWriteStream(rtpWriter, this, bufferMillis, _connection.CancelToken, _audioLogger); //Generates header
        }
        public AudioOutStream CreateDirectOpusStream()
        {
            var outputStream = new OutputStream(ApiClient); //Ignores header
            var sodiumEncrypter = new SodiumEncryptStream(outputStream, this); //Passes header
            return new RTPWriteStream(sodiumEncrypter, _ssrc); //Consumes header (external input), passes
        }
        public AudioOutStream CreatePCMStream(AudioApplication application, int? bitrate, int bufferMillis, int packetLoss)
        {
            var outputStream = new OutputStream(ApiClient); //Ignores header
            var sodiumEncrypter = new SodiumEncryptStream(outputStream, this); //Passes header
            var rtpWriter = new RTPWriteStream(sodiumEncrypter, _ssrc); //Consumes header, passes
            var bufferedStream = new BufferedWriteStream(rtpWriter, this, bufferMillis, _connection.CancelToken, _audioLogger); //Ignores header, generates header
            return new OpusEncodeStream(bufferedStream, bitrate ?? (96 * 1024), application, packetLoss); //Generates header
        }
        public AudioOutStream CreateDirectPCMStream(AudioApplication application, int? bitrate, int packetLoss)
        {
            var outputStream = new OutputStream(ApiClient); //Ignores header
            var sodiumEncrypter = new SodiumEncryptStream(outputStream, this); //Passes header
            var rtpWriter = new RTPWriteStream(sodiumEncrypter, _ssrc); //Consumes header, passes
            return new OpusEncodeStream(rtpWriter, bitrate ?? (96 * 1024), application, packetLoss); //Generates header
        }

        internal async Task CreateInputStreamAsync(ulong userId)
        {
            //Assume Thread-safe
            if (!_streams.ContainsKey(userId))
            {
                var readerStream = new InputStream(); //Consumes header
                var opusDecoder = new OpusDecodeStream(readerStream); //Passes header
                //var jitterBuffer = new JitterBuffer(opusDecoder, _audioLogger);
                var rtpReader = new RTPReadStream(opusDecoder); //Generates header
                var decryptStream = new SodiumDecryptStream(rtpReader, this); //No header
                _streams.TryAdd(userId, new StreamPair(readerStream, decryptStream));
                await _streamCreatedEvent.InvokeAsync(userId, readerStream);
            }
        }
        internal AudioInStream GetInputStream(ulong id)
        {
            if (_streams.TryGetValue(id, out StreamPair streamPair))
                return streamPair.Reader;

            return null;
        }
        internal async Task RemoveInputStreamAsync(ulong userId)
        {
            if (_streams.TryRemove(userId, out StreamPair pair))
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
        #endregion

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

                            ApiClient.SetUdpEndpoint(data.Ip, data.Port);
                            await ApiClient.SendDiscoveryAsync(_ssrc).ConfigureAwait(false);

                            _heartbeatTask = RunHeartbeatAsync(_heartbeatInterval, _connection.CancelToken);
                        }
                        break;
                    case VoiceOpCode.SessionDescription:
                        {
                            await _audioLogger.DebugAsync("Received SessionDescription").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<SessionDescriptionEvent>(_serializer);

                            if (data.Mode != DiscordVoiceAPIClient.Mode)
                                throw new InvalidOperationException($"Discord selected an unexpected mode: {data.Mode}");

                            SecretKey = data.SecretKey;
                            _isSpeaking = false;
                            await ApiClient.SendSetSpeaking(_isSpeaking).ConfigureAwait(false);
                            _keepaliveTask = RunKeepaliveAsync(_connection.CancelToken);

                            _ = _connection.CompleteAsync();
                        }
                        break;
                    case VoiceOpCode.HeartbeatAck:
                        {
                            await _audioLogger.DebugAsync("Received HeartbeatAck").ConfigureAwait(false);

                            if (_heartbeatTimes.TryDequeue(out long time))
                            {
                                int latency = (int)(Environment.TickCount - time);
                                int before = Latency;
                                Latency = latency;

                                await _latencyUpdatedEvent.InvokeAsync(before, latency).ConfigureAwait(false);
                            }
                        }
                        break;
                    case VoiceOpCode.Hello:
                        {
                            await _audioLogger.DebugAsync("Received Hello").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<HelloEvent>(_serializer);

                            _heartbeatInterval = data.HeartbeatInterval;
                        }
                        break;
                    case VoiceOpCode.Speaking:
                        {
                            await _audioLogger.DebugAsync("Received Speaking").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<SpeakingEvent>(_serializer);
                            _ssrcMap[data.Ssrc] = data.UserId;

                            await _speakingUpdatedEvent.InvokeAsync(data.UserId, data.Speaking);
                        }
                        break;
                    case VoiceOpCode.ClientDisconnect:
                        {
                            await _audioLogger.DebugAsync("Received ClientDisconnect").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<ClientDisconnectEvent>(_serializer);

                            await _clientDisconnectedEvent.InvokeAsync(data.UserId);
                        }
                        break;
                    case VoiceOpCode.Resumed:
                        {
                            await _audioLogger.DebugAsync($"Voice connection resumed: wss://{_url}");
                            _resuming = false;

                            _heartbeatTask = RunHeartbeatAsync(_heartbeatInterval, _connection.CancelToken);
                            _keepaliveTask = RunKeepaliveAsync(_connection.CancelToken);

                            _ = _connection.CompleteAsync();
                        }
                        break;
                    default:
                        await _audioLogger.WarningAsync($"Unknown OpCode ({opCode})").ConfigureAwait(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                await _audioLogger.ErrorAsync($"Error handling {opCode}", ex).ConfigureAwait(false);
            }
        }
        private async Task ProcessPacketAsync(byte[] packet)
        {
            try
            {
                if (_connection.State == ConnectionState.Connecting)
                {
                    if (packet.Length != 74)
                    {
                        await _audioLogger.DebugAsync("Malformed Packet").ConfigureAwait(false);
                        return;
                    }
                    string ip;
                    try
                    {
                        ip = Encoding.UTF8.GetString(packet, 8, 74 - 10).TrimEnd('\0');
                    }
                    catch (Exception ex)
                    {
                        await _audioLogger.DebugAsync("Malformed Packet", ex).ConfigureAwait(false);
                        return;
                    }

                    await _audioLogger.DebugAsync("Received Discovery").ConfigureAwait(false);
                    await ApiClient.SendSelectProtocol(ip).ConfigureAwait(false);
                }
                else if (_connection.State == ConnectionState.Connected)
                {
                    if (packet.Length == 8)
                    {
                        await _audioLogger.DebugAsync("Received Keepalive").ConfigureAwait(false);

                        ulong value =
                            ((ulong)packet[0] >> 0) |
                            ((ulong)packet[1] >> 8) |
                            ((ulong)packet[2] >> 16) |
                            ((ulong)packet[3] >> 24) |
                            ((ulong)packet[4] >> 32) |
                            ((ulong)packet[5] >> 40) |
                            ((ulong)packet[6] >> 48) |
                            ((ulong)packet[7] >> 56);

                        while (_keepaliveTimes.TryDequeue(out var pair))
                        {
                            if (pair.Key == value)
                            {
                                int latency = Environment.TickCount - pair.Value;
                                int before = UdpLatency;
                                UdpLatency = latency;

                                await _udpLatencyUpdatedEvent.InvokeAsync(before, latency).ConfigureAwait(false);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!RTPReadStream.TryReadSsrc(packet, 0, out uint ssrc))
                        {
                            await _audioLogger.DebugAsync("Malformed Frame").ConfigureAwait(false);
                        }
                        else if (!_ssrcMap.TryGetValue(ssrc, out ulong userId))
                        {
                            await _audioLogger.DebugAsync($"Unknown SSRC {ssrc}").ConfigureAwait(false);
                        }
                        else if (!_streams.TryGetValue(userId, out StreamPair pair))
                        {
                            await _audioLogger.DebugAsync($"Unknown User {userId}").ConfigureAwait(false);
                        }
                        else
                        {
                            try
                            {
                                await pair.Writer.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                await _audioLogger.DebugAsync("Malformed Frame", ex).ConfigureAwait(false);
                            }
                        }
                        //await _audioLogger.DebugAsync($"Received {packet.Length} bytes from user {userId}").ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                await _audioLogger.WarningAsync("Failed to process UDP packet", ex).ConfigureAwait(false);
            }
        }

        private async Task RunHeartbeatAsync(int intervalMillis, CancellationToken cancelToken)
        {
            int delayInterval = (int)(intervalMillis * DiscordConfig.HeartbeatIntervalFactor);

            // TODO: Clean this up when Discord's session patch is live
            try
            {
                await _audioLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
                while (!cancelToken.IsCancellationRequested)
                {
                    int now = Environment.TickCount;

                    //Did server respond to our last heartbeat?
                    if (_heartbeatTimes.Count != 0 && (now - _lastMessageTime) > intervalMillis &&
                        ConnectionState == ConnectionState.Connected)
                    {
                        _connection.Error(new WebSocketException(WebSocketError.InvalidState, "Server missed last heartbeat"));
                        return;
                    }

                    _heartbeatTimes.Enqueue(now);
                    try
                    {
                        await ApiClient.SendHeartbeatAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await _audioLogger.WarningAsync("Failed to send heartbeat", ex).ConfigureAwait(false);
                    }

                    int delay = Math.Max(0, delayInterval - Latency);
                    await Task.Delay(delay, cancelToken).ConfigureAwait(false);
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
        private async Task RunKeepaliveAsync(CancellationToken cancelToken)
        {
            try
            {
                await _audioLogger.DebugAsync("Keepalive Started").ConfigureAwait(false);
                while (!cancelToken.IsCancellationRequested)
                {
                    int now = Environment.TickCount;

                    try
                    {
                        ulong value = await ApiClient.SendKeepaliveAsync().ConfigureAwait(false);
                        if (_keepaliveTimes.Count < 12) //No reply for 60 Seconds
                            _keepaliveTimes.Enqueue(new KeyValuePair<ulong, int>(value, now));
                    }
                    catch (Exception ex)
                    {
                        await _audioLogger.WarningAsync("Failed to send keepalive", ex).ConfigureAwait(false);
                    }

                    await Task.Delay(KeepAliveIntervalMs, cancelToken).ConfigureAwait(false);
                }
                await _audioLogger.DebugAsync("Keepalive Stopped").ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await _audioLogger.DebugAsync("Keepalive Stopped").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _audioLogger.ErrorAsync("Keepalive Errored", ex).ConfigureAwait(false);
            }
        }

        public async Task SetSpeakingAsync(bool value)
        {
            if (_isSpeaking != value)
            {
                _isSpeaking = value;
                await ApiClient.SendSetSpeaking(value).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Waits until all post-disconnect actions are done.
        /// </summary>
        /// <param name="timeout">Maximum time to wait.</param>
        /// <returns>
        ///     A <see cref="Task"/> that represents an asynchronous process of waiting.
        /// </returns>
        internal async Task WaitForDisconnectAsync(TimeSpan timeout)
        {
            if (ConnectionState == ConnectionState.Disconnected)
                return;

            var completion = new TaskCompletionSource<Exception>();

            var cts = new CancellationTokenSource();

            var _ = Task.Delay(timeout, cts.Token).ContinueWith(_ =>
            {
                completion.TrySetException(new TimeoutException("Exceeded maximum time to wait"));
                cts.Dispose();
            }, cts.Token);

            _connection.Disconnected += HandleDisconnectSubscription;

            await completion.Task.ConfigureAwait(false);

            Task HandleDisconnectSubscription(Exception exception, bool reconnect)
            {
                try
                {
                    cts.Cancel();
                    completion.TrySetResult(exception);
                }
                finally
                {
                    _connection.Disconnected -= HandleDisconnectSubscription;
                    cts.Dispose();
                }

                return Task.CompletedTask;
            }
        }

        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAsync().GetAwaiter().GetResult();
                ApiClient.Dispose();
                _stateLock?.Dispose();
            }
        }
        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        internal enum StopReason
        {
            Unknown = 0,
            Normal,
            Disconnected,
            Moved
        }
    }
}
