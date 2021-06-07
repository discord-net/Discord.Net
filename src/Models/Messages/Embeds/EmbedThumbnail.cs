using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord embed thumbnail object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-thumbnail-structure"/>
    /// </remarks>
    public record EmbedThumbnail
    {
        /// <summary>
        /// Source url of thumbnail (only supports http(s) and attachments).
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; init; }

        /// <summary>
        /// A proxied url of the thumbnail.
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public Optional<string> ProxyUrl { get; init; }

        /// <summary>
        /// Height of thumbnail.
        /// </summary>
        [JsonPropertyName("height")]
        public Optional<int> Height { get; init; }

        /// <summary>
        /// Width of thumbnail.
        /// </summary>
        [JsonPropertyName("width")]
        public Optional<int> Width { get; init; }
    }
}
