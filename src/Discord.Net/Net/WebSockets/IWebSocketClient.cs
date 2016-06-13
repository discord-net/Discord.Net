using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public interface IWebSocketClient
    {
        event Func<byte[], int, int, Task> BinaryMessage;
        event Func<string, Task> TextMessage;
        event Func<Exception, Task> Closed;

        void SetHeader(string key, string value);
        void SetCancelToken(CancellationToken cancelToken);

        Task ConnectAsync(string host);
        Task DisconnectAsync();

        Task SendAsync(byte[] data, int index, int count, bool isText);
    }
}
