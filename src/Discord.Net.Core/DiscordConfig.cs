using System.Reflection;

namespace Discord
{
    public class DiscordConfig
    {
        public const int APIVersion = 6;        
        public static string Version { get; } =
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3) ?? 
            "Unknown";

        public static readonly string APIUrl = $"https://discordapp.com/api/v{APIVersion}/";
        public const string CDNUrl = "https://cdn.discordapp.com/";
        public const string InviteUrl = "https://discord.gg/";

        public const int DefaultRequestTimeout = 15000;
        public const int MaxMessageSize = 2000;
        public const int MaxMessagesPerBatch = 100;
        public const int MaxUsersPerBatch = 1000;
        public const int MaxGuildsPerBatch = 100;

        /// <summary> Gets or sets how a request should act in the case of an error, by default. </summary>
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;
        
        /// <summary> Gets or sets the minimum log level severity that will be sent to the Log event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary> Gets or sets whether the initial log entry should be printed. </summary>
        internal bool DisplayInitialLog { get; set; } = true;
    }
}
