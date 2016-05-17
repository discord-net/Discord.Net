using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    //TODO: Add ETF
    public interface IWebSocketClient
    {
        event EventHandler<BinaryMessageEventArgs> BinaryMessage;
        event EventHandler<TextMessageEventArgs> TextMessage;

        void SetHeader(string key, string value);

        Task Connect(string host, CancellationToken cancelToken);
        Task Disconnect();
        void QueueMessage(string message);
    }
}
