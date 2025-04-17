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

        [JsonProperty("banner")]
        public Optional<Image?> Banner { get; set; }
    }
}
