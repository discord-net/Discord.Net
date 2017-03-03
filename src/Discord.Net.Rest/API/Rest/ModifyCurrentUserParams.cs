#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyCurrentUserParams
    {
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }
        [JsonProperty("avatar")]
        public Optional<Image?> Avatar { get; set; }
    }
}
