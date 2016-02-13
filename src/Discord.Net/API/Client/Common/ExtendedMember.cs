using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class ExtendedMember : Member
    {
        [JsonProperty("mute")]
        public bool? IsServerMuted { get; set; }
        [JsonProperty("deaf")]
        public bool? IsServerDeafened { get; set; }
    }
}
