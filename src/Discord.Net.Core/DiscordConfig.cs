using System.Reflection;

namespace Discord
{
    /// <summary>
    ///     Defines various behaviors of Discord.Net.
    /// </summary>
    public class DiscordConfig
    {
        /// <summary> 
        ///     Returns the API version Discord.Net uses. 
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the API version that Discord.Net uses to communicate with Discord.
        ///     <para>A list of available API version can be seen on the official 
        ///     <see href="https://discordapp.com/developers/docs/reference#api-versioning">Discord API documentation</see>
        ///     .</para>
        /// </returns>
        public const int APIVersion = 6;
        /// <summary>
        /// Returns the Voice API version Discord.Net uses.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the API version that Discord.Net uses to communicate with Discord's
        ///     voice server.
        /// </returns>
        public const int VoiceAPIVersion = 3;
        /// <summary>
        ///     Gets the Discord.Net version, including the build number.
        /// </summary>
        /// <returns>
        ///     A string containing the detailed version information, including its build number; <c>Unknown</c> when
        ///     the version fails to be fetched.
        /// </returns>
        public static string Version { get; } =
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3) ??
            "Unknown";
        
        /// <summary>
        ///     Gets the user agent that Discord.Net uses in its clients.
        /// </summary>
        /// <returns>
        ///     The user agent used in each Discord.Net request.
        /// </returns>
        public static string UserAgent { get; } = $"DiscordBot (https://github.com/RogueException/Discord.Net, v{Version})";
        /// <summary>
        ///     Returns the base Discord API URL.
        /// </summary>
        /// <returns>
        ///     The Discord API URL using <see cref="APIVersion"/>.
        /// </returns>
        public static readonly string APIUrl = $"https://discordapp.com/api/v{APIVersion}/";
        /// <summary> 
        ///     Returns the base Discord CDN URL. 
        /// </summary>
        /// <returns>
        ///     The base Discord Content Delivery Network (CDN) URL.
        /// </returns>
        public const string CDNUrl = "https://cdn.discordapp.com/";
        /// <summary> 
        ///     Returns the base Discord invite URL.
        /// </summary>
        /// <returns>
        ///     The base Discord invite URL.
        /// </returns>
        public const string InviteUrl = "https://discord.gg/";

        /// <summary> 
        ///     Returns the default timeout for requests. 
        /// </summary>
        /// <returns>
        ///     The amount of time it takes in milliseconds before a request is timed out.
        /// </returns>
        public const int DefaultRequestTimeout = 15000;
        /// <summary> 
        ///     Returns the max length for a Discord message. 
        /// </summary>
        /// <returns>
        ///     The maximum length of a message allowed by Discord.
        /// </returns>
        public const int MaxMessageSize = 2000;
        /// <summary> 
        ///     Returns the max messages allowed to be in a request. 
        /// </summary>
        /// <returns>
        ///     The maximum number of messages that can be gotten per-batch.
        /// </returns>
        public const int MaxMessagesPerBatch = 100;
        /// <summary> 
        ///     Returns the max users allowed to be in a request.
        /// </summary>
        /// <returns>
        ///     The maximum number of users that can be gotten per-batch.
        /// </returns>
        public const int MaxUsersPerBatch = 1000;
        /// <summary> 
        ///     Returns the max guilds allowed to be in a request. 
        /// </summary>
        /// <returns>
        ///     The maximum number of guilds that can be gotten per-batch.
        /// </returns>
        public const int MaxGuildsPerBatch = 100;
        /// <summary>
        ///     Returns the max user reactions allowed to be in a request.
        /// </summary>
        /// <returns>
        ///     The maximum number of user reactions that can be gotten per-batch.
        /// </returns>
        public const int MaxUserReactionsPerBatch = 100;
        /// <summary> 
        ///     Returns the max audit log entries allowed to be in a request. 
        /// </summary>
        /// <returns>
        ///     The maximum number of audit log entries that can be gotten per-batch.
        /// </returns>
        public const int MaxAuditLogEntriesPerBatch = 100;

        /// <summary>
        ///     Gets or sets how a request should act in the case of an error, by default.
        /// </summary>
        /// <returns>
        ///     The currently set <see cref="RetryMode"/>.
        /// </returns>
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;
        
        /// <summary>
        ///     Gets or sets the minimum log level severity that will be sent to the Log event.
        /// </summary>
        /// <returns>
        ///     The currently set <see cref="LogSeverity"/> for logging level.
        /// </returns>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary>
        ///     Gets or sets whether the initial log entry should be printed.
        /// </summary>
        /// <remarks>
        ///     If set to <c>true</c>, the library will attempt to print the current version of the library, as well as
        ///     the API version it uses on startup.
        /// </remarks>
        internal bool DisplayInitialLog { get; set; } = true;
    }
}
