using System;
using System.Net;

namespace Discord.Net.WebSockets
{
    public static class DefaultWebSocketProvider
    {
#if DEFAULTWEBSOCKET
        public static readonly WebSocketProvider Instance = Create();

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
#else
        public static readonly WebSocketProvider Instance = () =>
        {
            throw new PlatformNotSupportedException("The default WebSocketProvider is not supported on this platform.\n" +
                "You must specify a WebSocketProvider or target a runtime supporting .NET Standard 1.3, such as .NET Framework 4.6+.");
        };
#endif
    }
}