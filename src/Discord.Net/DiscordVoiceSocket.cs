//#define USE_THREAD
#if !DNXCORE50
using Discord.API.Models;
using Discord.Opus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using WebSocketMessage = Discord.API.Models.VoiceWebSocketCommands.WebSocketMessage;

namespace Discord
{
	internal sealed partial class DiscordVoiceSocket : DiscordWebSocket
	{
		private readonly int _targetAudioBufferLength;
        private ManualResetEventSlim _connectWaitOnLogin;
		private uint _ssrc;
        private readonly Random _rand = new Random();
		
		private OpusEncoder _encoder;
		private ConcurrentQueue<byte[]> _sendQueue;
		private ManualResetEventSlim _sendQueueWait;
		private UdpClient _udp;
		private IPEndPoint _endpoint;
		private bool _isReady, _isClearing;
		private byte[] _secretKey;
		private string _myIp;
		private ushort _sequence;
		private string _mode;
		private byte[] _encodingBuffer;
#if USE_THREAD
		private Thread _sendThread;
#endif

		public DiscordVoiceSocket(DiscordClient client, int timeout, int interval, int audioBufferLength, bool isDebug)
			: base(client, timeout, interval, isDebug)
		{
			_connectWaitOnLogin = new ManualResetEventSlim(false);
			_sendQueue = new ConcurrentQueue<byte[]>();
			_sendQueueWait = new ManualResetEventSlim(true);
			_encoder = new OpusEncoder(48000, 1, 20, Application.Audio);
			_encodingBuffer = new byte[4000];
			_targetAudioBufferLength = audioBufferLength / 20;
        }
		
		protected override void OnConnect()
		{
			_udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
#if !DNX451 && !MONO
			_udp.AllowNatTraversal(true);
#endif
			_isReady = false;
			_isClearing = false;
        }
		protected override void OnDisconnect()
		{
			_udp = null;
#if USE_THREAD
			_sendThread.Join();
			_sendThread = null;
#endif
        }

		protected override Task[] CreateTasks()
		{
#if USE_THREAD
			_sendThread = new Thread(new ThreadStart(() => SendAsync(_disconnectToken)));
			_sendThread.Start();
#endif
			return new Task[]
			{
				ReceiveVoiceAsync(),
#if !USE_THREAD
				SendVoiceAsync(),
#endif
				WatcherAsync()
			}.Concat(base.CreateTasks()).ToArray();
		}

		public async Task Login(string serverId, string userId, string sessionId, string token)
		{
			var cancelToken = _disconnectToken.Token;

			_connectWaitOnLogin.Reset();

			VoiceWebSocketCommands.Login msg = new VoiceWebSocketCommands.Login();
			msg.Payload.ServerId = serverId;
			msg.Payload.SessionId = sessionId;
			msg.Payload.Token = token;
			msg.Payload.UserId = userId;
			await SendMessage(msg, cancelToken);

			try
			{
				if (!_connectWaitOnLogin.Wait(_timeout, cancelToken)) //Waiting on JoinServer message
					throw new Exception("No reply from Discord server");
			}
			catch (OperationCanceledException)
			{
				if (_disconnectReason == null)
					throw new Exception("An unknown websocket error occurred.");
				else
					_disconnectReason.Throw();
			}

			SetConnected();
		}
		
		private async Task ReceiveVoiceAsync()
		{
			var cancelSource = _disconnectToken;
			var cancelToken = cancelSource.Token;
			await Task.Yield();

			try
			{
				while (!cancelToken.IsCancellationRequested)
				{
					var result = await _udp.ReceiveAsync();
					ProcessUdpMessage(result);
				}
			}
			catch (OperationCanceledException) { }
			catch (ObjectDisposedException) { }
			catch (Exception ex) { DisconnectInternal(ex); }
		}

#if USE_THREAD
		private void SendVoiceAsync(CancellationTokenSource cancelSource)
		{
			var cancelToken = cancelSource.Token;
#else
		private async Task SendVoiceAsync()
		{
			var cancelSource = _disconnectToken;
			var cancelToken = cancelSource.Token;
			await Task.Yield();

			byte[] packet;
			try
			{
				while (!cancelToken.IsCancellationRequested && !_isReady)
					Thread.Sleep(1);

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
					//If we have less than our target data buffered, request more
					if (_sendQueue.Count < _targetAudioBufferLength)
						_sendQueueWait.Set();

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
									_udp.Send(rtpPacket, packet.Count + 12);
#else
									await _udp.SendAsync(rtpPacket, packet.Length + 12);
#endif
								}
								timestamp = unchecked(timestamp + samplesPerFrame);
								nextTicks += ticksPerFrame;
							}
						}
					}
					//Dont sleep for 1 millisecond if we need to output audio in the next 1.5
					else if (_sendQueue.Count == 0 || ticksToNextFrame >= spinLockThreshold)
#if USE_THREAD
						Thread.Sleep(1);
#else
						await Task.Delay(1);
#endif
				}
			}
			catch (OperationCanceledException) { }
			catch (ObjectDisposedException) { }
			catch (Exception ex) { DisconnectInternal(ex); }
        }
