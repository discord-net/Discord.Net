#if !DNXCORE50
using Discord.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WSSharpNWebSocket = WebSocketSharp.WebSocket;

namespace Discord.WebSockets
{
	public class WSSharpWebSocketEngine : IWebSocketEngine
	{
		private readonly ConcurrentQueue<string> _sendQueue;
		private readonly int _sendInterval;
		private readonly string _userAgent;
		private readonly WebSocket _parent;
		private WSSharpNWebSocket _webSocket;

		public event EventHandler<WebSocketMessageEventArgs> ProcessMessage;
		private void RaiseProcessMessage(string msg)
		{
			if (ProcessMessage != null)
				ProcessMessage(this, new WebSocketMessageEventArgs(msg));
		}

		internal WSSharpWebSocketEngine(WebSocket parent, string userAgent, int sendInterval)
		{
			_parent = parent;
			_userAgent = userAgent;
			_sendInterval = sendInterval;
			_sendQueue = new ConcurrentQueue<string>();
		}

		public Task Connect(string host, CancellationToken cancelToken)
		{
			_webSocket = new WSSharpNWebSocket(host);
			_webSocket.EmitOnPing = false;
			_webSocket.EnableRedirection = true;
            _webSocket.Compression = WebSocketSharp.CompressionMethod.None;
            _webSocket.OnMessage += (s, e) => RaiseProcessMessage(e.Data);
			_webSocket.OnError += async (s, e) =>
			{
				_parent.RaiseOnLog(LogMessageSeverity.Error, $"Websocket Error: {e.Message}");
				await _parent.DisconnectInternal(e.Exception, skipAwait: true);
			};
			_webSocket.OnClose += async (s, e) =>
			{
				string code = e.WasClean ? e.Code.ToString() : "Unexpected";
				string reason = e.Reason != "" ? e.Reason : "No Reason";
				Exception ex = new Exception($"Got Close Message ({code}): {reason}");
				await _parent.DisconnectInternal(ex, skipAwait: true);
			};
			_webSocket.Log.Output = (e, m) => { }; //Dont let websocket-sharp print to console
            _webSocket.Connect();
			return TaskHelper.CompletedTask;
		}

		public Task Disconnect()
		{
			string ignored;
			while (_sendQueue.TryDequeue(out ignored)) { }

			var socket = _webSocket;
			_webSocket = null;
			if (socket != null)
				socket.Close();

			return TaskHelper.CompletedTask;
		}

		public IEnumerable<Task> GetTasks(CancellationToken cancelToken)
		{
			return new Task[]
			{
				SendAsync(cancelToken)
			};
		}

		private Task SendAsync(CancellationToken cancelToken)
		{
			return Task.Run(async () =>
			{
				try
				{
					while (!cancelToken.IsCancellationRequested)
					{
						string json;
						while (_sendQueue.TryDequeue(out json))
							_webSocket.Send(json);
						await Task.Delay(_sendInterval, cancelToken).ConfigureAwait(false);
					}
				}
				catch (OperationCanceledException) { }
			});
		}

		public void QueueMessage(string message)
		{
			_sendQueue.Enqueue(message);
		}
	}
}
#endif