using Discord.API;
using Discord.Audio;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
	public partial class VoiceWebSocket : WebSocket
	{
		public enum OpCodes : byte
		{
			Identify = 0,
			SelectProtocol = 1,
			Ready = 2,
			Heartbeat = 3,
			SessionDescription = 4,
			Speaking = 5
		}

		private const int MaxOpusSize = 4000;
        private const string EncryptedMode = "xsalsa20_poly1305";
		private const string UnencryptedMode = "plain";

		//private readonly Random _rand;
		private readonly int _targetAudioBufferLength;
		private readonly ConcurrentDictionary<uint, OpusDecoder> _decoders;
		private readonly AudioServiceConfig _audioConfig;
		private OpusEncoder _encoder;
		private uint _ssrc;
		private ConcurrentDictionary<uint, long> _ssrcMapping;

		private VoiceBuffer _sendBuffer;
		private UdpClient _udp;
		private IPEndPoint _endpoint;
		private bool _isEncrypted;
		private byte[] _secretKey, _encodingBuffer;
		private ushort _sequence;
		private long? _serverId, _channelId, _userId;
		private string _sessionId, _token, _encryptionMode;
		private int _ping;
		
		private Thread _sendThread, _receiveThread;
		
		public long? ServerId { get { return _serverId; } internal set { _serverId = value; } }
		public long? ChannelId { get { return _channelId; } internal set { _channelId = value; } }
		public int Ping => _ping;
		internal VoiceBuffer OutputBuffer => _sendBuffer;

		public VoiceWebSocket(DiscordConfig config, AudioServiceConfig audioConfig, Logger logger)
			: base(config, logger)
		{
			_audioConfig = audioConfig;
			_decoders = new ConcurrentDictionary<uint, OpusDecoder>();
			_targetAudioBufferLength = _audioConfig.BufferLength / 20; //20 ms frames
			_encodingBuffer = new byte[MaxOpusSize];
			_ssrcMapping = new ConcurrentDictionary<uint, long>();
			_encoder = new OpusEncoder(48000, 1, 20, _audioConfig.Bitrate, Opus.Application.Audio);
			_sendBuffer = new VoiceBuffer((int)Math.Ceiling(_audioConfig.BufferLength / (double)_encoder.FrameLength), _encoder.FrameSize);
        }
		
		public async Task Login(long userId, string sessionId, string token, CancellationToken cancelToken)
		{
			if ((WebSocketState)_state == WebSocketState.Connected)
			{
				//Adjust the host and tell the system to reconnect
				await DisconnectInternal(new Exception("Server transfer occurred."), isUnexpected: false).ConfigureAwait(false);
				return;
			}
			
			_userId = userId;
			_sessionId = sessionId;
			_token = token;
			
			await Start().ConfigureAwait(false);
		}
		public async Task Reconnect()
		{
			try
			{
				var cancelToken = ParentCancelToken.Value;
                await Task.Delay(_config.ReconnectDelay, cancelToken).ConfigureAwait(false);
				while (!cancelToken.IsCancellationRequested)
				{
					try
					{
						//This check is needed in case we start a reconnect before the initial login completes
						if (_state != (int)WebSocketState.Disconnected) 
							await Start().ConfigureAwait(false);
						break;
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						_logger.Log(LogSeverity.Error, "Reconnect failed", ex);
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(_config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
					}
				}
			}
			catch (OperationCanceledException) { }
		}

		protected override IEnumerable<Task> GetTasks()
		{			
			_udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

			VoiceLoginCommand msg = new VoiceLoginCommand();
			msg.Payload.ServerId = _serverId.Value;
			msg.Payload.SessionId = _sessionId;
			msg.Payload.Token = _token;
			msg.Payload.UserId = _userId.Value;
			QueueMessage(msg);

			List<Task> tasks = new List<Task>();
			if ((_audioConfig.Mode & AudioMode.Outgoing) != 0)
			{
				_sendThread = new Thread(new ThreadStart(() => SendVoiceAsync(_cancelToken)));
				_sendThread.IsBackground = true;
                _sendThread.Start();
			}
			
			//This thread is required to establish a connection even if we're outgoing only
			if ((_audioConfig.Mode & AudioMode.Incoming) != 0)
			{
				_receiveThread = new Thread(new ThreadStart(() => ReceiveVoiceAsync(_cancelToken)));
				_receiveThread.IsBackground = true;
				_receiveThread.Start();
			}
			else //Dont make an OS thread if we only want to capture one packet...
				tasks.Add(Task.Run(() => ReceiveVoiceAsync(_cancelToken)));

#if !DOTNET5_4
			tasks.Add(WatcherAsync());
#endif
			if (tasks.Count > 0)
			{
				// We need to combine tasks into one because receiveThread is 
				// supposed to exit early if it's an outgoing-only client
				// and we dont want the main thread to think we errored
				var task = Task.WhenAll(tasks);
				tasks.Clear();
				tasks.Add(task);
			}
			tasks.AddRange(base.GetTasks());
			
			return new Task[] { Task.WhenAll(tasks.ToArray()) };
		}
		protected override Task Stop()
		{
			if (_sendThread != null)
				_sendThread.Join();
			if (_receiveThread != null)	
				_receiveThread.Join();
			_sendThread = null;
			_receiveThread = null;

			OpusDecoder decoder;
			foreach (var pair in _decoders)
			{
				if (_decoders.TryRemove(pair.Key, out decoder))
					decoder.Dispose();
            }

			ClearPCMFrames();
			if (!_wasDisconnectUnexpected)
			{
				_userId = null;
				_sessionId = null;
				_token = null;
			}
			_udp = null;

			return base.Stop();
		}
		
		private void ReceiveVoiceAsync(CancellationToken cancelToken)
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
					Thread.Sleep(1);
					if (_udp.Available > 0)
					{
#if !DOTNET5_4
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
							if (_state != (int)WebSocketState.Connected)
							{
								if (packetLength != 70)
									return;

								int port = packet[68] | packet[69] << 8;
								string ip = Encoding.UTF8.GetString(packet, 4, 70 - 6).TrimEnd('\0');

								var login2 = new VoiceLogin2Command();
								login2.Payload.Protocol = "udp";
								login2.Payload.SocketData.Address = ip;
								login2.Payload.SocketData.Mode = _encryptionMode;
								login2.Payload.SocketData.Port = port;
								QueueMessage(login2);
								if ((_audioConfig.Mode & AudioMode.Incoming) == 0)
									return;
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
									int ret = Sodium.Decrypt(packet, 12, packetLength - 12, decodingBuffer, nonce, _secretKey);
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

								long userId;
								if (_ssrcMapping.TryGetValue(ssrc, out userId))
									RaiseOnPacket(userId, _channelId.Value, result, resultOffset, resultLength);
							}
						}
					}
				}
			}
            catch (OperationCanceledException) { }
			catch (InvalidOperationException) { } //Includes ObjectDisposedException
		}
		
		private void SendVoiceAsync(CancellationToken cancelToken)
		{
			try
			{
				while (!cancelToken.IsCancellationRequested && _state != (int)WebSocketState.Connected)
					Thread.Sleep(1);

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
				pingPacket[0] = 0x80; //Flags;
				pingPacket[1] = 0xC9; //Payload Type
				pingPacket[2] = 0x00; //Length
				pingPacket[3] = 0x01; //Length (1*8 bytes)
				pingPacket[4] = (byte)((_ssrc >> 24) & 0xFF);
				pingPacket[5] = (byte)((_ssrc >> 16) & 0xFF);
				pingPacket[6] = (byte)((_ssrc >> 8) & 0xFF);
				pingPacket[7] = (byte)((_ssrc >> 0) & 0xFF);
				if (_isEncrypted)
				{
					Buffer.BlockCopy(pingPacket, 0, nonce, 0, 8);
					int ret = Sodium.Encrypt(pingPacket, 8, encodedFrame, 0, nonce, _secretKey);
					if (ret != 0)
						throw new InvalidOperationException("Failed to encrypt ping packet");
					pingPacket = new byte[pingPacket.Length + 16];
					Buffer.BlockCopy(encodedFrame, 0, pingPacket, 0, pingPacket.Length);
					Array.Clear(nonce, 0, nonce.Length);
                }

				int rtpPacketLength = 0;
				voicePacket[0] = 0x80; //Flags;
				voicePacket[1] = 0x78; //Payload Type
				voicePacket[8] = (byte)((_ssrc >> 24) & 0xFF);
				voicePacket[9] = (byte)((_ssrc >> 16) & 0xFF);
				voicePacket[10] = (byte)((_ssrc >> 8) & 0xFF);
				voicePacket[11] = (byte)((_ssrc >> 0) & 0xFF);

				if  (_isEncrypted)
					Buffer.BlockCopy(voicePacket, 0, nonce, 0, 12);

				bool hasFrame = false;
				while (!cancelToken.IsCancellationRequested)
				{
					if (!hasFrame && _sendBuffer.Pop(frame))
					{
						ushort sequence = unchecked(_sequence++);
						voicePacket[2] = (byte)((sequence >> 8) & 0xFF);
						voicePacket[3] = (byte)((sequence >> 0) & 0xFF);
						voicePacket[4] = (byte)((timestamp >> 24) & 0xFF);
						voicePacket[5] = (byte)((timestamp >> 16) & 0xFF);
						voicePacket[6] = (byte)((timestamp >> 8) & 0xFF);
						voicePacket[7] = (byte)((timestamp >> 0) & 0xFF);

						//Encode
						int encodedLength = _encoder.EncodeFrame(frame, 0, encodedFrame);

						//Encrypt
						if (_isEncrypted)
						{
							Buffer.BlockCopy(voicePacket, 2, nonce, 2, 6); //Update nonce
							int ret = Sodium.Encrypt(encodedFrame, encodedLength, voicePacket, 12, nonce, _secretKey);
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
								_udp.Send(voicePacket, rtpPacketLength);
							}
							catch (SocketException ex)
							{
								_logger.Log(LogSeverity.Error, "Failed to send UDP packet.", ex);
							}
							hasFrame = false;
						}
						nextTicks += ticksPerFrame;

						//Is it time to send out another ping?
						if (currentTicks > nextPingTicks)
						{
							_udp.Send(pingPacket, pingPacket.Length);
							nextPingTicks = currentTicks + 5 * ticksPerSeconds;
                        }
                    }
					else
					{
						if (hasFrame)
						{
							int time = (int)Math.Floor(ticksToNextFrame / ticksPerMillisecond);
							if (time > 0)
								Thread.Sleep(time);
						}
						else
							Thread.Sleep(1); //Give as much time to the encrypter as possible
					}
				}
			}
			catch (OperationCanceledException) { }
			catch (InvalidOperationException) { } //Includes ObjectDisposedException
		}
