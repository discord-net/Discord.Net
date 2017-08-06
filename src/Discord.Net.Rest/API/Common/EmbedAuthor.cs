using System;
using Discord.Serialization;

namespace Discord.API
{
    internal class EmbedAuthor
    {
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("url")]
        public string Url { get; set; }
        [ModelProperty("icon_url")]
        public string IconUrl { get; set; }
        [ModelProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }
}
