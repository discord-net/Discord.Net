using System;
using Newtonsoft.Json;

namespace Discord.API
{
    internal class EmbedFooter
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("icon_url")]
        public Uri IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public Uri ProxyIconUrl { get; set; }
    }
}
