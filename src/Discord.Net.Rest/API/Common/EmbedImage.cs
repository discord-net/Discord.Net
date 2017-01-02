#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class EmbedImage
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public Optional<int> Height { get; set; }
        [JsonProperty("width")]
        public Optional<int> Width { get; set; }
    }
}
