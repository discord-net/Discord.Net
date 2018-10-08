using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Discord.Rest;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a configuration class for <see cref="DiscordSocketClient"/>.
    /// </summary>
    /// <remarks>
    ///     This configuration, based on <see cref="DiscordRestConfig"/>, helps determine several key configurations the
    ///     socket client depend on. For instance, shards and connection timeout.
    /// </remarks>
    /// <example>
    ///     The following config enables the message cache and configures the client to always download user upon guild
    ///     availability.
    ///     <code language="cs">
    ///     var config = new DiscordSocketConfig
    ///     {
    ///         AlwaysDownloadUsers = true,
    ///         MessageCacheSize = 100
    ///     };
    ///     var client = new DiscordSocketClient(config);
    ///     </code>
    /// </example>
    public class DiscordSocketConfig : DiscordRestConfig
    {
        /// <summary>
        ///     Gets or sets the encoding gateway should use.
        /// </summary>
        public const string GatewayEncoding = "json";

        /// <summary>
        ///     Gets or sets the WebSocket host to connect to. If <c>null</c>, the client will use the
        ///     /gateway endpoint.
        /// </summary>
        public string GatewayHost { get; set; } = null;

        /// <summary>
        ///     Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary>
        ///     Gets or sets the ID for this shard. Must be less than <see cref="TotalShards"/>.
        /// </summary>
        public int? ShardId { get; set; } = null;
        /// <summary>
        ///     Gets or sets the total number of shards for this application.
        /// </summary>
        public int? TotalShards { get; set; } = null;

        /// <summary>
        ///     Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero
        ///     disables the message cache entirely.
        /// </summary>
        public int MessageCacheSize { get; set; } = 0;
        /// <summary>
        ///     Gets or sets the max number of users a guild may have for offline users to be included in the READY
        ///     packet. Max is 250.
        /// </summary>
        public int LargeThreshold { get; set; } = 250;

        /// <summary>
        ///     Gets or sets the provider used to generate new WebSocket connections.
        /// </summary>
        public WebSocketProvider WebSocketProvider { get; set; }
        /// <summary>
        ///     Gets or sets the provider used to generate new UDP sockets.
        /// </summary>
        public UdpSocketProvider UdpSocketProvider { get; set; }

        /// <summary>
        ///     Gets or sets whether or not all users should be downloaded as guilds come available.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         By default, Discord gateway will only send offline members if a guild has less than a certain number
        ///         of members (determined by <see cref="LargeThreshold"/> in this library). This behaviour is why
        ///         sometimes a user may be missing from the WebSocket cache for collections such as 
        ///         <see cref="Discord.WebSocket.SocketGuild.Users"/>.
        ///     </para>
        ///     <para>
        ///         This property ensures that whenever a guild becomes available (determined by
        ///         <see cref="Discord.WebSocket.BaseSocketClient.GuildAvailable"/>), incomplete user chunks will be
        ///         downloaded to the WebSocket cache.
        ///     </para>
        ///     <para>
        ///         For more information, please see 
        ///         <see href="https://discordapp.com/developers/docs/topics/gateway#request-guild-members">Request Guild Members</see>
        ///         on the official Discord API documentation.
        ///     </para>
        ///     <note>
        ///         Please note that it can be difficult to fill the cache completely on large guilds depending on the
        ///         traffic. If you are using the command system, the default user TypeReader may fail to find the user
        ///         due to this issue. This may be resolved at v3 of the library. Until then, you may want to consider
        ///         overriding the TypeReader and use <see cref="DiscordRestClient.GetGuildUserAsync"/> as a backup.
        ///     </note>
        /// </remarks>
        public bool AlwaysDownloadUsers { get; set; } = false;
        /// <summary>
        ///     Gets or sets the timeout for event handlers, in milliseconds, after which a warning will be logged. Null
        ///     disables this check.
        /// </summary>
        public int? HandlerTimeout { get; set; } = 3000;

        /// <summary>
        ///     Initializes a default configuration.
        /// </summary>
        public DiscordSocketConfig()
        {
            WebSocketProvider = DefaultWebSocketProvider.Instance;
            UdpSocketProvider = DefaultUdpSocketProvider.Instance;
        }

        internal DiscordSocketConfig Clone() => MemberwiseClone() as DiscordSocketConfig;
    }
}
