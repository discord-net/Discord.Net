using System.Reflection;

namespace Discord
{
    /// <summary> Stores common configuration settings </summary>
    public class DiscordConfig
    {
        /// <summary> The version of Discord's REST API which is used </summary>
        public const int APIVersion = 6;
        /// <summary> Version information about Discord.Net </summary>
        public static string Version { get; } =
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3) ?? 
            "Unknown";

        /// <summary> The base URL for all REST API requests </summary>
        public static readonly string ClientAPIUrl = $"https://discordapp.com/api/v{APIVersion}/";
        /// <summary> The base URL for all CDN requests </summary>
        public const string CDNUrl = "https://discordcdn.com/";
        /// <summary> The base URL for all invite links </summary>
        public const string InviteUrl = "https://discord.gg/";

        /// <summary> The maximum amount of characters which can be sent in a message </summary>
        public const int MaxMessageSize = 2000;
        /// <summary> The maximum number of messages which can be received in a batch </summary>
        public const int MaxMessagesPerBatch = 100;
        /// <summary> The maximum number of users which can be received in a batch </summary>
        public const int MaxUsersPerBatch = 1000;

        /// <summary> Gets or sets the minimum log level severity that will be sent to the LogMessage event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
    }
}
