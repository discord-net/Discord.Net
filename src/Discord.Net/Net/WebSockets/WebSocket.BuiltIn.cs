using Discord.Helpers;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using State = System.Net.WebSockets.WebSocketState;

namespace Discord.Net.WebSockets
{
    internal class BuiltInWebSocketEngine : IWebSocketEngine
	{
		private const int ReceiveChunkSize = 4096;
		private const int SendChunkSize = 4096;
		private const int HR_TIMEOUT = -2147012894;

		private readonly ConcurrentQueue<byte[]> _sendQueue;
		private readonly ClientWebSocket _webSocket;
		private readonly int _sendInterval;
		
		public event EventHandler<WebSocketMessageEventArgs> ProcessMessage;
		private void RaiseProcessMessage(string msg)
		{
			if (ProcessMessage != null)
				ProcessMessage(this, new WebSocketMessageEventArgs(msg));
		}

		public BuiltInWebSocketEngine(int sendInterval)
		{
			_sendInterval = sendInterval;
			_sendQueue = new ConcurrentQueue<byte[]>();
			_webSocket = new ClientWebSocket();
            _webSocket.Options.KeepAliveInterval = TimeSpan.Zero;
        }

		public Task Connect(string host, CancellationToken cancelToken)
		{
			return _webSocket.ConnectAsync(new Uri(host), cancelToken);
		}

		public Task Disconnect()
		{
			byte[] ignored;
			while (_sendQueue.TryDequeue(out ignored)) { }
			return TaskHelper.CompletedTask;
		}

		public Task[] RunTasks(CancellationToken cancelToken)
		{
			return new Task[]
			{
				ReceiveAsync(cancelToken),
				SendAsync(cancelToken)
			};
		}

		private Task ReceiveAsync(CancellationToken cancelToken)
		{
			return Task.Run(async () =>
			{
				var buffer = new ArraySegment<byte>(new byte[ReceiveChunkSize]);
                var builder = new StringBuilder();

				try
				{
					while (_webSocket.State == State.Open && !cancelToken.IsCancellationRequested)
					{
						WebSocketReceiveResult result = null;
						do
						{
							if (_webSocket.State != State.Open || cancelToken.IsCancellationRequested)
								return;

							try
							{
								result = await _webSocket.ReceiveAsync(buffer, cancelToken).ConfigureAwait(false);
							}
							catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
							{
								throw new Exception($"Connection timed out.");
							}

							if (result.MessageType == WebSocketMessageType.Close)
								throw new Exception($"Got Close Message ({result.CloseStatus?.ToString() ?? "Unexpected"}, {result.CloseStatusDescription ?? "No Reason"})");
							else
								builder.Append(Encoding.UTF8.GetString(buffer.Array, buffer.Offset, result.Count));

						}
						while (result == null || !result.EndOfMessage);
						
						RaiseProcessMessage(builder.ToString());

						builder.Clear();
					}
				}
				catch (OperationCanceledException) { }
			});
		}
		private Task SendAsync(CancellationToken cancelToken)
		{
			return Task.Run(async () =>
			{
				try
				{
					byte[] bytes;
					while (_webSocket.State == State.Open && !cancelToken.IsCancellationRequested)
					{
						while (_sendQueue.TryDequeue(out bytes))
							await SendMessageInternal(bytes, cancelToken);
						await Task.Delay(_sendInterval, cancelToken).ConfigureAwait(false);
					}
				}
				catch (OperationCanceledException) { }
			});
		}
		
		public void QueueMessage(byte[] message)
		{
			_sendQueue.Enqueue(message);
        }

		private async Task SendMessageInternal(byte[] message, CancellationToken cancelToken)
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

				try
				{
					await _webSocket.SendAsync(new ArraySegment<byte>(message, offset, count), WebSocketMessageType.Text, isLast, cancelToken).ConfigureAwait(false);
				}
				catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
				{
					return;
				}
			}
		}
	}
}