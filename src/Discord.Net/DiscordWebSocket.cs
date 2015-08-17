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
		private const int ReadyTimeout = 5000; //Max time in milliseconds between connecting to Discord and receiving a READY event

		private volatile ClientWebSocket _webSocket;
		private volatile CancellationTokenSource _cancelToken;
		private volatile Task _tasks;
		private ConcurrentQueue<byte[]> _sendQueue;
		private int _heartbeatInterval;
		private DateTime _lastHeartbeat;
		private AutoResetEvent _connectWaitOnLogin, _connectWaitOnLogin2;

		public async Task ConnectAsync(string url, bool autoLogin, HttpOptions options)
		{
			await DisconnectAsync();

			_connectWaitOnLogin = new AutoResetEvent(false);
			_connectWaitOnLogin2 = new AutoResetEvent(false);
			_sendQueue = new ConcurrentQueue<byte[]>();

			_webSocket = new ClientWebSocket();
			_webSocket.Options.KeepAliveInterval = TimeSpan.Zero;

			_cancelToken = new CancellationTokenSource();
			var cancelToken = _cancelToken.Token;

			await _webSocket.ConnectAsync(new Uri(url), cancelToken);
			_tasks = Task.WhenAll(
				await Task.Factory.StartNew(ReceiveAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default),
				await Task.Factory.StartNew(SendAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
			).ContinueWith(x =>
			{
				//Do not clean up until both tasks have ended
				_heartbeatInterval = 0;
				_lastHeartbeat = DateTime.MinValue;
				_webSocket.Dispose();
				_webSocket = null;
				_cancelToken.Dispose();
				_cancelToken = null;
				_tasks = null;

				RaiseDisconnected();
			});

			if (autoLogin)
				Login(options);
        }
		public void Login(HttpOptions options)
		{
			WebSocketCommands.Login msg = new WebSocketCommands.Login();
			msg.Payload.Token = options.Token;
			msg.Payload.Properties["$os"] = "";
			msg.Payload.Properties["$browser"] = "";
			msg.Payload.Properties["$device"] = "Discord.Net";
			msg.Payload.Properties["$referrer"] = "";
			msg.Payload.Properties["$referring_domain"] = "";
			SendMessage(msg, _cancelToken.Token);

			if (!_connectWaitOnLogin.WaitOne(ReadyTimeout)) //Pre-Event
				throw new Exception("No reply from Discord server");
			_connectWaitOnLogin2.WaitOne(); //Post-Event

			RaiseConnected();
		}
		public async Task DisconnectAsync()
		{
			if (_tasks != null)
			{
				_cancelToken.Cancel();
				await _tasks;
			}
		}

		private async Task ReceiveAsync()
		{
			var cancelToken = _cancelToken.Token;
			var buffer = new byte[ReceiveChunkSize];
			var builder = new StringBuilder();

			try
			{
				while (_webSocket.State == WebSocketState.Open && !cancelToken.IsCancellationRequested)
				{
					WebSocketReceiveResult result;
					do
					{
						result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancelToken.Token);

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
								SendMessage(new WebSocketCommands.UpdateStatus(), cancelToken);
								SendMessage(new WebSocketCommands.KeepAlive(), cancelToken);
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
			finally { _cancelToken.Cancel(); }
		}

		private async Task SendAsync()
		{
			var cancelToken = _cancelToken.Token;
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
							SendMessage(new WebSocketCommands.KeepAlive(), cancelToken);
							_lastHeartbeat = now;
						}
					}
					while (_sendQueue.TryDequeue(out bytes))
					{
						var frameCount = (int)Math.Ceiling((double)bytes.Length / SendChunkSize);

						int offset = 0;
						for (var i = 0; i < frameCount; i++, offset += SendChunkSize)
						{
							bool isLast = i == (frameCount - 1);

							int count;
							if (isLast)
								count = bytes.Length - (i * SendChunkSize);
							else
								count = SendChunkSize;

							await _webSocket.SendAsync(new ArraySegment<byte>(bytes, offset, count), WebSocketMessageType.Text, isLast, cancelToken);
						}
					}
					await Task.Delay(100);
				}
			}
			catch { }
			finally { _cancelToken.Cancel(); }
		}

		private void SendMessage(object frame, CancellationToken token)
		{
			var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(frame));
			_sendQueue.Enqueue(bytes);
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
