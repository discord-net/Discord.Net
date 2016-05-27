using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    //TODO: Add ETF
    public interface IWebSocketClient
    {
        event Func<BinaryMessageEventArgs, Task> BinaryMessage;
        event Func<TextMessageEventArgs, Task> TextMessage;

        void SetHeader(string key, string value);
        void SetCancelToken(CancellationToken cancelToken);

        Task Connect(string host);
        Task Disconnect();

        Task Send(byte[] data, int offset, int length, bool isText);
    }
}
