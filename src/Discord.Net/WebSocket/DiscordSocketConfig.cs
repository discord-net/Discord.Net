using Discord.Audio;
using Discord.Net.WebSockets;
using Discord.Rest;

namespace Discord.WebSocket
{
    public class DiscordSocketConfig : DiscordRestConfig
    {
        public const string GatewayEncoding = "json";

        /// <summary> Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting. </summary>
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary> Gets or sets the id for this shard. Must be less than TotalShards. </summary>
        public int ShardId { get; set; } = 0;
        /// <summary> Gets or sets the total number of shards for this application. </summary>
        public int TotalShards { get; set; } = 1;

        /// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
        public int MessageCacheSize { get; set; } = 0;
        /// <summary> 
        /// Gets or sets the max number of users a guild may have for offline users to be included in the READY packet. Max is 250.
        /// </summary>
        public int LargeThreshold { get; set; } = 250;

        /// <summary> Gets or sets the type of audio this DiscordClient supports. </summary>
        public AudioMode AudioMode { get; set; } = AudioMode.Disabled;

        /// <summary> Gets or sets the provider used to generate new websocket connections. </summary>
        public WebSocketProvider WebSocketProvider { get; set; } = () => new DefaultWebSocketClient();
    }
}
