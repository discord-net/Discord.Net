using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateChannelInviteParams
    {
        [JsonProperty("max_age")]
        public Optional<int> MaxAge { get; set; }
        [JsonProperty("max_uses")]
        public Optional<int> MaxUses { get; set; }
        [JsonProperty("temporary")]
        public Optional<bool> Temporary { get; set; }
        [JsonProperty("xkcdpass")]
        public Optional<bool> XkcdPass { get; set; }
    }
}
