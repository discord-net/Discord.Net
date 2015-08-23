//#if !DNXCORE50
using Discord.API.Models;
using Discord.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WebSocketMessage = Discord.API.Models.VoiceWebSocketCommands.WebSocketMessage;

namespace Discord
{
	internal sealed partial class DiscordVoiceWebSocket : DiscordWebSocket
	{
		private const int ReadyTimeout = 2500; //Max time in milliseconds between connecting to Discord and receiving a READY event

		private ManualResetEventSlim _connectWaitOnLogin;
		private UdpClient _udp;
		private ConcurrentQueue<byte[]> _sendQueue;
		private string _ip;

		public DiscordVoiceWebSocket(int interval)
			: base(interval)
		{
			_connectWaitOnLogin = new ManualResetEventSlim(false);

			_sendQueue = new ConcurrentQueue<byte[]>();
        }

		protected override void OnConnect()
		{
			_udp = new UdpClient(0);
		}
		protected override void OnDisconnect()
		{
			_udp = null;
		}

		protected override Task[] CreateTasks(CancellationToken cancelToken)
		{
			return new Task[]
			{
				Task.Factory.StartNew(ReceiveAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Result,
				Task.Factory.StartNew(SendAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Result,
				Task.Factory.StartNew(WatcherAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Result
			}.Concat(base.CreateTasks(cancelToken)).ToArray();
		}

		public async Task Login(string serverId, string userId, string sessionId, string token)
		{
			var cancelToken = _disconnectToken.Token;

			_connectWaitOnLogin.Reset();

			_ip = (await Http.Get("http://ipinfo.io/ip")).Trim();

			VoiceWebSocketCommands.Login msg = new VoiceWebSocketCommands.Login();
			msg.Payload.ServerId = serverId;
			msg.Payload.SessionId = sessionId;
			msg.Payload.Token = token;
			msg.Payload.UserId = userId;
			await SendMessage(msg, cancelToken);
			System.Diagnostics.Debug.WriteLine("<<< " + JsonConvert.SerializeObject(msg));

			try
			{
				if (!_connectWaitOnLogin.Wait(ReadyTimeout, cancelToken)) //Waiting on JoinServer message
					throw new Exception("No reply from Discord server");
			}
			catch (OperationCanceledException)
			{
				throw new InvalidOperationException("Bad Token");
			}

			SetConnected();
		}

		private async Task ReceiveAsync()
		{
			var cancelToken = _disconnectToken.Token;

			try
			{
				while (!cancelToken.IsCancellationRequested)
				{
					var result = await _udp.ReceiveAsync();					
					ProcessUdpMessage(result);
				}
			}
			catch { }
			finally { _disconnectToken.Cancel(); }
		}
		private async Task SendAsync()
		{
			var cancelToken = _disconnectToken.Token;
			try
			{
				byte[] bytes;
				while (!cancelToken.IsCancellationRequested)
				{
					while (_sendQueue.TryDequeue(out bytes))
						await SendMessage(bytes, cancelToken);
					await Task.Delay(_sendInterval);
				}
			}
			catch { }
			finally { _disconnectToken.Cancel(); }
		}
		private async Task WatcherAsync()
		{
			try
			{
				await Task.Delay(-1, _disconnectToken.Token);
			}
			catch (TaskCanceledException) { }
#if DNXCORE50
			finally { _udp.Dispose(); }
#else
			finally { _udp.Close(); }
#endif
        }

		protected override void ProcessMessage(string json)
		{
			var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
			System.Diagnostics.Debug.WriteLine(">>> " + JsonConvert.SerializeObject(msg));
			switch (msg.Operation)
			{
				case 2:
					{
						var payload = (msg.Payload as JToken).ToObject<VoiceWebSocketEvents.Ready>();
						_heartbeatInterval = payload.HeartbeatInterval;

						var login2 = new VoiceWebSocketCommands.Login2();
						login2.Payload.Protocol = "udp";
						login2.Payload.SocketData.Address = _ip;
						login2.Payload.SocketData.Mode = payload.Modes.Last();
						login2.Payload.SocketData.Port = (_udp.Client.LocalEndPoint as IPEndPoint).Port;
						QueueMessage(login2);

						System.Diagnostics.Debug.WriteLine("<<< " + JsonConvert.SerializeObject(login2));
					}
					break;
				case 4:
					{
						var payload = (msg.Payload as JToken).ToObject<VoiceWebSocketEvents.JoinServer>();
						QueueMessage(GetKeepAlive());

						System.Diagnostics.Debug.WriteLine("<<< " + JsonConvert.SerializeObject(GetKeepAlive()));
						_connectWaitOnLogin.Set();
					}
					break;
				default:
					RaiseOnDebugMessage("Unknown WebSocket operation ID: " + msg.Operation);
					break;
			}
		}
		private void ProcessUdpMessage(UdpReceiveResult msg)
		{
		}

		protected override object GetKeepAlive()
		{
			return new VoiceWebSocketCommands.KeepAlive();
		}
	}
}
//#endif