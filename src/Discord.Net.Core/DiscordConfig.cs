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

        public static readonly string ClientAPIUrl = $"https://discordapp.com/api/v{APIVersion}/";
        public const string CDNUrl = "https://discordcdn.com/";
        public const string InviteUrl = "https://discord.gg/";

        public const int MaxMessageSize = 2000;
        public const int MaxMessagesPerBatch = 100;
        public const int MaxUsersPerBatch = 1000;

        /// <summary> Gets or sets the minimum log level severity that will be sent to the LogMessage event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
    }
}
