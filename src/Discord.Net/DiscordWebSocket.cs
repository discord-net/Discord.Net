using Discord.API.Models;
using Discord.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed partial class DiscordWebSocket : IDisposable
	{
		private const int ReceiveChunkSize = 4096;
		private const int SendChunkSize = 4096;
		private const int ReadyTimeout = 2500; //Max time in milliseconds between connecting to Discord and receiving a READY event

		private volatile ClientWebSocket _webSocket;
		private volatile CancellationTokenSource _disconnectToken;
		private volatile Task _tasks;
		private ConcurrentQueue<byte[]> _sendQueue;
		private int _heartbeatInterval;
		private DateTime _lastHeartbeat;
		private ManualResetEventSlim _connectWaitOnLogin, _connectWaitOnLogin2;
		private bool _isConnected;

		public DiscordWebSocket()
		{
			_connectWaitOnLogin = new ManualResetEventSlim(false);
			_connectWaitOnLogin2 = new ManualResetEventSlim(false);
			
			_sendQueue = new ConcurrentQueue<byte[]>();
		}

		public async Task ConnectAsync(string url, bool autoLogin)
		{
			await DisconnectAsync();

			_disconnectToken = new CancellationTokenSource();
			var cancelToken = _disconnectToken.Token;

			_webSocket = new ClientWebSocket();
			_webSocket.Options.KeepAliveInterval = TimeSpan.Zero;
			await _webSocket.ConnectAsync(new Uri(url), cancelToken);

			_tasks = Task.WhenAll(
				await Task.Factory.StartNew(ReceiveAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default),
				await Task.Factory.StartNew(SendAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default))
			.ContinueWith(x =>
			{
				//Do not clean up until both tasks have ended
				_heartbeatInterval = 0;
				_lastHeartbeat = DateTime.MinValue;
				_webSocket.Dispose();
				_webSocket = null;
				_disconnectToken.Dispose();
				_disconnectToken = null;
				_tasks = null;

				//Clear send queue
				byte[] ignored;
				while (_sendQueue.TryDequeue(out ignored)) { }

				if (_isConnected)
				{
					_isConnected = false;
					RaiseDisconnected();
				}
			});

			if (autoLogin)
				Login();
        }
		public void Login()
		{
			var cancelToken = _disconnectToken.Token;

			_connectWaitOnLogin.Reset();
			_connectWaitOnLogin2.Reset();

			WebSocketCommands.Login msg = new WebSocketCommands.Login();
			msg.Payload.Token = Http.Token;
			msg.Payload.Properties["$os"] = "";
			msg.Payload.Properties["$browser"] = "";
			msg.Payload.Properties["$device"] = "Discord.Net";
			msg.Payload.Properties["$referrer"] = "";
			msg.Payload.Properties["$referring_domain"] = "";
			SendMessage(msg, _disconnectToken.Token);

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

			_isConnected = true;
			RaiseConnected();
		}
		public async Task DisconnectAsync()
		{
			if (_tasks != null)
			{
				_disconnectToken.Cancel();
				await _tasks;
			}
		}

		private async Task ReceiveAsync()
		{
			var cancelToken = _disconnectToken.Token;
			var buffer = new byte[ReceiveChunkSize];
			var builder = new StringBuilder();

			try
			{
				while (_webSocket.State == WebSocketState.Open && !cancelToken.IsCancellationRequested)
				{
					WebSocketReceiveResult result;
					do
					{
						result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _disconnectToken.Token);

						if (result.MessageType == WebSocketMessageType.Close)
						{
							await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
							return;
						}
						else
							builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

					}
					while (!result.EndOfMessage);

					var msg = JsonConvert.DeserializeObject<WebSocketMessage>(builder.ToString());
					switch (msg.Operation)
					{
						case 0:
							if (msg.Type == "READY")
							{
								var payload = (msg.Payload as JToken).ToObject<WebSocketEvents.Ready>();
								_heartbeatInterval = payload.HeartbeatInterval;
								QueueMessage(new WebSocketCommands.UpdateStatus());
								QueueMessage(new WebSocketCommands.KeepAlive());
								_connectWaitOnLogin.Set(); //Pre-Event
                            }
							RaiseGotEvent(msg.Type, msg.Payload as JToken);
							if (msg.Type == "READY")
								_connectWaitOnLogin2.Set(); //Post-Event
							break;
						default:
							RaiseOnDebugMessage("Unknown WebSocket operation ID: " + msg.Operation);
							break;
					}

					builder.Clear();
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
				while (_webSocket.State == WebSocketState.Open && !cancelToken.IsCancellationRequested)
				{
					if (_heartbeatInterval > 0)
					{
						DateTime now = DateTime.UtcNow;
						if ((now - _lastHeartbeat).TotalMilliseconds > _heartbeatInterval)
						{
							await SendMessage(new WebSocketCommands.KeepAlive(), cancelToken);
							_lastHeartbeat = now;
						}
					}
					while (_sendQueue.TryDequeue(out bytes))
						await SendMessage(bytes, cancelToken);
					await Task.Delay(100);
				}
			}
			catch { }
			finally { _disconnectToken.Cancel(); }
		}

		private void QueueMessage(object message)
		{
			var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
			_sendQueue.Enqueue(bytes);
		}

		private Task SendMessage(object message, CancellationToken cancelToken)
			=> SendMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)), cancelToken);
		private async Task SendMessage(byte[] message, CancellationToken cancelToken)
		{
			var frameCount = (int)Math.Ceiling((double)message.Length / SendChunkSize);

			int offset = 0;
			for (var i = 0; i < frameCount; i++, offset += SendChunkSize)
			{
				bool isLast = i == (frameCount - 1);

				int count;
				if (isLast)
					count = message.Length - (i * SendChunkSize);
				else
					count = SendChunkSize;

				await _webSocket.SendAsync(new ArraySegment<byte>(message, offset, count), WebSocketMessageType.Text, isLast, cancelToken);
			}
		}

		#region IDisposable Support
		private bool _isDisposed = false;

		public void Dispose()
		{
			if (!_isDisposed)
			{
				DisconnectAsync().Wait();
				_isDisposed = true;
			}
		}
		#endregion
	}
}
