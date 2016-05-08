using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateChannelInviteParams
    {
        [JsonProperty("max_age")]
        public int MaxAge { get; set; } = 86400; //24 Hours
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; } = 0;
        [JsonProperty("temporary")]
        public bool Temporary { get; set; } = false;
        [JsonProperty("xkcdpass")]
        public bool XkcdPass { get; set; } = false;
    }
}
