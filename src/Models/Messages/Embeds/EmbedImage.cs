using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord embed image object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-image-structure"/>
    /// </remarks>
    public record EmbedImage
    {
        /// <summary>
        /// Source url of image (only supports http(s) and attachments).
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; init; }

        /// <summary>
        /// A proxied url of the image.
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public Optional<string> ProxyUrl { get; init; }

        /// <summary>
        /// Height of image.
        /// </summary>
        [JsonPropertyName("height")]
        public Optional<int> Height { get; init; }

        /// <summary>
        /// Width of image.
        /// </summary>
        [JsonPropertyName("width")]
        public Optional<int> Width { get; init; }
    }
}
