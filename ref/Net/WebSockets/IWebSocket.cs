using System;
using System.Threading;

namespace Discord.Net.WebSockets
{
    public interface IWebSocket
    {
        CancellationToken CancelToken { get; }
        ConnectionState State { get; }
        string Host { get; set; }

        event EventHandler Connected;
        event EventHandler<DisconnectedEventArgs> Disconnected;
    }
}
