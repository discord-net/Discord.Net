using Discord.Net.WebSockets;
using Discord.Rest;
using System;

namespace Discord.Rpc
{
    public class DiscordRpcConfig : DiscordRestConfig
    {
        public const int RpcAPIVersion = 1;

        public const int PortRangeStart = 6463;
        public const int PortRangeEnd = 6472;

        /// <summary> Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting. </summary>
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary> Gets or sets the provider used to generate new websocket connections. </summary>
        public WebSocketProvider WebSocketProvider { get; set; }

        public DiscordRpcConfig()
        {
#if NETSTANDARD1_3
            WebSocketProvider = () => new DefaultWebSocketClient();
#else
            WebSocketProvider = () =>
            {
                throw new InvalidOperationException("The default websocket provider is not supported on this platform.\n" +
                    "You must specify a WebSocketProvider or target a runtime supporting .NET Standard 1.3, such as .NET Framework 4.6+.");
            };
#endif
        }
    }
}
