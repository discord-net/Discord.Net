using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyCurrentUserParams
    {
        [JsonPropertyName("username")]
        public Optional<string> Username { get; set; }
        [JsonPropertyName("avatar")]
        public Optional<Image?> Avatar { get; set; }
    }
}
