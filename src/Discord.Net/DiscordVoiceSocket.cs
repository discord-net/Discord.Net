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
		private struct Packet
		{
			public byte[] Data;
			public int Count;
			public Packet(byte[] data, int count)
			{
				Data = data;
				Count = count;
			}
		}

		private ManualResetEventSlim _connectWaitOnLogin;
		private uint _ssrc;
		private readonly Random _rand = new Random();

#if !DNXCORE50
		private OpusEncoder _encoder;
		private ConcurrentQueue<Packet> _sendQueue;
		private UdpClient _udp;
		private IPEndPoint _endpoint;
		private bool _isReady, _isClearing;
		private byte[] _secretKey;
		private string _myIp;
		private ushort _sequence;
		private string _mode;
#if USE_THREAD
		private Thread _sendThread;
#endif
#endif

		public DiscordVoiceSocket(DiscordClient client, int timeout, int interval)
			: base(client, timeout, interval)
		{
			_connectWaitOnLogin = new ManualResetEventSlim(false);
#if !DNXCORE50
			_sendQueue = new ConcurrentQueue<Packet>();
			_encoder = new OpusEncoder(48000, 1, 20, Application.Audio);
#endif
		}

#if !DNXCORE50
		protected override void OnConnect()
		{
			_udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
			_udp.AllowNatTraversal(true);
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
#endif

		protected override Task[] CreateTasks()
		{
#if !DNXCORE50 && USE_THREAD
			_sendThread = new Thread(new ThreadStart(() => SendAsync(_disconnectToken)));
			_sendThread.Start();
#endif
			return new Task[]
			{
#if !DNXCORE50
				ReceiveAsync(),
#if !USE_THREAD
				SendAsync(),
#endif
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
				throw new InvalidOperationException("Bad Token");
			}

			SetConnected();
		}

#if !DNXCORE50
		private async Task ReceiveAsync()
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
			catch { }
			finally { cancelSource.Cancel(); }
		}

#if USE_THREAD
		private void SendAsync(CancellationTokenSource cancelSource)
		{
			var cancelToken = cancelSource.Token;
#else
		private async Task SendAsync()
		{
			var cancelSource = _disconnectToken;
			var cancelToken = cancelSource.Token;
			await Task.Yield();
#endif

			Packet packet;
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

				byte[] rtpPacket = new byte[4012];
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
									Buffer.BlockCopy(packet.Data, 0, rtpPacket, 12, packet.Count);
#if USE_THREAD
									_udp.Send(rtpPacket, packet.Count + 12);
#else
									await _udp.SendAsync(rtpPacket, packet.Count + 12);
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
			catch { }
			finally { cancelSource.Cancel(); }
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
			catch (TaskCanceledException) { }
#if !DNXCORE50
			finally { _udp.Close(); }
#endif
        }

#if DNXCORE50
		protected override Task ProcessMessage(string json)
#else
		protected override async Task ProcessMessage(string json)
#endif
		{
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			switch (msg.Operation)
			{
				case 2: //READY
					{
#if !DNXCORE50
						if (!_isReady)
						{
#endif
							var payload = (msg.Payload as JToken).ToObject<VoiceWebSocketEvents.Ready>();
							_heartbeatInterval = payload.HeartbeatInterval;
							_ssrc = payload.SSRC;
#if !DNXCORE50
							_endpoint = new IPEndPoint((await Dns.GetHostAddressesAsync(_host)).FirstOrDefault(), payload.Port);
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
#else
							_connectWaitOnLogin.Set();
#endif
					}
					break;
#if !DNXCORE50
				case 4: //SESSION_DESCRIPTION
					{
						var payload = (msg.Payload as JToken).ToObject<VoiceWebSocketEvents.JoinServer>();
						_secretKey = payload.SecretKey;
						SendIsTalking(true);
						_connectWaitOnLogin.Set();
					}
					break;
#endif
				default:
					RaiseOnDebugMessage(DebugMessageType.WebSocketUnknownInput, "Unknown VoiceSocket op: " + msg.Operation);
					break;
			}
#if DNXCORE50
			return Task.CompletedTask;
#endif
		}
#if !DNXCORE50
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
						throw new Exception($"Unexpected message length. Expected 70, got {length}.");

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
					/*if (length < 12)
						throw new Exception($"Unexpected message length. Expected >= 12, got {length}.");

					byte flags = buffer[0];
					if (flags != 0x80)
						throw new Exception("Unexpected Flags");

					byte payloadType = buffer[1];
					if (payloadType != 0x78)
						throw new Exception("Unexpected Payload Type");

					ushort sequenceNumber = (ushort)((buffer[2] << 8) | buffer[3]);
					uint timestamp = (uint)((buffer[4] << 24) | (buffer[5] << 16) |
											(buffer[6] << 8) | (buffer[7] << 0));
					uint ssrc = (uint)((buffer[8] << 24) | (buffer[9] << 16) |
											 (buffer[10] << 8) | (buffer[11] << 0));

					//Decrypt
					if (_mode == "xsalsa20_poly1305")
					{
						if (length < 36) //12 + 24
							throw new Exception($"Unexpected message length. Expected >= 36, got {length}.");

#if !DNXCORE50
						byte[] nonce = new byte[24]; //16 bytes static, 8 bytes incrementing?
						Buffer.BlockCopy(buffer, 12, nonce, 0, 24);

						byte[] cipherText = new byte[buffer.Length - 36];
						Buffer.BlockCopy(buffer, 36, cipherText, 0, cipherText.Length);
						
						Sodium.SecretBox.Open(cipherText, nonce, _secretKey);
#endif
					}
					else //Plain
					{
						byte[] newBuffer = new byte[buffer.Length - 12];
						Buffer.BlockCopy(buffer, 12, newBuffer, 0, newBuffer.Length);
						buffer = newBuffer;
					}*/

					//TODO: Use Voice Data
				}
			}
        }

		public void SendPCMFrame(byte[] data, int count)
		{
			if (count != _encoder.FrameSize)
				throw new InvalidOperationException($"Invalid frame size. Got {count}, expected {_encoder.FrameSize}.");

			lock (_encoder)
			{
				byte[] payload = new byte[4000];
				int encodedLength = _encoder.EncodeFrame(data, payload);

				if (_mode == "xsalsa20_poly1305")
				{
					//TODO: Encode
				}

				lock (_sendQueue)
					_sendQueue.Enqueue(new Packet(payload, encodedLength));
			}
		}
		public void ClearPCMFrames()
		{
			_isClearing = true;
			Packet ignored;
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
#endif

		protected override object GetKeepAlive()
		{
			return new VoiceWebSocketCommands.KeepAlive();
		}
	}
}
#endif