using System;

namespace Discord.Net.WebSockets
{
    public static class DefaultWebSocketProvider
    {
#if NETSTANDARD1_3
        public static readonly WebSocketProvider Instance = () => 
        {
            try
            {
                return new DefaultWebSocketClient();                    
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default WebSocketProvider is not supported on this platform.", ex);
            }
        };
#else
        public static readonly WebSocketProvider Instance = () =>
        {
            throw new PlatformNotSupportedException("The default WebSocketProvider is not supported on this platform.\n" +
                "You must specify a WebSocketProvider or target a runtime supporting .NET Standard 1.3, such as .NET Framework 4.6+.");
        };
#endif
    }
}