using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildEmoteParams
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }
        [JsonPropertyName("roles")]
        public Optional<ulong[]> RoleIds { get; set; }
    }
}
