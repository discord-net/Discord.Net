using Newtonsoft.Json;

namespace Discord.API
{
    public class ExtendedMember : GuildMember
    {
        [JsonProperty("mute")]
        public bool? IsMuted { get; set; }
        [JsonProperty("deaf")]
        public bool? IsDeafened { get; set; }
    }
}
