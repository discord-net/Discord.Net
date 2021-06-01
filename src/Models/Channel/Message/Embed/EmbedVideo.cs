using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a embed video object.
    /// </summary>
    public record EmbedVideo
    {
        /// <summary>
        ///     Creates a <see cref="EmbedVideo"/> with the provided parameters.
        /// </summary>
        /// <param name="url">Source url of video.</param>
        /// <param name="proxyUrl">A proxied url of the video.</param>
        /// <param name="height">Height of video.</param>
        /// <param name="width">Width of video.</param>
        [JsonConstructor]
        public EmbedVideo(Optional<string> url, Optional<string> proxyUrl, Optional<int> height, Optional<int> width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }

        /// <summary>
        ///     Source url of video.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }

        /// <summary>
        ///     A proxied url of the video.
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public Optional<string> ProxyUrl { get; }

        /// <summary>
        ///     Height of video.
        /// </summary>
        [JsonPropertyName("height")]
        public Optional<int> Height { get; }

        /// <summary>
        ///     Width of video.
        /// </summary>
        [JsonPropertyName("width")]
        public Optional<int> Width { get; }
    }
}
