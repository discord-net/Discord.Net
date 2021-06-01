using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a embed thumbnail object.
    /// </summary>
    public record EmbedThumbnail
    {
        /// <summary>
        ///     Creates a <see cref="EmbedThumbnail"/> with the provided parameters.
        /// </summary>
        /// <param name="url">Source url of thumbnail (only supports http(s) and attachments).</param>
        /// <param name="proxyUrl">A proxied url of the thumbnail.</param>
        /// <param name="height">Height of thumbnail.</param>
        /// <param name="width">Width of thumbnail.</param>
        [JsonConstructor]
        public EmbedThumbnail(Optional<string> url, Optional<string> proxyUrl, Optional<int> height, Optional<int> width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }

        /// <summary>
        ///     Source url of thumbnail (only supports http(s) and attachments).
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }

        /// <summary>
        ///     A proxied url of the thumbnail.
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public Optional<string> ProxyUrl { get; }

        /// <summary>
        ///     Height of thumbnail.
        /// </summary>
        [JsonPropertyName("height")]
        public Optional<int> Height { get; }

        /// <summary>
        ///     Width of thumbnail.
        /// </summary>
        [JsonPropertyName("width")]
        public Optional<int> Width { get; }
    }
}
