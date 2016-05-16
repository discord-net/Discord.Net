using Discord.API.Client;
using Discord.API.Client.VoiceSocket;
using Discord.Audio;
using Discord.Audio.Opus;
using Discord.Audio.Sodium;
using Discord.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public partial class VoiceSocket : WebSocket
    {
        private const int MaxOpusSize = 4000;
        private const string EncryptedMode = "xsalsa20_poly1305";
        private const string UnencryptedMode = "plain";

        private readonly int _targetAudioBufferLength;
        private readonly ConcurrentDictionary<uint, OpusDecoder> _decoders;
        private readonly AudioServiceConfig _audioConfig;
        private Task _sendTask, _receiveTask;
        private VoiceBuffer _sendBuffer;
        private OpusEncoder _encoder;
        private uint _ssrc;
        private ConcurrentDictionary<uint, ulong> _ssrcMapping;
        private UdpClient _udp;
        private IPEndPoint _endpoint;
        private bool _isEncrypted;
        private byte[] _secretKey, _encodingBuffer;
        private ushort _sequence;
        private string _encryptionMode;
        private int _ping;
        private ulong? _userId;
        private string _sessionId;

        public string Token { get; internal set; }
        public Server Server { get; internal set; }
        public Channel Channel { get; internal set; }

        public int Ping => _ping;
        internal VoiceBuffer OutputBuffer => _sendBuffer;

        internal event EventHandler<InternalIsSpeakingEventArgs> UserIsSpeaking = delegate { };
        internal event EventHandler<InternalFrameEventArgs> FrameReceived = delegate { };

        private void OnUserIsSpeaking(ulong userId, bool isSpeaking)
            => UserIsSpeaking(this, new InternalIsSpeakingEventArgs(userId, isSpeaking));
        internal void OnFrameReceived(ulong userId, ulong channelId, byte[] buffer, int offset, int count)
            => FrameReceived(this, new InternalFrameEventArgs(userId, channelId, buffer, offset, count));

        internal VoiceSocket(DiscordConfig config, AudioServiceConfig audioConfig, JsonSerializer serializer, Logger logger)
            : base(config, serializer, logger)
        {
            _audioConfig = audioConfig;
            _decoders = new ConcurrentDictionary<uint, OpusDecoder>();
            _targetAudioBufferLength = _audioConfig.BufferLength / 20; //20 ms frames
            _encodingBuffer = new byte[MaxOpusSize];
            _ssrcMapping = new ConcurrentDictionary<uint, ulong>();
            _encoder = new OpusEncoder(48000, _audioConfig.Channels, 20, _audioConfig.Bitrate, OpusApplication.MusicOrMixed);
            _sendBuffer = new VoiceBuffer((int)Math.Ceiling(_audioConfig.BufferLength / (double)_encoder.FrameLength), _encoder.FrameSize);
        }

        public Task Connect(string host, string token, ulong userId, string sessionId, CancellationToken parentCancelToken)
        {
            Host = host;
            Token = token;
            _userId = userId;
            _sessionId = sessionId;
            return BeginConnect(parentCancelToken);
        }
        private async Task Reconnect()
        {
            try
            {
                var cancelToken = _parentCancelToken;
                await Task.Delay(_config.ReconnectDelay, cancelToken).ConfigureAwait(false);
                while (!cancelToken.IsCancellationRequested)
                {
                    try
                    {
                        await BeginConnect(_parentCancelToken).ConfigureAwait(false);
                        break;
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        Logger.Error("Reconnect failed", ex);
                        //Net is down? We can keep trying to reconnect until the user runs Disconnect()
                        await Task.Delay(_config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }
        public async Task Disconnect()
        {
            await _taskManager.Stop(true).ConfigureAwait(false);
            _userId = null;
        }

        protected override async Task Run()
        {
            _udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            List<Task> tasks = new List<Task>();
            if (_audioConfig.Mode.HasFlag(AudioMode.Outgoing))
                _sendTask = Task.Run(() => SendVoiceAsync(CancelToken));
            _receiveTask = Task.Run(() => ReceiveVoiceAsync(CancelToken));

            SendIdentify(_userId.Value, _sessionId);

#if !NETSTANDARD1_3
            tasks.Add(WatcherAsync());
#endif
            tasks.AddRange(_engine.GetTasks(CancelToken));
            tasks.Add(HeartbeatAsync(CancelToken));
            await _taskManager.Start(tasks, _cancelSource).ConfigureAwait(false);
        }
        protected override async Task Cleanup()
        {
            var sendThread = _sendTask;
            if (sendThread != null)
            {
                try { await sendThread.ConfigureAwait(false); }
                catch (Exception) { } //Ignore any errors during cleanup
            }
            _sendTask = null;

            var receiveThread = _receiveTask;
            if (receiveThread != null)
            {
                try { await receiveThread.ConfigureAwait(false); }
                catch (Exception) { } //Ignore any errors during cleanup
            }
            _receiveTask = null;

            OpusDecoder decoder;
            foreach (var pair in _decoders)
            {
                if (_decoders.TryRemove(pair.Key, out decoder))
                    decoder.Dispose();
            }

            ClearPCMFrames();
            _udp = null;

            await base.Cleanup().ConfigureAwait(false);
        }

        private async Task ReceiveVoiceAsync(CancellationToken cancelToken)
        {
            var closeTask = cancelToken.Wait();
            try
            {
                byte[] packet, decodingBuffer = null, nonce = null, result;
                int packetLength, resultOffset, resultLength;
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

                if ((_audioConfig.Mode & AudioMode.Incoming) != 0)
                {
                    decodingBuffer = new byte[MaxOpusSize];
                    nonce = new byte[24];
                }

                while (!cancelToken.IsCancellationRequested)
                {
                    await Task.Delay(1).ConfigureAwait(false);
                    if (_udp.Available > 0)
                    {
#if !NETSTANDARD1_3
                        packet = _udp.Receive(ref endpoint);
#else
						//TODO: Is this really the only way to end a Receive call in DOTNET5_4?
						var receiveTask = _udp.ReceiveAsync();
                        var task = Task.WhenAny(closeTask, receiveTask).Result;
						if (task == closeTask)
							break;
						var udpPacket = receiveTask.Result;
                        packet = udpPacket.Buffer;
						endpoint = udpPacket.RemoteEndPoint;
#endif
                        packetLength = packet.Length;

                        if (packetLength > 0 && endpoint.Equals(_endpoint))
                        {
                            if (State != ConnectionState.Connected)
                            {
                                if (packetLength != 70)
                                    return;

                                string ip = Encoding.UTF8.GetString(packet, 4, 70 - 6).TrimEnd('\0');
                                int port = packet[68] | packet[69] << 8;

                                SendSelectProtocol(ip, port);
                                if ((_audioConfig.Mode & AudioMode.Incoming) == 0)
                                    return; //We dont need this thread anymore
                            }
                            else
                            {
                                //Parse RTP Data
                                if (packetLength < 12) return;
                                if (packet[0] != 0x80) return; //Flags
                                if (packet[1] != 0x78) return; //Payload Type

                                ushort sequenceNumber = (ushort)((packet[2] << 8) |
                                                                  packet[3] << 0);
                                uint timestamp = (uint)((packet[4] << 24) |
                                                        (packet[5] << 16) |
                                                        (packet[6] << 8) |
                                                        (packet[7] << 0));
                                uint ssrc = (uint)((packet[8] << 24) |
                                                   (packet[9] << 16) |
                                                   (packet[10] << 8) |
                                                   (packet[11] << 0));

                                //Decrypt
                                if (_isEncrypted)
                                {
                                    if (packetLength < 28) //12 + 16 (RTP + Poly1305 MAC)
                                        return;

                                    Buffer.BlockCopy(packet, 0, nonce, 0, 12);
                                    int ret = SecretBox.Decrypt(packet, 12, packetLength - 12, decodingBuffer, nonce, _secretKey);
                                    if (ret != 0)
                                        continue;
                                    result = decodingBuffer;
                                    resultOffset = 0;
                                    resultLength = packetLength - 28;
                                }
                                else //Plain
                                {
                                    result = packet;
                                    resultOffset = 12;
                                    resultLength = packetLength - 12;
                                }

                                /*if (_logLevel >= LogMessageSeverity.Debug)
									RaiseOnLog(LogMessageSeverity.Debug, $"Received {buffer.Length - 12} bytes.");*/

                                ulong userId;
                                if (_ssrcMapping.TryGetValue(ssrc, out userId))
                                    OnFrameReceived(userId, Channel.Id, result, resultOffset, resultLength);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (InvalidOperationException) { } //Includes ObjectDisposedException
        }

        private async Task SendVoiceAsync(CancellationToken cancelToken)
        {
            try
            {
                while (!cancelToken.IsCancellationRequested && State != ConnectionState.Connected)
                    await Task.Delay(1).ConfigureAwait(false);

                if (cancelToken.IsCancellationRequested)
                    return;

                byte[] frame = new byte[_encoder.FrameSize];
                byte[] encodedFrame = new byte[MaxOpusSize];
                byte[] voicePacket, pingPacket, nonce = null;
                uint timestamp = 0;
                double nextTicks = 0.0, nextPingTicks = 0.0;
                long ticksPerSeconds = Stopwatch.Frequency;
                double ticksPerMillisecond = Stopwatch.Frequency / 1000.0;
                double ticksPerFrame = ticksPerMillisecond * _encoder.FrameLength;
                double spinLockThreshold = 3 * ticksPerMillisecond;
                uint samplesPerFrame = (uint)_encoder.SamplesPerFrame;
                Stopwatch sw = Stopwatch.StartNew();

                if (_isEncrypted)
                {
                    nonce = new byte[24];
                    voicePacket = new byte[MaxOpusSize + 12 + 16];
                }
                else
                    voicePacket = new byte[MaxOpusSize + 12];

                pingPacket = new byte[8];

                int rtpPacketLength = 0;
                voicePacket[0] = 0x80; //Flags;
                voicePacket[1] = 0x78; //Payload Type
                voicePacket[8] = (byte)(_ssrc >> 24);
                voicePacket[9] = (byte)(_ssrc >> 16);
                voicePacket[10] = (byte)(_ssrc >> 8);
                voicePacket[11] = (byte)(_ssrc >> 0);

                if (_isEncrypted)
                    Buffer.BlockCopy(voicePacket, 0, nonce, 0, 12);

                bool hasFrame = false;
                while (!cancelToken.IsCancellationRequested)
                {
                    if (!hasFrame && _sendBuffer.Pop(frame))
                    {
                        ushort sequence = unchecked(_sequence++);
                        voicePacket[2] = (byte)(sequence >> 8);
                        voicePacket[3] = (byte)(sequence >> 0);
                        voicePacket[4] = (byte)(timestamp >> 24);
                        voicePacket[5] = (byte)(timestamp >> 16);
                        voicePacket[6] = (byte)(timestamp >> 8);
                        voicePacket[7] = (byte)(timestamp >> 0);

                        //Encode
                        int encodedLength = _encoder.EncodeFrame(frame, 0, encodedFrame);

                        //Encrypt
                        if (_isEncrypted)
                        {
                            Buffer.BlockCopy(voicePacket, 2, nonce, 2, 6); //Update nonce
                            int ret = SecretBox.Encrypt(encodedFrame, encodedLength, voicePacket, 12, nonce, _secretKey);
                            if (ret != 0)
                                continue;
                            rtpPacketLength = encodedLength + 12 + 16;
                        }
                        else
                        {
                            Buffer.BlockCopy(encodedFrame, 0, voicePacket, 12, encodedLength);
                            rtpPacketLength = encodedLength + 12;
                        }

                        timestamp = unchecked(timestamp + samplesPerFrame);
                        hasFrame = true;
                    }

                    long currentTicks = sw.ElapsedTicks;
                    double ticksToNextFrame = nextTicks - currentTicks;
                    if (ticksToNextFrame <= 0.0)
                    {
                        if (hasFrame)
                        {
                            try
                            {
                                await _udp.SendAsync(voicePacket, rtpPacketLength, _endpoint).ConfigureAwait(false);
                            }
                            catch (SocketException ex)
                            {
                                Logger.Error("Failed to send UDP packet.", ex);
                            }
                            hasFrame = false;
                        }
                        nextTicks += ticksPerFrame;

                        //Is it time to send out another ping?
                        if (currentTicks > nextPingTicks)
                        {
                            //Increment in LE
                            for (int i = 0; i < 8; i++)
                            {
                                var b = pingPacket[i];
                                if (b == byte.MaxValue)
                                    pingPacket[i] = 0;
                                else
                                {
                                    pingPacket[i] = (byte)(b + 1);
                                    break;
                                }
                            }
                            await _udp.SendAsync(pingPacket, pingPacket.Length, _endpoint).ConfigureAwait(false);
                            nextPingTicks = currentTicks + 5 * ticksPerSeconds;
                        }
                    }
                    else
                    {
                        if (hasFrame)
                        {
                            int time = (int)Math.Floor(ticksToNextFrame / ticksPerMillisecond);
                            if (time > 0)
                                await Task.Delay(time).ConfigureAwait(false);
                        }
                        else
                            await Task.Delay(1).ConfigureAwait(false); //Give as much time to the encrypter as possible
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (InvalidOperationException) { } //Includes ObjectDisposedException
        }
#if !NETSTANDARD1_3
        //Closes the UDP socket when _disconnectToken is triggered, since UDPClient doesn't allow passing a canceltoken
        private async Task WatcherAsync()
        {
            await CancelToken.Wait().ConfigureAwait(false);
            _udp.Close();
        }
#endif

        protected override async Task ProcessMessage(string json)
        {
            await base.ProcessMessage(json).ConfigureAwait(false);

            WebSocketMessage msg;
            using (var reader = new JsonTextReader(new StringReader(json)))
                msg = _serializer.Deserialize(reader, typeof(WebSocketMessage)) as WebSocketMessage;

            var opCode = (OpCodes)msg.Operation;
            switch (opCode)
            {
                case OpCodes.Ready:
                    {
                        if (State != ConnectionState.Connected)
                        {
                            var payload = (msg.Payload as JToken).ToObject<ReadyEvent>(_serializer);
                            _heartbeatInterval = payload.HeartbeatInterval;
                            _ssrc = payload.SSRC;
                            string hostname = Host.Replace("wss://", "");
                            var address = (await Dns.GetHostAddressesAsync(hostname).ConfigureAwait(false)).FirstOrDefault();
                            _endpoint = new IPEndPoint(address, payload.Port);

                            if (_audioConfig.EnableEncryption)
                            {
                                if (payload.Modes.Contains(EncryptedMode))
                                {
                                    _encryptionMode = EncryptedMode;
                                    _isEncrypted = true;
                                }
                                else
                                    throw new InvalidOperationException("Unexpected encryption format.");
                            }
                            else
                            {
                                _encryptionMode = UnencryptedMode;
                                _isEncrypted = false;
                            }

                            _sequence = 0;// (ushort)_rand.Next(0, ushort.MaxValue);
                                          //No thread issue here because SendAsync doesn't start until _isReady is true
                            byte[] packet = new byte[70];
                            packet[0] = (byte)(_ssrc >> 24);
                            packet[1] = (byte)(_ssrc >> 16);
                            packet[2] = (byte)(_ssrc >> 8);
                            packet[3] = (byte)(_ssrc >> 0);
                            await _udp.SendAsync(packet, 70, _endpoint).ConfigureAwait(false);
                        }
                    }
                    break;
                case OpCodes.Heartbeat:
                    {
                        long time = EpochTime.GetMilliseconds();
                        var payload = (long)msg.Payload;
                        _ping = (int)(payload - time);
                        //TODO: Use this to estimate latency
                    }
                    break;
                case OpCodes.SessionDescription:
                    {
                        var payload = (msg.Payload as JToken).ToObject<SessionDescriptionEvent>(_serializer);
                        _secretKey = payload.SecretKey;
                        SendSetSpeaking(true);
                        await EndConnect().ConfigureAwait(false);
                    }
                    break;
                case OpCodes.Speaking:
                    {
                        var payload = (msg.Payload as JToken).ToObject<SpeakingEvent>(_serializer);
                        OnUserIsSpeaking(payload.UserId, payload.IsSpeaking);
                    }
                    break;
                default:
                    Logger.Warning($"Unknown Opcode: {opCode}");
                    break;
            }
        }

        public void SendPCMFrames(byte[] data, int offset, int count)
        {
            _sendBuffer.Push(data, offset, count, CancelToken);
        }
        public void ClearPCMFrames()
        {
            _sendBuffer.Clear(CancelToken);
        }

        public void WaitForQueue()
        {
            _sendBuffer.Wait(CancelToken);
        }

        public override void SendHeartbeat()
            => QueueMessage(new HeartbeatCommand());
        public void SendIdentify(ulong id, string sessionId)
            => QueueMessage(new IdentifyCommand
            {
                GuildId = Server.Id,
                UserId = id,
                SessionId = sessionId,
                Token = Token
            });
        public void SendSelectProtocol(string externalAddress, int externalPort)
            => QueueMessage(new SelectProtocolCommand
            {
                Protocol = "udp",
                ExternalAddress = externalAddress,
                ExternalPort = externalPort,
                EncryptionMode = _encryptionMode
            });
        public void SendSetSpeaking(bool value)
            => QueueMessage(new SetSpeakingCommand { IsSpeaking = value, Delay = 0 });

    }
}