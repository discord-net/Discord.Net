#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Attachment
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
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
