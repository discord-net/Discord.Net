using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class AddGuildMemberParams
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("nick")]
        public Optional<string> Nickname { get; set; }
        [JsonPropertyName("roles")]
        public Optional<ulong[]> RoleIds { get; set; }
        [JsonPropertyName("mute")]
        public Optional<bool> IsMuted { get; set; }
        [JsonPropertyName("deaf")]
        public Optional<bool> IsDeafened { get; set; }
    }
}
