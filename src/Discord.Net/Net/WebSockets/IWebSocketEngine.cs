using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
	internal class WebSocketMessageEventArgs : EventArgs
	{
		public readonly string Message;
		public WebSocketMessageEventArgs(string msg) { Message = msg; }
	}

	internal interface IWebSocketEngine
	{
		event EventHandler<WebSocketMessageEventArgs> ProcessMessage;

		Task Connect(string host, CancellationToken cancelToken);
		Task Disconnect();
		void QueueMessage(string message);
		IEnumerable<Task> GetTasks(CancellationToken cancelToken);
	}
}
