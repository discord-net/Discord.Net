using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a embed author object.
    /// </summary>
    public record EmbedAuthor
    {
        /// <summary>
        ///     Creates a <see cref="EmbedAuthor"/> with the provided parameters.
        /// </summary>
        /// <param name="name">Name of author.</param>
        /// <param name="url">Url of author.</param>
        /// <param name="iconUrl">Url of author icon (only supports http(s) and attachments).</param>
        /// <param name="proxyIconUrl">A proxied url of author icon.</param>
        [JsonConstructor]
        public EmbedAuthor(Optional<string> name, Optional<string> url, Optional<string> iconUrl, Optional<string> proxyIconUrl)
        {
            Name = name;
            Url = url;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }

        /// <summary>
        ///     Name of author.
        /// </summary>
        [JsonPropertyName("name")]
        public Optional<string> Name { get; }

        /// <summary>
        ///     Url of author.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }

        /// <summary>
        ///     Url of author icon (only supports http(s) and attachments).
        /// </summary>
        [JsonPropertyName("icon_url")]
        public Optional<string> IconUrl { get; }

        /// <summary>
        ///     A proxied url of author icon.
        /// </summary>
        [JsonPropertyName("proxy_icon_url")]
        public Optional<string> ProxyIconUrl { get; }
    }
}
