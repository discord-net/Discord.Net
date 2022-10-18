using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class EmbedImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonPropertyName("height")]
        public Optional<int> Height { get; set; }
        [JsonPropertyName("width")]
        public Optional<int> Width { get; set; }
    }
}
