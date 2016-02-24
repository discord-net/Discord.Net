using Discord.Logging;
using System;
using System.Threading;

namespace Discord.Net.WebSockets
{
	public abstract partial class WebSocket
    {        
        public CancellationToken CancelToken { get; }
        public ConnectionState State { get; }
        public string Host { get; }

        public event EventHandler Connected = delegate { };
        public event EventHandler<DisconnectedEventArgs> Disconnected = delegate { };

        public WebSocket(DiscordConfig config, ILogger logger) { }

        public abstract void SendHeartbeat();

        public virtual void WaitForConnection(CancellationToken cancelToken) { }
	}
}
