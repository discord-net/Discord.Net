using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public interface IWebSocketClient
    {
        event Func<byte[], int, int, Task> BinaryMessage;
        event Func<string, Task> TextMessage;

        void SetHeader(string key, string value);
        void SetCancelToken(CancellationToken cancelToken);

        Task Connect(string host);
        Task Disconnect();

        Task Send(byte[] data, int index, int count, bool isText);
    }
}
