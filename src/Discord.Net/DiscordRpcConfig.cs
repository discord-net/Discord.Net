using Discord.Net.WebSockets;

namespace Discord
{
    public class DiscordRpcConfig : DiscordConfig
    {
        public const int RpcAPIVersion = 1;

        public const int PortRangeStart = 6463;
        public const int PortRangeEnd = 6472;

        public DiscordRpcConfig(string clientId)
        {
            ClientId = clientId;
        }

        /// <summary> Gets or sets the Discord client/application id used for this RPC connection. </summary>
        public string ClientId { get; set; }

        /// <summary> Gets or sets the provider used to generate new websocket connections. </summary>
        public WebSocketProvider WebSocketProvider { get; set; } = () => new DefaultWebSocketClient();
    }
}
