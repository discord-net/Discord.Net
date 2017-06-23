#pragma warning disable CS1591
using System;
using Newtonsoft.Json;

namespace Discord.API
{
    internal class EmbedImage
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }
        [JsonProperty("proxy_url")]
        public Uri ProxyUrl { get; set; }
        [JsonProperty("height")]
        public Optional<int> Height { get; set; }
        [JsonProperty("width")]
        public Optional<int> Width { get; set; }
    }
}
