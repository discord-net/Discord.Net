using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
	public interface IWebSocketEngine
	{
		event EventHandler<BinaryMessageEventArgs> BinaryMessage;
		event EventHandler<TextMessageEventArgs> TextMessage;

		Task Connect(string host, CancellationToken cancelToken);
		Task Disconnect();
		void QueueMessage(string message);
		IEnumerable<Task> GetTasks(CancellationToken cancelToken);
	}
}
