using Discord.Net.Rest;

namespace Discord.Rest
{
    /// <summary> A set of common configuration options for REST clients. </summary>
    public class DiscordRestConfig : DiscordConfig
    {
        /// <summary> Gets the user agent used in REST API calls </summary>
        public static string UserAgent { get; } = $"DiscordBot (https://github.com/RogueException/Discord.Net, v{Version})";
        
        internal const int RestTimeout = 10000;
        internal const int MessageQueueInterval = 100;
        internal const int WebSocketQueueInterval = 100;

        /// <summary> Gets or sets the provider used to generate new REST connections. </summary>
        public RestClientProvider RestClientProvider { get; set; } = url => new DefaultRestClient(url);
    }
}
