using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public interface IWebSocketClient : IDisposable
    {
        event Func<byte[], int, int, Task> BinaryMessage;
        event Func<string, Task> TextMessage;
        event Func<Exception, Task> Closed;

        void SetHeader(string key, string value);
        void SetCancelToken(CancellationToken cancelToken);

        Task ConnectAsync(string host);
        Task DisconnectAsync(int closeCode = 1000);

        Task SendAsync(byte[] data, int index, int count, bool isText);
    }
}
