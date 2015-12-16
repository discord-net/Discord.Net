using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class InviteReference
    {
        [JsonProperty("guild")]
        public GuildReference Guild { get; set; }
        [JsonProperty("channel")]
        public ChannelReference Channel { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("xkcdpass")]
        public string XkcdPass { get; set; }
    }
}
