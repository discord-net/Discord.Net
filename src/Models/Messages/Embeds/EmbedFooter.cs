using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord embed footer object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-footer-structure"/>
    /// </remarks>
    public record EmbedFooter
    {
        /// <summary>
        /// Footer text.
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; init; } // Required property candidate

        /// <summary>
        /// Url of footer icon (only supports http(s) and attachments).
        /// </summary>
        [JsonPropertyName("icon_url")]
        public Optional<string> IconUrl { get; init; }

        /// <summary>
        /// A proxied url of footer icon.
        /// </summary>
        [JsonPropertyName("proxy_icon_url")]
        public Optional<string> ProxyIconUrl { get; init; }
    }
}
