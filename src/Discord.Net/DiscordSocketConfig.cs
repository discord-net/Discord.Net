using Discord.Net.WebSockets;

namespace Discord
{
    public class DiscordSocketConfig : DiscordConfig
    {
        /// <summary> Gets or sets the id for this shard. Must be less than TotalShards. </summary>
        public int ShardId { get; set; } = 0;
        /// <summary> Gets or sets the total number of shards for this application. </summary>
        public int TotalShards { get; set; } = 1;

        /// <summary> Gets or sets the time (in milliseconds) to wait for the websocket to connect and initialize. </summary>
        public int ConnectionTimeout { get; set; } = 30000;
        /// <summary> Gets or sets the time (in milliseconds) to wait after an unexpected disconnect before reconnecting. </summary>
        public int ReconnectDelay { get; set; } = 1000;
        /// <summary> Gets or sets the time (in milliseconds) to wait after an reconnect fails before retrying. </summary>
        public int FailedReconnectDelay { get; set; } = 15000;

        /// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
        public int MessageCacheSize { get; set; } = 0;
        /*/// <summary> 
        /// Gets or sets whether the permissions cache should be used. 
        /// This makes operations such as User.GetPermissions(Channel), User.GuildPermissions, Channel.GetUser, and Channel.Members much faster at the expense of increased memory usage.
        /// </summary>
        public bool UsePermissionsCache { get; set; } = false;*/
        /// <summary> 
        /// Gets or sets the max number of users a guild may have for offline users to be included in the READY packet. Max is 250. 
        /// Decreasing this may reduce CPU usage while increasing login time and network usage. 
        /// </summary>
        public int LargeThreshold { get; set; } = 250;
        
        /// <summary> Gets or sets the provider used to generate new websocket connections. </summary>
        public WebSocketProvider WebSocketProvider { get; set; } = () => new DefaultWebSocketClient();
    }
}
