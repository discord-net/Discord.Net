using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
	internal class WebSocketBinaryMessageEventArgs : EventArgs
	{
		public readonly byte[] Data;
		public WebSocketBinaryMessageEventArgs(byte[] data) { Data = data; }
	}
	internal class WebSocketTextMessageEventArgs : EventArgs
	{
		public readonly string Message;
		public WebSocketTextMessageEventArgs(string msg) { Message = msg; }
	}

	internal interface IWebSocketEngine
	{
		event EventHandler<WebSocketBinaryMessageEventArgs> BinaryMessage;
		event EventHandler<WebSocketTextMessageEventArgs> TextMessage;

		Task Connect(string host, CancellationToken cancelToken);
		Task Disconnect();
		void QueueMessage(string message);
		IEnumerable<Task> GetTasks(CancellationToken cancelToken);
	}
}
