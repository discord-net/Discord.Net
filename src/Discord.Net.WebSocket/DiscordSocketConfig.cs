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
        ///    Returns the encoding gateway should use.
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
        ///     packet. The maximum value allowed is 250.
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
        ///         By default, the Discord gateway will only send offline members if a guild has less than a certain number
        ///         of members (determined by <see cref="LargeThreshold"/> in this library). This behavior is why
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
        ///         <see href="https://discord.com/developers/docs/topics/gateway#request-guild-members">Request Guild Members</see>
        ///         on the official Discord API documentation.
        ///     </para>
        ///     <note>
        ///         Please note that it can be difficult to fill the cache completely on large guilds depending on the
        ///         traffic. If you are using the command system, the default user TypeReader may fail to find the user
        ///         due to this issue. This may be resolved at v3 of the library. Until then, you may want to consider
        ///         overriding the TypeReader and use
        ///         <see cref="DiscordRestClient.GetUserAsync(ulong,Discord.RequestOptions)"/>
        ///         or <see cref="DiscordRestClient.GetGuildUserAsync"/>
        ///         as a backup.
        ///     </note>
        /// </remarks>
        public bool AlwaysDownloadUsers { get; set; } = false;

        /// <summary>
        ///     Gets or sets the timeout for event handlers, in milliseconds, after which a warning will be logged.
        ///     Setting this property to <c>null</c>disables this check.
        /// </summary>
        public int? HandlerTimeout { get; set; } = 3000;

        /// <summary>
        ///     Gets or sets the maximum identify concurrency.
        /// </summary>
        /// <remarks>
        ///     This information is provided by Discord.
        ///     It is only used when using a <see cref="DiscordShardedClient"/> and auto-sharding is disabled.
        /// </remarks>
        public int IdentifyMaxConcurrency { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the maximum wait time in milliseconds between GUILD_AVAILABLE events before firing READY.
        ///     If zero, READY will fire as soon as it is received and all guilds will be unavailable.
        /// </summary>
        /// <remarks>
        ///     <para>This property is measured in milliseconds; negative values will throw an exception.</para>
        ///     <para>If a guild is not received before READY, it will be unavailable.</para>
        /// </remarks>
        /// <returns>
        ///     A <see cref="int"/> representing the maximum wait time in milliseconds between GUILD_AVAILABLE events
        ///     before firing READY.
        /// </returns>
        /// <exception cref="System.ArgumentException">Value must be at least 0.</exception>
        public int MaxWaitBetweenGuildAvailablesBeforeReady
        {
            get
            {
                return maxWaitForGuildAvailable;
            }

            set
            {
                Preconditions.AtLeast(value, 0, nameof(MaxWaitBetweenGuildAvailablesBeforeReady));
                maxWaitForGuildAvailable = value;
            }
        }

        private int maxWaitForGuildAvailable = 10000;

        /// <summary>
        ///    Gets or sets gateway intents to limit what events are sent from Discord.
        ///    The default is <see cref="GatewayIntents.AllUnprivileged"/>.
        /// </summary>
        /// <remarks>
        ///     For more information, please see
        ///     <see href="https://discord.com/developers/docs/topics/gateway#gateway-intents">GatewayIntents</see>
        ///     on the official Discord API documentation.
        /// </remarks>
        public GatewayIntents GatewayIntents { get; set; } = GatewayIntents.AllUnprivileged;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscordSocketConfig"/> class with the default configuration.
        /// </summary>
        public DiscordSocketConfig()
        {
            WebSocketProvider = DefaultWebSocketProvider.Instance;
            UdpSocketProvider = DefaultUdpSocketProvider.Instance;
        }

        internal DiscordSocketConfig Clone() => MemberwiseClone() as DiscordSocketConfig;
    }
}
