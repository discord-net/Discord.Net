#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateGuildEmoteParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("image")]
        public Image Image { get; set; }
        [JsonProperty("roles")]
        public Optional<ulong[]> RoleIds { get; set; }
    }
}
