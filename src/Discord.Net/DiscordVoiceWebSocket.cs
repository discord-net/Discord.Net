#if !DNXCORE50
using Discord.API.Models;
using Discord.Helpers;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed partial class DiscordVoiceWebSocket : DiscordWebSocket
	{
		private const int ReadyTimeout = 2500; //Max time in milliseconds between connecting to Discord and receiving a READY event

		private ManualResetEventSlim _connectWaitOnLogin, _connectWaitOnLogin2;
		private UdpClient _udp;
		private ConcurrentQueue<byte[]> _sendQueue;

		public DiscordVoiceWebSocket(int interval)
			: base(interval)
		{
			_connectWaitOnLogin = new ManualResetEventSlim(false);
			_connectWaitOnLogin2 = new ManualResetEventSlim(false);

			_udp = new UdpClient();
			_sendQueue = new ConcurrentQueue<byte[]>();
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
			_connectWaitOnLogin2.Reset();

			string ip = await Http.Get("http://ipinfo.io/ip");

			VoiceWebSocketCommands.Login msg = new VoiceWebSocketCommands.Login();
			msg.Payload.ServerId = serverId;
			msg.Payload.SessionId = sessionId;
			msg.Payload.Token = token;
			msg.Payload.UserId = userId;
			await SendMessage(msg, cancelToken);

			try
			{
				if (!_connectWaitOnLogin.Wait(ReadyTimeout, cancelToken)) //Waiting on READY message
					throw new Exception("No reply from Discord server");
			}
			catch (OperationCanceledException)
			{
				throw new InvalidOperationException("Bad Token");
			}
			try { _connectWaitOnLogin2.Wait(cancelToken); } //Waiting on READY handler
			catch (OperationCanceledException) { return; }

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
			catch (OperationCanceledException) { }
			_udp.Close();
        }

		protected override void ProcessMessage(WebSocketMessage msg)
		{
		}
		private void ProcessUdpMessage(UdpReceiveResult msg)
		{
		}

		protected override WebSocketMessage GetKeepAlive()
		{
			return new VoiceWebSocketCommands.KeepAlive();
		}
	}
}
#endif