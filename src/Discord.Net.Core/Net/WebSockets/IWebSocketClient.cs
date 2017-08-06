using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public interface IWebSocketClient
    {
        event Func<ReadOnlyBuffer<byte>, bool, Task> Message;
        event Func<Exception, Task> Closed;

        void SetHeader(string key, string value);
        void SetCancelToken(CancellationToken cancelToken);

        Task ConnectAsync(string host);
        Task DisconnectAsync();

        Task SendAsync(ReadOnlyBuffer<byte> data, bool isText);
    }
}
