using Discord.API.Models;
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
	internal abstract partial class DiscordWebSocket : IDisposable
	{
		private const int ReceiveChunkSize = 4096;
		private const int SendChunkSize = 4096;

		protected volatile CancellationTokenSource _disconnectToken;
		protected int _heartbeatInterval;
		protected readonly int _sendInterval;

		private volatile ClientWebSocket _webSocket;
		private volatile Task _tasks;
		private ConcurrentQueue<byte[]> _sendQueue;
		private DateTime _lastHeartbeat;
		private bool _isConnected;

		public DiscordWebSocket(int interval)
		{
			_sendInterval = interval;

			_sendQueue = new ConcurrentQueue<byte[]>();
		}

		public async Task ConnectAsync(string url)
		{
			await DisconnectAsync();

			_disconnectToken = new CancellationTokenSource();
			var cancelToken = _disconnectToken.Token;

			_webSocket = new ClientWebSocket();
			_webSocket.Options.KeepAliveInterval = TimeSpan.Zero;
			await _webSocket.ConnectAsync(new Uri(url), cancelToken);

			_tasks = Task.WhenAll(CreateTasks(cancelToken))
			.ContinueWith(x =>
			{
				//Do not clean up until both tasks have ended
				_heartbeatInterval = 0;
				_lastHeartbeat = DateTime.MinValue;
				_webSocket.Dispose();
				_webSocket = null;
				_disconnectToken.Dispose();
				_disconnectToken = null;

				//Clear send queue
				byte[] ignored;
				while (_sendQueue.TryDequeue(out ignored)) { }

				if (_isConnected)
				{
					_isConnected = false;
					RaiseDisconnected();
				}

				_tasks = null;
			});
		}
		public async Task DisconnectAsync()
		{
			if (_tasks != null)
			{
				try { _disconnectToken.Cancel(); } catch (NullReferenceException) { }
				try { await _tasks; } catch (NullReferenceException) { }
			}
		}

		protected void SetConnected()
		{
			_isConnected = true;
			RaiseConnected();
		}

		protected virtual Task[] CreateTasks(CancellationToken cancelToken)
		{
			return new Task[]
			{
				Task.Factory.StartNew(ReceiveAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Result,
				Task.Factory.StartNew(SendAsync, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Result
			};
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
					ProcessMessage(msg);

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
							await SendMessage(GetKeepAlive(), cancelToken);
							_lastHeartbeat = now;
						}
					}
					while (_sendQueue.TryDequeue(out bytes))
						await SendMessage(bytes, cancelToken);
					await Task.Delay(_sendInterval);
				}
			}
			catch { }
			finally { _disconnectToken.Cancel(); }
		}

		protected abstract void ProcessMessage(WebSocketMessage msg);
		protected abstract WebSocketMessage GetKeepAlive();

        protected void QueueMessage(object message)
		{
			var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
			_sendQueue.Enqueue(bytes);
		}
		protected Task SendMessage(object message, CancellationToken cancelToken)
			=> SendMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)), cancelToken);
		protected async Task SendMessage(byte[] message, CancellationToken cancelToken)
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
