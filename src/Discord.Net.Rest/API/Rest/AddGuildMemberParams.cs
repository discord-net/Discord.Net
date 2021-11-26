using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class AddGuildMemberParams
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nickname { get; set; }
        [JsonProperty("roles")]
        public Optional<ulong[]> RoleIds { get; set; }
        [JsonProperty("mute")]
        public Optional<bool> IsMuted { get; set; }
        [JsonProperty("deaf")]
        public Optional<bool> IsDeafened { get; set; }
    }
}
