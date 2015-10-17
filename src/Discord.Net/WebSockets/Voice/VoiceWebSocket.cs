#define USE_THREAD
using Discord.Audio;
using Discord.Helpers;
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

namespace Discord.WebSockets.Voice
{
	internal partial class VoiceWebSocket : WebSocket
	{
		private const int MaxOpusSize = 4000;
        private const string EncryptedMode = "xsalsa20_poly1305";
		private const string UnencryptedMode = "plain";

		private readonly Random _rand;
		private readonly int _targetAudioBufferLength;
		private OpusEncoder _encoder;
		private readonly ConcurrentDictionary<uint, OpusDecoder> _decoders;
		private uint _ssrc;
		private ConcurrentDictionary<uint, string> _ssrcMapping;

		private VoiceBuffer _sendBuffer;
		private UdpClient _udp;
		private IPEndPoint _endpoint;
		private bool _isEncrypted;
		private byte[] _secretKey, _encodingBuffer;
		private ushort _sequence;
		private string _serverId, _channelId, _userId, _sessionId, _token, _encryptionMode;

#if USE_THREAD
		private Thread _sendThread, _receiveThread;
#endif

		public string CurrentServerId => _serverId;
		public string CurrentChannelId => _channelId;
		public VoiceBuffer OutputBuffer => _sendBuffer;

		public VoiceWebSocket(DiscordSimpleClient client)
			: base(client)
		{
			_rand = new Random();
			_decoders = new ConcurrentDictionary<uint, OpusDecoder>();
			_targetAudioBufferLength = client.Config.VoiceBufferLength / 20; //20 ms frames
			_encodingBuffer = new byte[MaxOpusSize];
			_ssrcMapping = new ConcurrentDictionary<uint, string>();
			_encoder = new OpusEncoder(48000, 1, 20, Opus.Application.Audio);
			_sendBuffer = new VoiceBuffer((int)Math.Ceiling(client.Config.VoiceBufferLength / (double)_encoder.FrameLength), _encoder.FrameSize);
        }

		public Task SetChannel(string serverId, string channelId)
		{
			_serverId = serverId;
			_channelId = channelId;

			return base.BeginConnect();
        }
		public async Task Login(string userId, string sessionId, string token, CancellationToken cancelToken)
		{
			if ((WebSocketState)_state == WebSocketState.Connected)
			{
				//Adjust the host and tell the system to reconnect
				await DisconnectInternal(new Exception("Server transfer occurred."), isUnexpected: false);
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
                await Task.Delay(_client.Config.ReconnectDelay, cancelToken).ConfigureAwait(false);
				while (!cancelToken.IsCancellationRequested)
				{
					try
					{
						await Start().ConfigureAwait(false);
						break;
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						RaiseOnLog(LogMessageSeverity.Error, $"Reconnect failed: {ex.GetBaseException().Message}");
						//Net is down? We can keep trying to reconnect until the user runs Disconnect()
						await Task.Delay(_client.Config.FailedReconnectDelay, cancelToken).ConfigureAwait(false);
					}
				}
			}
			catch (OperationCanceledException) { }
		}

