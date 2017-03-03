using Discord.Net.Rest;

namespace Discord.Rest
{
    public class DiscordRestConfig : DiscordConfig
    {
        public static string UserAgent { get; } = $"DiscordBot (https://github.com/RogueException/Discord.Net, v{Version})";
        
        internal const int MessageQueueInterval = 100;
        internal const int WebSocketQueueInterval = 100;

        /// <summary> Gets or sets the provider used to generate new REST connections. </summary>
        public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
    }
}
