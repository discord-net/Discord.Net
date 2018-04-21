using System.Reflection;

namespace Discord
{
    /// <summary>
    ///     Defines various behaviors of Discord.Net.
    /// </summary>
    public class DiscordConfig
    {
        /// <summary> 
        ///     Returns the gateway version Discord.Net uses. 
        /// </summary>
        public const int APIVersion = 6;
        /// <summary>
        ///     Gets the Discord.Net version, including the build number.
        /// </summary>
        public static string Version { get; } =
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3) ?? 
            "Unknown";
        
        /// <summary>
        ///     Gets the user agent that Discord.Net uses in its clients.
        /// </summary>
        public static string UserAgent { get; } = $"DiscordBot (https://github.com/RogueException/Discord.Net, v{Version})";
        /// <summary>
        ///     Returns the base Discord API URL.
        /// </summary>
        public static readonly string APIUrl = $"https://discordapp.com/api/v{APIVersion}/";
        /// <summary> 
        ///     Returns the base Discord CDN URL. 
        /// </summary>
        public const string CDNUrl = "https://cdn.discordapp.com/";
        /// <summary> 
        ///     Returns the base Discord invite URL. 
        /// </summary>
        public const string InviteUrl = "https://discord.gg/";

        /// <summary> 
        ///     Returns the default timeout for requests. 
        /// </summary>
        public const int DefaultRequestTimeout = 15000;
        /// <summary> 
        ///     Returns the max length for a Discord message. 
        /// </summary>
        public const int MaxMessageSize = 2000;
        /// <summary> 
        ///     Returns the max messages allowed to be in a request. 
        /// </summary>
        public const int MaxMessagesPerBatch = 100;
        /// <summary> 
        ///     Returns the max users allowed to be in a request.
        /// </summary>
        public const int MaxUsersPerBatch = 1000;
        /// <summary> 
        ///     Returns the max guilds allowed to be in a request. 
        /// </summary>
        public const int MaxGuildsPerBatch = 100;

        /// <summary>
        ///     Gets or sets how a request should act in the case of an error, by default.
        /// </summary>
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;
        
        /// <summary>
        ///     Gets or sets the minimum log level severity that will be sent to the Log event.
        /// </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary>
        ///     Gets or sets whether the initial log entry should be printed.
        /// </summary>
        internal bool DisplayInitialLog { get; set; } = true;
    }
}
