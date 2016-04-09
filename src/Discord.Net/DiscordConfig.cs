using Discord.Net.Rest;
using Discord.Net.WebSockets;
using System.Reflection;

namespace Discord
{	
    public class DiscordConfig
    {
        public const int MaxMessageSize = 2000;
        public const int MaxMessagesPerBatch = 100;

        public const string LibName = "Discord.Net";
        public static string LibVersion { get; } = typeof(DiscordConfig).GetTypeInfo().Assembly?.GetName().Version.ToString(3) ?? "Unknown";
        public const string LibUrl = "https://github.com/RogueException/Discord.Net";

        public const string ClientAPIUrl = "https://discordapp.com/api/";
        public const string CDNUrl = "https://cdn.discordapp.com/";
        public const string InviteUrl = "https://discord.gg/";

        internal const int RestTimeout = 10000;
        internal const int MessageQueueInterval = 100;
        internal const int WebSocketQueueInterval = 100;

        /// <summary> Gets or sets the minimum log level severity that will be sent to the LogMessage event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
        
        /// <summary> Gets or sets the time (in milliseconds) to wait for the websocket to connect and initialize. </summary>
        public int ConnectionTimeout { get; set; } = 30000;
        /// <summary> Gets or sets the time (in milliseconds) to wait after an unexpected disconnect before reconnecting. </summary>
        public int ReconnectDelay { get; set; } = 1000;
        /// <summary> Gets or sets the time (in milliseconds) to wait after an reconnect fails before retrying. </summary>
        public int FailedReconnectDelay { get; set; } = 15000;

        //Performance
        
        /// <summary> Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero disables the message cache entirely. </summary>
        public int MessageCacheSize { get; set; } = 100;
        /// <summary> 
        /// Gets or sets whether the permissions cache should be used. 
        /// This makes operations such as User.GetPermissions(Channel), User.GuildPermissions, Channel.GetUser, and Channel.Members much faster while increasing memory usage.
        /// </summary>
        public bool UsePermissionsCache { get; set; } = true;
        /// <summary> Gets or sets whether the a copy of a model is generated on an update event to allow you to check which properties changed. </summary>
        public bool EnablePreUpdateEvents { get; set; } = true;
        /// <summary> 
        /// Gets or sets the max number of users a guild may have for offline users to be included in the READY packet. Max is 250. 
        /// Decreasing this may reduce CPU usage while increasing login time and network usage. 
        /// </summary>
        public int LargeThreshold { get; set; } = 250;

        //Engines

        /// <summary> Gets or sets the REST engine to use. Defaults to DefaultRestClientProvider, which is built around .Net's HttpClient class. </summary>
        public RestClientProvider RestClientProvider { get; set; } = (url, ct) => new DefaultRestEngine(url, ct);
        /// <summary> 
        /// Gets or sets the WebSocket engine to use. Defaults to DefaultWebSocketProvider, which uses .Net's WebSocketClient class. 
        /// WebSockets are only used if DiscordClient.Connect() is called.
        /// </summary>
        public WebSocketProvider WebSocketProvider { get; set; } = null;
    }
}

