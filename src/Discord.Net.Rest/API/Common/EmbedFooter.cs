using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class EmbedFooter
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }
        [JsonPropertyName("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }
}
