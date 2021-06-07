using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord attachment object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#attachment-object-attachment-structure"/>
    /// </remarks>
    public record Attachment
    {
        /// <summary>
        /// <see cref="Attachment"/> id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// Name of file attached.
        /// </summary>
        [JsonPropertyName("filename")]
        public string? Filename { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="Attachment"/>'s media type.
        /// </summary>
        [JsonPropertyName("content_type")]
        public Optional<string> ContentType { get; init; }

        /// <summary>
        /// Size of file in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public int Size { get; init; }

        /// <summary>
        /// Source url of file.
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; init; } // Required property candidate

        /// <summary>
        /// A proxied url of file.
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public string? ProxyUrl { get; init; } // Required property candidate

        /// <summary>
        /// Height of file (if image).
        /// </summary>
        [JsonPropertyName("height")]
        public Optional<int?> Height { get; init; }

        /// <summary>
        /// Width of file (if image).
        /// </summary>
        [JsonPropertyName("width")]
        public Optional<int?> Width { get; init; }
    }
}
