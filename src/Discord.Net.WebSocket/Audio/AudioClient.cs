using Discord.API.Voice;
using Discord.Audio.Streams;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Discord.Audio
{
    //TODO: Add audio reconnecting
    internal partial class AudioClient : IAudioClient
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

        private readonly ILogger _audioLogger;
        private readonly JsonSerializer _serializer;
        private readonly ConnectionManager _connection;
        private readonly SemaphoreSlim _stateLock;
        private readonly ConcurrentQueue<long> _heartbeatTimes;
        private readonly ConcurrentQueue<KeyValuePair<ulong, int>> _keepaliveTimes;
        private readonly ConcurrentDictionary<uint, ulong> _ssrcMap;
        private readonly ConcurrentDictionary<ulong, StreamPair> _streams;

        private Task _heartbeatTask, _keepaliveTask;
        private long _lastMessageTime;
        private string _url, _sessionId, _token;
        private ulong _userId;
        private uint _ssrc;
        private bool _isSpeaking;

        public SocketGuild Guild { get; }
        public DiscordVoiceAPIClient ApiClient { get; private set; }
        public int Latency { get; private set; }
        public int UdpLatency { get; private set; }
        public ulong ChannelId { get; internal set; }
        internal byte[] SecretKey { get; private set; }

        private DiscordSocketClient Discord => Guild.Discord;
        public ConnectionState ConnectionState => _connection.State;

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        internal AudioClient(SocketGuild guild, int clientId, ulong channelId)
        {
            Guild = guild;
            ChannelId = channelId;
            _audioLogger = Discord.LogManager.CreateLogger($"Audio #{clientId}");

            ApiClient = new DiscordVoiceAPIClient(guild.Id, Discord.WebSocketProvider, Discord.UdpSocketProvider);
            ApiClient.SentGatewayMessage += opCode =>
            {
                _audioLogger.LogDebug($"Sent {opCode}");
                return Task.CompletedTask;
            };
            ApiClient.SentDiscovery += () =>
            {
                _audioLogger.LogDebug("Sent Discovery");
                return Task.CompletedTask;
            };
            //ApiClient.SentData += async bytes => await _audioLogger.DebugAsync($"Sent {bytes} Bytes").ConfigureAwait(false);
            ApiClient.ReceivedEvent += ProcessMessageAsync;
            ApiClient.ReceivedPacket += ProcessPacketAsync;

            _stateLock = new SemaphoreSlim(1, 1);
            _connection = new ConnectionManager(_stateLock, _audioLogger, 30000,
                OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
            _connection.Connected += () => _connectedEvent.InvokeAsync();
            _connection.Disconnected += (ex, recon) => _disconnectedEvent.InvokeAsync(ex);
            _heartbeatTimes = new ConcurrentQueue<long>();
            _keepaliveTimes = new ConcurrentQueue<KeyValuePair<ulong, int>>();
            _ssrcMap = new ConcurrentDictionary<uint, ulong>();
            _streams = new ConcurrentDictionary<ulong, StreamPair>();

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _audioLogger.LogWarning(e.ErrorContext.Error, e.ErrorContext.Error.Message);
                e.ErrorContext.Handled = true;
            };

            LatencyUpdated += (old, val) =>
            {
                _audioLogger.LogDebug($"Latency = {val} ms");
                return Task.CompletedTask;
            };
            UdpLatencyUpdated += (old, val) =>
            {
                _audioLogger.LogDebug($"UDP Latency = {val} ms");
                return Task.CompletedTask;
            };
        }

        internal async Task StartAsync(string url, ulong userId, string sessionId, string token)
        {
            _url = url;
            _userId = userId;
            _sessionId = sessionId;
            _token = token;
            await _connection.StartAsync().ConfigureAwait(false);
        }

        public IReadOnlyDictionary<ulong, AudioInStream> GetStreams()
        {
            return _streams.ToDictionary(pair => pair.Key, pair => pair.Value.Reader);
        }

        public async Task StopAsync()
        {
            await _connection.StopAsync().ConfigureAwait(false);
        }

        private async Task OnConnectingAsync()
        {
            _audioLogger.LogDebug("Connecting ApiClient");
            await ApiClient.ConnectAsync("wss://" + _url + "?v=" + DiscordConfig.VoiceAPIVersion).ConfigureAwait(false);
            _audioLogger.LogDebug("Listening on port " + ApiClient.UdpPort);
            _audioLogger.LogDebug("Sending Identity");
            await ApiClient.SendIdentityAsync(_userId, _sessionId, _token).ConfigureAwait(false);

            //Wait for READY
            await _connection.WaitAsync().ConfigureAwait(false);
        }
        private async Task OnDisconnectingAsync(Exception ex)
        {
            _audioLogger.LogDebug("Disconnecting ApiClient");
            await ApiClient.DisconnectAsync().ConfigureAwait(false);

            //Wait for tasks to complete
            _audioLogger.LogDebug("Waiting for heartbeater");
            var heartbeatTask = _heartbeatTask;
            if (heartbeatTask != null)
                await heartbeatTask.ConfigureAwait(false);
            _heartbeatTask = null;
            var keepaliveTask = _keepaliveTask;
            if (keepaliveTask != null)
                await keepaliveTask.ConfigureAwait(false);
            _keepaliveTask = null;

            while (_heartbeatTimes.TryDequeue(out _)) { }
            _lastMessageTime = 0;

            await ClearInputStreamsAsync().ConfigureAwait(false);

            _audioLogger.LogDebug("Sending Voice State");
            await Discord.ApiClient.SendVoiceStateUpdateAsync(Guild.Id, null, false, false).ConfigureAwait(false);
        }

        public AudioOutStream CreateOpusStream(int bufferMillis)
        {
            var outputStream = new OutputStream(ApiClient); //Ignores header
            var sodiumEncrypter = new SodiumEncryptStream( outputStream, this); //Passes header
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
                            _audioLogger.LogDebug("Received Ready");
                            var data = (payload as JToken).ToObject<ReadyEvent>(_serializer);

                            _ssrc = data.SSRC;

                            if (!data.Modes.Contains(DiscordVoiceAPIClient.Mode))
                                throw new InvalidOperationException($"Discord does not support {DiscordVoiceAPIClient.Mode}");

                            ApiClient.SetUdpEndpoint(data.Ip, data.Port);
                            await ApiClient.SendDiscoveryAsync(_ssrc).ConfigureAwait(false);


                            _heartbeatTask = RunHeartbeatAsync(41250, _connection.CancelToken);
                        }
                        break;
                    case VoiceOpCode.SessionDescription:
                        {
                            _audioLogger.LogDebug("Received SessionDescription");
                            var data = (payload as JToken).ToObject<SessionDescriptionEvent>(_serializer);

                            if (data.Mode != DiscordVoiceAPIClient.Mode)
                                throw new InvalidOperationException($"Discord selected an unexpected mode: {data.Mode}");

                            SecretKey = data.SecretKey;
                            _isSpeaking = false;
                            await ApiClient.SendSetSpeaking(false).ConfigureAwait(false);
                            _keepaliveTask = RunKeepaliveAsync(5000, _connection.CancelToken);

                            var _ = _connection.CompleteAsync();
                        }
                        break;
                    case VoiceOpCode.HeartbeatAck:
                        {
                            _audioLogger.LogDebug("Received HeartbeatAck");

                            if (_heartbeatTimes.TryDequeue(out long time))
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
                            _audioLogger.LogDebug("Received Speaking");

                            var data = (payload as JToken).ToObject<SpeakingEvent>(_serializer);
                            _ssrcMap[data.Ssrc] = data.UserId; //TODO: Memory Leak: SSRCs are never cleaned up

                            await _speakingUpdatedEvent.InvokeAsync(data.UserId, data.Speaking);
                        }
                        break;
                    default:
                        _audioLogger.LogWarning($"Unknown OpCode ({opCode})");
                        return;
                }
            }
            catch (Exception ex)
            {
                _audioLogger.LogError($"Error handling {opCode}", ex);
                return;
            }
        }
        private async Task ProcessPacketAsync(byte[] packet)
        {
            try
            {
                if (_connection.State == ConnectionState.Connecting)
                {
                    if (packet.Length != 70)
                    {
                        _audioLogger.LogDebug("Malformed Packet");
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
                        _audioLogger.LogDebug("Malformed Packet", ex);
                        return;
                    }

                    _audioLogger.LogDebug("Received Discovery");
                    await ApiClient.SendSelectProtocol(ip, port).ConfigureAwait(false);
                }
                else if (_connection.State == ConnectionState.Connected)
                {
                    if (packet.Length == 8)
                    {
                        _audioLogger.LogDebug("Received Keepalive");

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
                        if (!RTPReadStream.TryReadSsrc(packet, 0, out var ssrc))
                        {
                            _audioLogger.LogDebug("Malformed Frame");
                            return;
                        }
                        if (!_ssrcMap.TryGetValue(ssrc, out var userId))
                        {
                            _audioLogger.LogDebug($"Unknown SSRC {ssrc}");
                            return;
                        }
                        if (!_streams.TryGetValue(userId, out var pair))
                        {
                            _audioLogger.LogDebug($"Unknown User {userId}");
                            return;
                        }
                        try
                        {
                            await pair.Writer.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _audioLogger.LogDebug(ex, "Malformed Frame");
                            return;
                        }
                        //await _audioLogger.DebugAsync($"Received {packet.Length} bytes from user {userId}").ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _audioLogger.LogWarning("Failed to process UDP packet", ex);
                return;
            }
        }

        private async Task RunHeartbeatAsync(int intervalMillis, CancellationToken cancelToken)
        {
            // TODO: Clean this up when Discord's session patch is live
            try
            {
                _audioLogger.LogDebug("Heartbeat Started");
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
                        _audioLogger.LogWarning("Failed to send heartbeat", ex);
                    }

                    await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);
                }
                _audioLogger.LogDebug("Heartbeat Stopped");
            }
            catch (OperationCanceledException)
            {
                _audioLogger.LogDebug("Heartbeat Stopped");
            }
            catch (Exception ex)
            {
                _audioLogger.LogError("Heartbeat Errored", ex);
            }
        }
        private async Task RunKeepaliveAsync(int intervalMillis, CancellationToken cancelToken)
        {
            try
            {
                _audioLogger.LogDebug("Keepalive Started");
                while (!cancelToken.IsCancellationRequested)
                {
                    var now = Environment.TickCount;

                    try
                    {
                        ulong value = await ApiClient.SendKeepaliveAsync().ConfigureAwait(false);
                        if (_keepaliveTimes.Count < 12) //No reply for 60 Seconds
                            _keepaliveTimes.Enqueue(new KeyValuePair<ulong, int>(value, now));
                    }
                    catch (Exception ex)
                    {
                        _audioLogger.LogWarning("Failed to send keepalive", ex);
                    }

                    await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);
                }
                _audioLogger.LogDebug("Keepalive Stopped");
            }
            catch (OperationCanceledException)
            {
                _audioLogger.LogDebug("Keepalive Stopped");
            }
            catch (Exception ex)
            {
                _audioLogger.LogError("Keepalive Errored", ex);
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
    }
}