#if !DOTNET5_4
		//Closes the UDP socket when _disconnectToken is triggered, since UDPClient doesn't allow passing a canceltoken
		private Task WatcherAsync()
		{
			var cancelToken = _cancelToken;
			return cancelToken.Wait()
				.ContinueWith(_ => _udp.Close());
		}
#endif

		protected override async Task ProcessMessage(string json)
		{
			await base.ProcessMessage(json).ConfigureAwait(false);
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			var opCode = (OpCodes)msg.Operation;
            switch (opCode)
			{
				case OpCodes.Ready:
					{
						if (_state != (int)WebSocketState.Connected)
						{
							var payload = (msg.Payload as JToken).ToObject<VoiceReadyEvent>(_serializer);
							_heartbeatInterval = payload.HeartbeatInterval;
							_ssrc = payload.SSRC;
							_endpoint = new IPEndPoint((await Dns.GetHostAddressesAsync(Host.Replace("wss://", "")).ConfigureAwait(false)).FirstOrDefault(), payload.Port);
							
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
                            _udp.Connect(_endpoint);

							_sequence = 0;// (ushort)_rand.Next(0, ushort.MaxValue);
							//No thread issue here because SendAsync doesn't start until _isReady is true
							byte[] packet = new byte[70];
							packet[0] = (byte)((_ssrc >> 24) & 0xFF);
							packet[1] = (byte)((_ssrc >> 16) & 0xFF);
							packet[2] = (byte)((_ssrc >> 8) & 0xFF);
							packet[3] = (byte)((_ssrc >> 0) & 0xFF);
							await _udp.SendAsync(packet, 70).ConfigureAwait(false);
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
						var payload = (msg.Payload as JToken).ToObject<JoinServerEvent>(_serializer);
						_secretKey = payload.SecretKey;
						SendIsTalking(true);
						EndConnect();
					}
					break;
				case OpCodes.Speaking:
					{
						var payload = (msg.Payload as JToken).ToObject<IsTalkingEvent>(_serializer);
						RaiseIsSpeaking(payload.UserId, payload.IsSpeaking);
					}
					break;
				default:
					if (_logger.Level >= LogSeverity.Warning)
						_logger.Log(LogSeverity.Warning, $"Unknown Opcode: {opCode}");
					break;
			}
		}

		public void SendPCMFrames(byte[] data, int bytes)
		{
			_sendBuffer.Push(data, bytes, _cancelToken);
		}
		public void ClearPCMFrames()
		{
			_sendBuffer.Clear(_cancelToken);
		}

		private void SendIsTalking(bool value)
		{
			var isTalking = new IsTalkingCommand();
			isTalking.Payload.IsSpeaking = value;
			isTalking.Payload.Delay = 0;
			QueueMessage(isTalking);
		}

		public override void SendHeartbeat()
		{
			QueueMessage(new VoiceKeepAliveCommand());
		}

		public void WaitForQueue()
		{
			_sendBuffer.Wait(_cancelToken);
		}
		public Task WaitForConnection(int timeout)
		{
			return Task.Run(() =>
			{
				try
				{
					if (!_connectedEvent.Wait(timeout, _cancelToken))
						throw new TimeoutException();
				}
				catch (OperationCanceledException)
				{
					ThrowError();
				}
			});
		}
	}
}