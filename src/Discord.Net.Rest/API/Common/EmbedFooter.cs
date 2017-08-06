using System;
using Discord.Serialization;

namespace Discord.API
{
    internal class EmbedFooter
    {
        [ModelProperty("text")]
        public string Text { get; set; }
        [ModelProperty("icon_url")]
        public string IconUrl { get; set; }
        [ModelProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }
}
