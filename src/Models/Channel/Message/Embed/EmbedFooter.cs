using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a embed footer object.
    /// </summary>
    public record EmbedFooter
    {
        /// <summary>
        ///     Creates a <see cref="EmbedFooter"/> with the provided parameters.
        /// </summary>
        /// <param name="text">Footer text.</param>
        /// <param name="iconUrl">Url of footer icon (only supports http(s) and attachments).</param>
        /// <param name="proxyIconUrl">A proxied url of footer icon.</param>
        [JsonConstructor]
        public EmbedFooter(string text, Optional<string> iconUrl, Optional<string> proxyIconUrl)
        {
            Text = text;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }

        /// <summary>
        ///     Footer text.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; }

        /// <summary>
        ///     Url of footer icon (only supports http(s) and attachments).
        /// </summary>
        [JsonPropertyName("icon_url")]
        public Optional<string> IconUrl { get; }

        /// <summary>
        ///     A proxied url of footer icon.
        /// </summary>
        [JsonPropertyName("proxy_icon_url")]
        public Optional<string> ProxyIconUrl { get; }
    }
}