		protected override IEnumerable<Task> GetTasks()
		{			
			_udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
#if !DNX451 && !__MonoCS__
			_udp.AllowNatTraversal(true);
#endif
			
			LoginCommand msg = new LoginCommand();
			msg.Payload.ServerId = _serverId;
			msg.Payload.SessionId = _sessionId;
			msg.Payload.Token = _token;
			msg.Payload.UserId = _userId;
			QueueMessage(msg);

			List<Task> tasks = new List<Task>();
			if ((_client.Config.VoiceMode & DiscordVoiceMode.Outgoing) != 0)
			{
#if USE_THREAD
				_sendThread = new Thread(new ThreadStart(() => SendVoiceAsync(_cancelToken)));
				_sendThread.IsBackground = true;
                _sendThread.Start();
#else
				tasks.Add(SendVoiceAsync());
#endif
			}

#if USE_THREAD
			//This thread is required to establish a connection even if we're outgoing only
			if ((_client.Config.VoiceMode & DiscordVoiceMode.Incoming) != 0)
			{
				_receiveThread = new Thread(new ThreadStart(() => ReceiveVoiceAsync(_cancelToken)));
				_receiveThread.IsBackground = true;
				_receiveThread.Start();
			}
			else //Dont make an OS thread if we only want to capture one packet...
				tasks.Add(Task.Run(() => ReceiveVoiceAsync(_cancelToken)));
#else
				tasks.Add(ReceiveVoiceAsync());
#endif

#if !DNXCORE50
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
		protected override Task Cleanup()
		{
#if USE_THREAD
			if (_sendThread != null)
				_sendThread.Join();
			if (_receiveThread != null)	
				_receiveThread.Join();
			_sendThread = null;
			_receiveThread = null;
#endif

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

			return base.Cleanup();
		}

#if USE_THREAD
		private void ReceiveVoiceAsync(CancellationToken cancelToken)
		{
#else
		private Task ReceiveVoiceAsync()
		{
			var cancelToken = _cancelToken;

			return Task.Run(async () =>
			{
#endif
			try
			{
				byte[] packet, decodingBuffer = null, nonce = null, result;
				int packetLength, resultOffset, resultLength;
				IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

				if ((_client.Config.VoiceMode & DiscordVoiceMode.Incoming) != 0)
				{
					decodingBuffer = new byte[MaxOpusSize];
					nonce = new byte[24];
				}

				while (!cancelToken.IsCancellationRequested)
				{
#if USE_THREAD
					Thread.Sleep(1);
#elif DNXCORE50
					await Task.Delay(1).ConfigureAwait(false);
#endif
#if USE_THREAD || DNXCORE50
					if (_udp.Available > 0)
					{
						packet = _udp.Receive(ref endpoint);
#else
						var msg = await _udp.ReceiveAsync().ConfigureAwait(false);		
						endpoint = msg.Endpoint;			
						receievedPacket = msg.Buffer;
#endif
						packetLength = packet.Length;

                        if (packetLength > 0 && endpoint.Equals(_endpoint))
						{
							if (_state != (int)WebSocketState.Connected)
							{
								if (packetLength != 70)
									return;

								int port = packet[68] | packet[69] << 8;
								string ip = Encoding.ASCII.GetString(packet, 4, 70 - 6).TrimEnd('\0');

								var login2 = new Login2Command();
								login2.Payload.Protocol = "udp";
								login2.Payload.SocketData.Address = ip;
								login2.Payload.SocketData.Mode = _encryptionMode;
								login2.Payload.SocketData.Port = port;
								QueueMessage(login2);
								if ((_client.Config.VoiceMode & DiscordVoiceMode.Incoming) == 0)
									return;
							}
							else
							{
								//Parse RTP Data
								if (packetLength < 12)
									return;

								byte flags = packet[0];
								if (flags != 0x80)
									return;

								byte payloadType = packet[1];
								if (payloadType != 0x78)
									return;

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

								string userId;
								if (_ssrcMapping.TryGetValue(ssrc, out userId))
									RaiseOnPacket(userId, _channelId, result, resultOffset, resultLength);
							}
						}
#if USE_THREAD || DNXCORE50
					}
#endif
				}
			}
            catch (OperationCanceledException) { }
			catch (InvalidOperationException) { } //Includes ObjectDisposedException
#if !USE_THREAD
			}).ConfigureAwait(false);
#endif
		}

#if USE_THREAD
		private void SendVoiceAsync(CancellationToken cancelToken)
		{
#else
		private Task SendVoiceAsync()
		{
			var cancelToken = _cancelToken;

			return Task.Run(async () =>
			{
#endif

			try
			{
				while (!cancelToken.IsCancellationRequested && _state != (int)WebSocketState.Connected)
				{
#if USE_THREAD
					Thread.Sleep(1);
#else
					await Task.Delay(1);
#endif
				}

				if (cancelToken.IsCancellationRequested)
					return;

				byte[] frame = new byte[_encoder.FrameSize];
				byte[] encodedFrame = new byte[MaxOpusSize];
				byte[] udpPacket, nonce = null;
				uint timestamp = 0;
				double nextTicks = 0.0;
				double ticksPerMillisecond = Stopwatch.Frequency / 1000.0;
				double ticksPerFrame = ticksPerMillisecond * _encoder.FrameLength;
				double spinLockThreshold = 3 * ticksPerMillisecond;
				uint samplesPerFrame = (uint)_encoder.SamplesPerFrame;
				Stopwatch sw = Stopwatch.StartNew();

				if (_isEncrypted)
				{
					nonce = new byte[24];
					udpPacket = new byte[MaxOpusSize + 12 + 16];
				}
				else
					udpPacket = new byte[MaxOpusSize + 12];

				int rtpPacketLength = 0;
				udpPacket[0] = 0x80; //Flags;
				udpPacket[1] = 0x78; //Payload Type
				udpPacket[8] = (byte)((_ssrc >> 24) & 0xFF);
				udpPacket[9] = (byte)((_ssrc >> 16) & 0xFF);
				udpPacket[10] = (byte)((_ssrc >> 8) & 0xFF);
				udpPacket[11] = (byte)((_ssrc >> 0) & 0xFF);

				if  (_isEncrypted)
					Buffer.BlockCopy(udpPacket, 0, nonce, 0, 12);

				while (!cancelToken.IsCancellationRequested)
				{
					double ticksToNextFrame = nextTicks - sw.ElapsedTicks;
					if (ticksToNextFrame <= 0.0)
					{
						while (sw.ElapsedTicks > nextTicks)
						{
							if (_sendBuffer.Pop(frame))
							{
								ushort sequence = unchecked(_sequence++);
								udpPacket[2] = (byte)((sequence >> 8) & 0xFF);
								udpPacket[3] = (byte)((sequence >> 0) & 0xFF);
								udpPacket[4] = (byte)((timestamp >> 24) & 0xFF);
								udpPacket[5] = (byte)((timestamp >> 16) & 0xFF);
								udpPacket[6] = (byte)((timestamp >> 8) & 0xFF);
								udpPacket[7] = (byte)((timestamp >> 0) & 0xFF);

								//Encode
								int encodedLength = _encoder.EncodeFrame(frame, 0, encodedFrame);

								//Encrypt
								if (_isEncrypted)
								{
									Buffer.BlockCopy(udpPacket, 2, nonce, 2, 6); //Update nonce
									int ret = Sodium.Encrypt(encodedFrame, encodedLength, udpPacket, 12, nonce, _secretKey);
									if (ret != 0)
										continue;
									rtpPacketLength = encodedLength + 12 + 16;
								}
								else
								{
									Buffer.BlockCopy(encodedFrame, 0, udpPacket, 12, encodedLength);
									rtpPacketLength = encodedLength + 12;
								}
#if USE_THREAD
								_udp.Send(udpPacket, rtpPacketLength);
#else
								await _udp.SendAsync(rtpPacket, rtpPacketLength).ConfigureAwait(false);
#endif
							}
							timestamp = unchecked(timestamp + samplesPerFrame);
							nextTicks += ticksPerFrame;
						}
					}
					//Dont sleep if we need to output audio in the next spinLockThreshold
					else if (ticksToNextFrame > spinLockThreshold)
					{
						int time = (int)Math.Ceiling((ticksToNextFrame - spinLockThreshold) / ticksPerMillisecond);
#if USE_THREAD
						Thread.Sleep(1);
#else
						await Task.Delay(1).ConfigureAwait(false);
#endif
					}
					//Don't spinlock if we're not actually sending audio (or buffer underrunning)
					/*else if (_sendQueue.Count == 0)
					{
						int time = (int)Math.Ceiling(ticksToNextFrame / ticksPerMillisecond);
#if USE_THREAD
						Thread.Sleep(1);
#else
						await Task.Delay(1).ConfigureAwait(false);
#endif
					}*/
				}
			}
			catch (OperationCanceledException) { }
			catch (InvalidOperationException) { } //Includes ObjectDisposedException
#if !USE_THREAD
			}).ConfigureAwait(false);
#endif
		}
#if !DNXCORE50
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
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			switch (msg.Operation)
			{
				case 2: //READY
					{
						if (_state != (int)WebSocketState.Connected)
						{
							var payload = (msg.Payload as JToken).ToObject<ReadyEvent>();
							_heartbeatInterval = payload.HeartbeatInterval;
							_ssrc = payload.SSRC;
							_endpoint = new IPEndPoint((await Dns.GetHostAddressesAsync(Host.Replace("wss://", "")).ConfigureAwait(false)).FirstOrDefault(), payload.Port);
							
							if (_client.Config.EnableVoiceEncryption)
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

							_sequence = (ushort)_rand.Next(0, ushort.MaxValue);
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
				case 4: //SESSION_DESCRIPTION
					{
						var payload = (msg.Payload as JToken).ToObject<JoinServerEvent>();
						_secretKey = payload.SecretKey;
						SendIsTalking(true);
						EndConnect();
					}
					break;
				case 5:
					{
						var payload = (msg.Payload as JToken).ToObject<IsTalkingEvent>();
						RaiseIsSpeaking(payload.UserId, payload.IsSpeaking);
					}
					break;
				default:
					if (_logLevel >= LogMessageSeverity.Warning)
						RaiseOnLog(LogMessageSeverity.Warning, $"Unknown Opcode: {msg.Operation}");
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

		protected override object GetKeepAlive()
		{
			return new KeepAliveCommand();
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
