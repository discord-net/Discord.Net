using Discord.Net.Rest;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a configuration class for <see cref="DiscordRestClient"/>.
    /// </summary>
    public class DiscordRestConfig : DiscordConfig
    {
        /// <summary> Gets or sets the provider used to generate new REST connections. </summary>
        public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
    }
}
