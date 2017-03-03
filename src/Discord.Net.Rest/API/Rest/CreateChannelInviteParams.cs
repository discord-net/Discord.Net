#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateChannelInviteParams
    {
        [JsonProperty("max_age")]
        public Optional<int> MaxAge { get; set; }
        [JsonProperty("max_uses")]
        public Optional<int> MaxUses { get; set; }
        [JsonProperty("temporary")]
        public Optional<bool> IsTemporary { get; set; }
        [JsonProperty("unique")]
        public Optional<bool> IsUnique { get; set; }
    }
}
