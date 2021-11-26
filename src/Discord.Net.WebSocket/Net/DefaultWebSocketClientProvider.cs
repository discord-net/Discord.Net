using System;
using System.Net;

namespace Discord.Net.WebSockets
{
    public static class DefaultWebSocketProvider
    {
        public static readonly WebSocketProvider Instance = Create();

        /// <exception cref="PlatformNotSupportedException">The default WebSocketProvider is not supported on this platform.</exception>
        public static WebSocketProvider Create(IWebProxy proxy = null)
        {
            return () =>
            {
                try
                {
                    return new DefaultWebSocketClient(proxy);
                }
                catch (PlatformNotSupportedException ex)
                {
                    throw new PlatformNotSupportedException("The default WebSocketProvider is not supported on this platform.", ex);
                }
            };
        }
    }
}
