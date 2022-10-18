using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildEmoteParams
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("image")]
        public Image Image { get; set; }
        [JsonPropertyName("roles")]
        public Optional<ulong[]> RoleIds { get; set; }
    }
}
