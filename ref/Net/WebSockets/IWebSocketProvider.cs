using System.Threading;

namespace Discord.Net.WebSockets
{
    public interface IWebSocketProvider
    {
        IWebSocket Create(CancellationToken cancelToken);
    }
}
