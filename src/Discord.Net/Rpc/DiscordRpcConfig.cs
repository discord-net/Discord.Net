using Discord.Net.WebSockets;
using Discord.Rest;

namespace Discord.Rpc
{
    public class DiscordRpcConfig : DiscordRestConfig
    {
        public const int RpcAPIVersion = 1;

        public const int PortRangeStart = 6463;
        public const int PortRangeEnd = 6472;

        public DiscordRpcConfig(string clientId, string origin)
        {
            ClientId = clientId;
            Origin = origin;
        }

        /// <summary> Gets or sets the Discord client/application id used for this RPC connection. </summary>
        public string ClientId { get; }
        /// <summary> Gets or sets the origin used for this RPC connection. </summary>
        public string Origin { get; }

        /// <summary> Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting. </summary>
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary> Gets or sets the provider used to generate new websocket connections. </summary>
        public WebSocketProvider WebSocketProvider { get; set; } = () => new DefaultWebSocketClient();
    }
}
