using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord embed author object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-author-structure"/>
    /// </remarks>
    public record EmbedAuthor
    {
        /// <summary>
        /// Name of author.
        /// </summary>
        [JsonPropertyName("name")]
        public Optional<string> Name { get; init; }

        /// <summary>
        /// Url of author.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; init; }

        /// <summary>
        /// Url of author icon (only supports http(s) and attachments).
        /// </summary>
        [JsonPropertyName("icon_url")]
        public Optional<string> IconUrl { get; init; }

        /// <summary>
        /// A proxied url of author icon.
        /// </summary>
        [JsonPropertyName("proxy_icon_url")]
        public Optional<string> ProxyIconUrl { get; init; }
    }
}
