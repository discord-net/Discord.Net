using Newtonsoft.Json;

namespace Discord.API
{
    public class MemberPresence : MemberReference
    {
        [JsonProperty("game")]
        public MemberPresenceGame Game { get; set; }
        [JsonProperty("status")]
        public UserStatus Status { get; set; }
        [JsonProperty("roles")]
        public ulong[] Roles { get; set; }
    }
}
