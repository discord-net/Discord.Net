using Discord.Audio;
using Discord.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    internal partial class VoiceWebSocket : WebSocket
	{
		private const string EncryptedMode = "xsalsa20_poly1305";
		private const string UnencryptedMode = "plain";

        private readonly int _targetAudioBufferLength;
		private ManualResetEventSlim _connectWaitOnLogin;
		private uint _ssrc;
		private readonly Random _rand = new Random();

		private OpusEncoder _encoder;
		private ConcurrentQueue<byte[]> _sendQueue;
		private ManualResetEventSlim _sendQueueWait, _sendQueueEmptyWait;
		private UdpClient _udp;
		private IPEndPoint _endpoint;
		private bool _isClearing, _isEncrypted;
		private byte[] _secretKey;
		private ushort _sequence;
		private byte[] _encodingBuffer;
		private string _serverId, _userId, _sessionId, _token;

#if USE_THREAD
		private Thread _sendThread;
#endif

		public string CurrentVoiceServerId => _serverId;

		public VoiceWebSocket(DiscordClient client)
			: base(client)
		{
			_connectWaitOnLogin = new ManualResetEventSlim(false);
			_sendQueue = new ConcurrentQueue<byte[]>();
			_sendQueueWait = new ManualResetEventSlim(true);
			_sendQueueEmptyWait = new ManualResetEventSlim(true);
			_encoder = new OpusEncoder(48000, 1, 20, Opus.Application.Audio);
			_encodingBuffer = new byte[4000];
			_targetAudioBufferLength = client.Config.VoiceBufferLength / 20; //20 ms frames
		}

		public void SetServer(string serverId)
		{
			_serverId = serverId;
        }
		public async Task Login(string userId, string sessionId, string token, CancellationToken cancelToken)
		{
			if ((WebSocketState)_state != WebSocketState.Disconnected)
			{
				//Adjust the host and tell the system to reconnect
				await DisconnectInternal(new Exception("Server transfer occurred."), isUnexpected: false);
				return;
			}
			
			_userId = userId;
			_sessionId = sessionId;
			_token = token;

			await Connect().ConfigureAwait(false);
		}
		public async Task Reconnect()
		{
			try
			{
				var cancelToken = ParentCancelToken;
                await Task.Delay(_client.Config.ReconnectDelay, cancelToken).ConfigureAwait(false);
				while (!cancelToken.IsCancellationRequested)
				{
					try
					{
						await Connect().ConfigureAwait(false);
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

		protected override Task[] Run()
		{
			_isClearing = false;
			
			_udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
#if !DNX451
			_udp.AllowNatTraversal(true);
#endif
			
			VoiceCommands.Login msg = new VoiceCommands.Login();
			msg.Payload.ServerId = _serverId;
			msg.Payload.SessionId = _sessionId;
			msg.Payload.Token = _token;
			msg.Payload.UserId = _userId;
			QueueMessage(msg);

#if USE_THREAD
			_sendThread = new Thread(new ThreadStart(() => SendVoiceAsync(_disconnectToken)));
			_sendThread.Start();
#endif
			return new Task[]
			{
				ReceiveVoiceAsync(),
#if !USE_THREAD
				SendVoiceAsync(),
#endif
#if !DNXCORE50
				WatcherAsync()
#endif
			}.Concat(base.Run()).ToArray();
		}
		protected override Task Cleanup()
		{
#if USE_THREAD
			_sendThread.Join();
			_sendThread = null;
#endif

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

		private async Task ReceiveVoiceAsync()
		{
			var cancelToken = _cancelToken;

			await Task.Run(async () =>
			{
				try
				{
					while (!cancelToken.IsCancellationRequested)
					{
#if DNXCORE50
						if (_udp.Available > 0)
						{
#endif
							var result = await _udp.ReceiveAsync().ConfigureAwait(false);
							ProcessUdpMessage(result);
#if DNXCORE50
						}
						else
							await Task.Delay(1).ConfigureAwait(false);
#endif
					}
				}
				catch (OperationCanceledException) { }
				catch (InvalidOperationException) { } //Includes ObjectDisposedException
				catch (Exception ex) { await DisconnectInternal(ex); }
			}).ConfigureAwait(false);
		}

#if USE_THREAD
		private void SendVoiceAsync(CancellationTokenSource cancelSource)
		{
			var cancelToken = cancelSource.Token;
#else
		private Task SendVoiceAsync()
		{
			var cancelToken = _cancelToken;

			return Task.Run(async () =>
			{
#endif

				byte[] packet;
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

					uint timestamp = 0;
					double nextTicks = 0.0;
					double ticksPerMillisecond = Stopwatch.Frequency / 1000.0;
					double ticksPerFrame = ticksPerMillisecond * _encoder.FrameLength;
					double spinLockThreshold = 1.5 * ticksPerMillisecond;
					uint samplesPerFrame = (uint)_encoder.SamplesPerFrame;
					Stopwatch sw = Stopwatch.StartNew();

					byte[] rtpPacket = new byte[_encodingBuffer.Length + 12];
					rtpPacket[0] = 0x80; //Flags;
					rtpPacket[1] = 0x78; //Payload Type
					rtpPacket[8] = (byte)((_ssrc >> 24) & 0xFF);
					rtpPacket[9] = (byte)((_ssrc >> 16) & 0xFF);
					rtpPacket[10] = (byte)((_ssrc >> 8) & 0xFF);
					rtpPacket[11] = (byte)((_ssrc >> 0) & 0xFF);

					while (!cancelToken.IsCancellationRequested)
					{
						double ticksToNextFrame = nextTicks - sw.ElapsedTicks;
						if (ticksToNextFrame <= 0.0)
						{
							while (sw.ElapsedTicks > nextTicks)
							{
								if (!_isClearing)
								{
									if (_sendQueue.TryDequeue(out packet))
									{
										ushort sequence = unchecked(_sequence++);
										rtpPacket[2] = (byte)((sequence >> 8) & 0xFF);
										rtpPacket[3] = (byte)((sequence >> 0) & 0xFF);
										rtpPacket[4] = (byte)((timestamp >> 24) & 0xFF);
										rtpPacket[5] = (byte)((timestamp >> 16) & 0xFF);
										rtpPacket[6] = (byte)((timestamp >> 8) & 0xFF);
										rtpPacket[7] = (byte)((timestamp >> 0) & 0xFF);
										Buffer.BlockCopy(packet, 0, rtpPacket, 12, packet.Length);
#if USE_THREAD
										_udp.Send(rtpPacket, packet.Length + 12);
#else
										await _udp.SendAsync(rtpPacket, packet.Length + 12).ConfigureAwait(false);
#endif
									}
									timestamp = unchecked(timestamp + samplesPerFrame);
									nextTicks += ticksPerFrame;

									//If we have less than our target data buffered, request more
									int count = _sendQueue.Count;
									if (count == 0)
									{
										_sendQueueWait.Set();
										_sendQueueEmptyWait.Set();
									}
									else if (count < _targetAudioBufferLength)
										_sendQueueWait.Set();
								}
							}
						}
						//Dont sleep for 1 millisecond if we need to output audio in the next 1.5
						else if (_sendQueue.Count == 0 || ticksToNextFrame >= spinLockThreshold)
#if USE_THREAD
						Thread.Sleep(1);
#else
							await Task.Delay(1).ConfigureAwait(false);
#endif
					}
				}
				catch (OperationCanceledException) { }
				catch (InvalidOperationException) { } //Includes ObjectDisposedException
#if !USE_THREAD
			});
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
							var payload = (msg.Payload as JToken).ToObject<VoiceEvents.Ready>();
							_heartbeatInterval = payload.HeartbeatInterval;
							_ssrc = payload.SSRC;
							_endpoint = new IPEndPoint((await Dns.GetHostAddressesAsync(Host.Replace("wss://", "")).ConfigureAwait(false)).FirstOrDefault(), payload.Port);
							//_mode = payload.Modes.LastOrDefault();
							_isEncrypted = !payload.Modes.Contains("plain");
                            _udp.Connect(_endpoint);

							_sequence = (ushort)_rand.Next(0, ushort.MaxValue);
							//No thread issue here because SendAsync doesn't start until _isReady is true
							await _udp.SendAsync(new byte[70] {
								(byte)((_ssrc >> 24) & 0xFF),
								(byte)((_ssrc >> 16) & 0xFF),
								(byte)((_ssrc >> 8) & 0xFF),
								(byte)((_ssrc >> 0) & 0xFF),
								0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
								0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
								0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
								0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
								0x0, 0x0, 0x0, 0x0, 0x0, 0x0 }, 70).ConfigureAwait(false);
						}
					}
					break;
				case 4: //SESSION_DESCRIPTION
					{
						var payload = (msg.Payload as JToken).ToObject<VoiceEvents.JoinServer>();
						_secretKey = payload.SecretKey;
						SendIsTalking(true);
						_connectWaitOnLogin.Set();
					}
					break;
				case 5:
					{
						var payload = (msg.Payload as JToken).ToObject<VoiceEvents.IsTalking>();
						RaiseIsSpeaking(payload.UserId, payload.IsSpeaking);
					}
					break;
				default:
					if (_logLevel >= LogMessageSeverity.Warning)
						RaiseOnLog(LogMessageSeverity.Warning, $"Unknown Opcode: {msg.Operation}");
					break;
			}
		}

		private void ProcessUdpMessage(UdpReceiveResult msg)
		{
			if (msg.Buffer.Length > 0 && msg.RemoteEndPoint.Equals(_endpoint))
			{
				byte[] buffer = msg.Buffer;
				int length = msg.Buffer.Length;
				if (_state != (int)WebSocketState.Connected)
				{
					if (length != 70)
					{
						if (_logLevel >= LogMessageSeverity.Warning)
							RaiseOnLog(LogMessageSeverity.Warning, $"Unexpected message length. Expected 70, got {length}.");
						return;
					}

					int port = buffer[68] | buffer[69] << 8;
					string ip = Encoding.ASCII.GetString(buffer, 4, 70 - 6).TrimEnd('\0');

					CompleteConnect();

					var login2 = new VoiceCommands.Login2();
					login2.Payload.Protocol = "udp";
					login2.Payload.SocketData.Address = ip;
					login2.Payload.SocketData.Mode = _isEncrypted ? EncryptedMode : UnencryptedMode;
                    login2.Payload.SocketData.Port = port;
					QueueMessage(login2);
				}
				else
				{
					//Parse RTP Data
					if (length < 12)
					{
						if (_logLevel >= LogMessageSeverity.Warning)
							RaiseOnLog(LogMessageSeverity.Warning, $"Unexpected message length. Expected >= 12, got {length}.");
						return;
					}

					byte flags = buffer[0];
					if (flags != 0x80)
					{
						if (_logLevel >= LogMessageSeverity.Warning)
							RaiseOnLog(LogMessageSeverity.Warning, $"Unexpected Flags: {flags}");
						return;
					}

					byte payloadType = buffer[1];
					if (payloadType != 0x78)
					{
						if (_logLevel >= LogMessageSeverity.Warning)
							RaiseOnLog(LogMessageSeverity.Warning, $"Unexpected Payload Type: {payloadType}");
						return;
					}

					ushort sequenceNumber = (ushort)((buffer[2] << 8) |
													  buffer[3] << 0);
					uint timestamp = (uint)((buffer[4] << 24) |
											(buffer[5] << 16) |
											(buffer[6] << 8) |
											(buffer[7] << 0));
					uint ssrc = (uint)((buffer[8] << 24) |
									   (buffer[9] << 16) |
									   (buffer[10] << 8) |
									   (buffer[11] << 0));

					//Decrypt
					/*if (_mode == "xsalsa20_poly1305")
					{
						if (length < 36) //12 + 24
							throw new Exception($"Unexpected message length. Expected >= 36, got {length}.");

						byte[] nonce = new byte[24]; //16 bytes static, 8 bytes incrementing?
						Buffer.BlockCopy(buffer, 12, nonce, 0, 24);

						byte[] cipherText = new byte[buffer.Length - 36];
						Buffer.BlockCopy(buffer, 36, cipherText, 0, cipherText.Length);
						
						Sodium.SecretBox.Open(cipherText, nonce, _secretKey);
					}
					else //Plain
					{
						byte[] newBuffer = new byte[buffer.Length - 12];
						Buffer.BlockCopy(buffer, 12, newBuffer, 0, newBuffer.Length);
						buffer = newBuffer;
					}*/

					if (_logLevel >= LogMessageSeverity.Debug)
						RaiseOnLog(LogMessageSeverity.Debug, $"Received {buffer.Length - 12} bytes.");
					//TODO: Use Voice Data
				}
			}
		}

		public void SendPCMFrames(byte[] data, int bytes)
		{
			int frameSize = _encoder.FrameSize;
			int frames = bytes / frameSize;
			int expectedBytes = frames * frameSize;
			int lastFrameSize = bytes - expectedBytes;

			//If this only consists of a partial frame and the buffer is too small to pad the end, make a new one
			if (data.Length < frameSize)
			{
				byte[] newData = new byte[frameSize];
				Buffer.BlockCopy(data, 0, newData, 0, bytes);
				data = newData;
			}

			byte[] payload;
			//Opus encoder requires packets be queued in the same order they were generated, so all of this must still be locked.
			lock (_encoder)
			{
				for (int i = 0, pos = 0; i <= frames; i++, pos += frameSize)
				{
					if (i == frames)
					{
						//If there are no partial frames, skip this step
						if (lastFrameSize == 0)
							break;

						//Take the partial frame from the end of the buffer and put it at the start
						Buffer.BlockCopy(data, pos, data, 0, lastFrameSize);
						pos = 0;

						//Wipe the end of the buffer
						for (int j = lastFrameSize; j < frameSize; j++)
							data[j] = 0;

					}

					//Encode the frame
					int encodedLength = _encoder.EncodeFrame(data, pos, _encodingBuffer);

					//TODO: Handle Encryption
					if (_isEncrypted)
					{
					}

					//Copy result to the queue
					payload = new byte[encodedLength];
					Buffer.BlockCopy(_encodingBuffer, 0, payload, 0, encodedLength);

					//Wait until the queue has a spot open
					_sendQueueWait.Wait(_cancelToken);
					_sendQueue.Enqueue(payload);
					if (_sendQueue.Count >= _targetAudioBufferLength)
						_sendQueueWait.Reset();
					_sendQueueEmptyWait.Reset();
				}
			}

			if (_logLevel >= LogMessageSeverity.Debug)
				RaiseOnLog(LogMessageSeverity.Debug, $"Queued {bytes} bytes for voice output.");
		}
		public void ClearPCMFrames()
		{
			_isClearing = true;
			byte[] ignored;
			while (_sendQueue.TryDequeue(out ignored)) { }
			if (_logLevel >= LogMessageSeverity.Debug)
				RaiseOnLog(LogMessageSeverity.Debug, "Cleared the voice buffer.");
			_isClearing = false;
		}

		private void SendIsTalking(bool value)
		{
			var isTalking = new VoiceCommands.IsTalking();
			isTalking.Payload.IsSpeaking = value;
			isTalking.Payload.Delay = 0;
			QueueMessage(isTalking);
		}

		protected override object GetKeepAlive()
		{
			return new VoiceCommands.KeepAlive();
		}

		public void Wait()
		{
			_sendQueueEmptyWait.Wait();
		}
	}
}
