using Discord.Net.Rest;

namespace Discord
{
    public class DiscordRestConfig : DiscordConfig
    {
        public static string UserAgent { get; } = $"DiscordBot (https://github.com/RogueException/Discord.Net, v{Version})";

        public const int MaxMessageSize = 2000;
        public const int MaxMessagesPerBatch = 100;
        public const int MaxUsersPerBatch = 1000;

        internal const int RestTimeout = 10000;
        internal const int MessageQueueInterval = 100;
        internal const int WebSocketQueueInterval = 100;

        /// <summary> Gets or sets the provider used to generate new REST connections. </summary>
        public RestClientProvider RestClientProvider { get; set; } = url => new DefaultRestClient(url);
    }
}
