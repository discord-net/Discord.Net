using System.Threading;

namespace Discord.Net.WebSockets
{
    public delegate IWebSocketEngine WebSocketProvider(CancellationToken cancelToken);
}
