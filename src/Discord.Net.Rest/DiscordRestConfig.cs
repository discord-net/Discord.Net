using Discord.Net.Rest;

namespace Discord.Rest
{
    public class DiscordRestConfig : DiscordConfig
    {
        /// <summary> Gets or sets the provider used to generate new REST connections. </summary>
        public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
    }
}
