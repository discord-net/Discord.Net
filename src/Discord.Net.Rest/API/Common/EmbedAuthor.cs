using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class EmbedAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }
        [JsonPropertyName("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }
}
