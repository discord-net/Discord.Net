using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Discord.Rest;

namespace Discord.WebSocket
{
    public class DiscordSocketConfig : DiscordRestConfig
    {
        public const string GatewayEncoding = "json";

        /// <summary> Gets or sets the websocket host to connect to. If null, the client will use the /gateway endpoint. </summary>
        public string GatewayHost { get; set; } = null;

        /// <summary> Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting. </summary>
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary> Gets or sets the id for this shard. Must be less than TotalShards. </summary>
        public int? ShardId { get; set; } = null;
        /// <summary> Gets or sets the total number of shards for this application. </summary>
        public int? TotalShards { get; set; } = null;

        /// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
        public int MessageCacheSize { get; set; } = 0;
        /// <summary> 
        /// Gets or sets the max number of users a guild may have for offline users to be included in the READY packet. Max is 250.
        /// </summary>
        public int LargeThreshold { get; set; } = 250;

        /// <summary> Gets or sets the provider used to generate new websocket connections. </summary>
        public WebSocketProvider WebSocketProvider { get; set; }
        /// <summary> Gets or sets the provider used to generate new udp sockets. </summary>
        public UdpSocketProvider UdpSocketProvider { get; set; }

        /// <summary> Gets or sets whether or not all users should be downloaded as guilds come available. </summary>
        public bool AlwaysDownloadUsers { get; set; } = false;
        /// <summary> Gets or sets the timeout for event handlers, in milliseconds, after which a warning will be logged. Null disables this check. </summary>
        public int? HandlerTimeout { get; set; } = 3000;

        public DiscordSocketConfig()
        {
            WebSocketProvider = DefaultWebSocketProvider.Instance;
            UdpSocketProvider = DefaultUdpSocketProvider.Instance;
        }

        internal DiscordSocketConfig Clone() => MemberwiseClone() as DiscordSocketConfig;
    }
}
