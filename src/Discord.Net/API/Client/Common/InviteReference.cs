using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class InviteReference
    {
        public class GuildData : GuildReference
        {
            [JsonProperty("splash_hash")]
            public string Splash { get; set; }
        }

        [JsonProperty("guild")]
        public GuildData Guild { get; set; }
        [JsonProperty("channel")]
        public ChannelReference Channel { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("xkcdpass")]
        public string XkcdPass { get; set; }
    }
}
