using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a embed image object.
    /// </summary>
    public record EmbedImage
    {
        /// <summary>
        ///     Creates a <see cref="EmbedImage"/> with the provided parameters.
        /// </summary>
        /// <param name="url">Source url of image (only supports http(s) and attachments).</param>
        /// <param name="proxyUrl">A proxied url of the image.</param>
        /// <param name="height">Height of image.</param>
        /// <param name="width">Width of image.</param>
        [JsonConstructor]
        public EmbedImage(Optional<string> url, Optional<string> proxyUrl, Optional<int> height, Optional<int> width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }

        /// <summary>
        ///     Source url of image (only supports http(s) and attachments).
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }

        /// <summary>
        ///     A proxied url of the image.
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public Optional<string> ProxyUrl { get; }

        /// <summary>
        ///     Height of image.
        /// </summary>
        [JsonPropertyName("height")]
        public Optional<int> Height { get; }

        /// <summary>
        ///     Width of image.
        /// </summary>
        [JsonPropertyName("width")]
        public Optional<int> Width { get; }
    }
}
