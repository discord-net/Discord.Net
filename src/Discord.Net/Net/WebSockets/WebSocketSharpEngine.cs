/*#if !DOTNET5_4
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WSSharpWebSocket = WebSocketSharp.WebSocket;

namespace Discord.Net.WebSockets
{
	internal class WebSocketSharpEngine : IWebSocketEngine
	{
		private readonly DiscordConfig _config;
		private readonly Logger _logger;
		private readonly ConcurrentQueue<string> _sendQueue;
		private readonly WebSocket _parent;
		private WSSharpWebSocket _webSocket;

		public event EventHandler<WebSocketBinaryMessageEventArgs> BinaryMessage;
		public event EventHandler<WebSocketTextMessageEventArgs> TextMessage;
		private void RaiseBinaryMessage(byte[] data)
		{
			if (BinaryMessage != null)
				BinaryMessage(this, new WebSocketBinaryMessageEventArgs(data));
		}
		private void RaiseTextMessage(string msg)
		{
			if (TextMessage != null)
				TextMessage(this, new WebSocketTextMessageEventArgs(msg));
		}

		internal WebSocketSharpEngine(WebSocket parent, DiscordConfig config, Logger logger)
		{
			_parent = parent;
			_config = config;
			_logger = logger;
			_sendQueue = new ConcurrentQueue<string>();
		}

		public Task Connect(string host, CancellationToken cancelToken)
		{
			_webSocket = new WSSharpWebSocket(host);
			_webSocket.EmitOnPing = false;
			_webSocket.EnableRedirection = true;
			//_webSocket.Compression = WebSocketSharp.CompressionMethod.Deflate;
			_webSocket.SetProxy(null, null, null); //Disable
			//_webSocket.SetProxy(_config.ProxyUrl, _config.ProxyCredentials?.UserName, _config.ProxyCredentials?.Password);
			_webSocket.OnMessage += (s, e) =>
			{
				if (e.IsBinary)
					RaiseBinaryMessage(e.RawData);
				else if (e.IsText)
					RaiseTextMessage(e.Data);
			};
			_webSocket.OnError += async (s, e) =>
			{
				_logger.Log(LogSeverity.Error, "WebSocket Error", e.Exception);
				await _parent.SignalDisconnect(e.Exception, isUnexpected: true).ConfigureAwait(false);
			};
			_webSocket.OnClose += async (s, e) =>
			{
				string code = e.WasClean ? e.Code.ToString() : "Unexpected";
				string reason = e.Reason != "" ? e.Reason : "No Reason";
				var ex = new Exception($"Got Close Message ({code}): {reason}");
				await _parent.SignalDisconnect(ex, isUnexpected: true).ConfigureAwait(false);
			};
			_webSocket.Log.Output = (e, m) => { }; //Dont let websocket-sharp print to console directly
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
			var sendInterval = _config.WebSocketInterval;
			return Task.Run(async () =>
			{
				try
				{
					while (!cancelToken.IsCancellationRequested)
					{
						string json;
						while (_sendQueue.TryDequeue(out json))
							_webSocket.Send(json);
						await Task.Delay(sendInterval, cancelToken).ConfigureAwait(false);
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
#endif*/