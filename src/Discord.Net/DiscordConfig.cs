using System.Reflection;

namespace Discord
{
    public class DiscordConfig
    {
        public const int APIVersion = 6;        
        public static string Version { get; } = typeof(DiscordRestConfig).GetTypeInfo().Assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion ?? "Unknown";

        public static readonly string ClientAPIUrl = $"https://discordapp.com/api/v{APIVersion}/";
        public const string CDNUrl = "https://cdn.discordapp.com/";
        public const string InviteUrl = "https://discord.gg/";

        /// <summary> Gets or sets the minimum log level severity that will be sent to the LogMessage event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
    }
}
