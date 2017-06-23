using System;
using Newtonsoft.Json;

namespace Discord.API
{
    internal class EmbedAuthor
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public Uri Url { get; set; }
        [JsonProperty("icon_url")]
        public Uri IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public Uri ProxyIconUrl { get; set; }
    }
}