#endif
		//Closes the UDP socket when _disconnectToken is triggered, since UDPClient doesn't allow passing a canceltoken
		private async Task WatcherAsync()
		{
			var cancelToken = _disconnectToken.Token;
			try
			{
				await Task.Delay(-1, cancelToken);
			}
			catch (OperationCanceledException) { }
			finally { _udp.Close(); }
        }
		
		protected override async Task ProcessMessage(string json)
		{
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			switch (msg.Operation)
			{
				case 2: //READY
					{
						if (!_isReady)
						{
							var payload = (msg.Payload as JToken).ToObject<VoiceWebSocketEvents.Ready>();
							_heartbeatInterval = payload.HeartbeatInterval;
							_ssrc = payload.SSRC;
							_endpoint = new IPEndPoint((await Dns.GetHostAddressesAsync(_host.Replace("wss://", ""))).FirstOrDefault(), payload.Port);
							//_mode = payload.Modes.LastOrDefault();
							_mode = "plain";
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
								0x0, 0x0, 0x0, 0x0, 0x0, 0x0 }, 70);
						}
					}
					break;
				case 4: //SESSION_DESCRIPTION
					{
						var payload = (msg.Payload as JToken).ToObject<VoiceWebSocketEvents.JoinServer>();
						_secretKey = payload.SecretKey;
						SendIsTalking(true);
						_connectWaitOnLogin.Set();
					}
					break;
				default:
					if (_isDebug)
						RaiseOnDebugMessage(DebugMessageType.WebSocketUnknownOpCode, "Unknown Opcode: " + msg.Operation);
					break;
			}
		}

		private void ProcessUdpMessage(UdpReceiveResult msg)
		{
            if (msg.Buffer.Length > 0 && msg.RemoteEndPoint.Equals(_endpoint))
			{
				byte[] buffer = msg.Buffer;
				int length = msg.Buffer.Length;
				if (!_isReady)
				{
					_isReady = true;
					if (length != 70)
					{
						if (_isDebug)
							RaiseOnDebugMessage(DebugMessageType.VoiceInput, $"Unexpected message length. Expected >= 70, got {length}.");
						return;
					}

					int port = buffer[68] | buffer[69] << 8;

					_myIp = Encoding.ASCII.GetString(buffer, 4, 70 - 6).TrimEnd('\0');

					_isReady = true;
					var login2 = new VoiceWebSocketCommands.Login2();
					login2.Payload.Protocol = "udp";
					login2.Payload.SocketData.Address = _myIp;
					login2.Payload.SocketData.Mode = _mode;
					login2.Payload.SocketData.Port = port;
					QueueMessage(login2);
				}
				else
				{
					//Parse RTP Data
					if (length < 12)
					{
						if (_isDebug)
							RaiseOnDebugMessage(DebugMessageType.VoiceInput, $"Unexpected message length. Expected >= 12, got {length}.");
						return;
					}

					byte flags = buffer[0];
					if (flags != 0x80)
					{
						if (_isDebug)
							RaiseOnDebugMessage(DebugMessageType.VoiceInput, $"Unexpected Flags: {flags}");
						return;
					}

					byte payloadType = buffer[1];
					if (payloadType != 0x78)
					{
						if (_isDebug)
							RaiseOnDebugMessage(DebugMessageType.VoiceInput, $"Unexpected Payload Type: {flags}");
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

					if (_isDebug)
						RaiseOnDebugMessage(DebugMessageType.VoiceInput, $"Received {buffer.Length - 12} bytes.");
					//TODO: Use Voice Data
				}
			}
        }

		public void SendPCMFrame(byte[] data, int count)
		{
			if (count != _encoder.FrameSize)
				throw new InvalidOperationException($"Invalid frame size. Got {count}, expected {_encoder.FrameSize}.");

			byte[] payload;
            lock (_encoder)
			{
				int encodedLength = _encoder.EncodeFrame(data, _encodingBuffer);

				if (_mode == "xsalsa20_poly1305")
				{
					//TODO: Encode
				}

				payload = new byte[encodedLength];
				Buffer.BlockCopy(_encodingBuffer, 0, payload, 0, encodedLength);
            }

			_sendQueueWait.Wait(_disconnectToken.Token);
			_sendQueue.Enqueue(payload);
			if (_sendQueue.Count >= _targetAudioBufferLength)
				_sendQueueWait.Reset();
        }
		public void ClearPCMFrames()
		{
			_isClearing = true;
			byte[] ignored;
            while (_sendQueue.TryDequeue(out ignored)) { }
			_isClearing = false;
		}

		private void SendIsTalking(bool value)
		{
			var isTalking = new VoiceWebSocketCommands.IsTalking();
			isTalking.Payload.IsSpeaking = value;
			isTalking.Payload.Delay = 0;
            QueueMessage(isTalking);
		}

		protected override object GetKeepAlive()
		{
			return new VoiceWebSocketCommands.KeepAlive();
		}
	}
}
#endif