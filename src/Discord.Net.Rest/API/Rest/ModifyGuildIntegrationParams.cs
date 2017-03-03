#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildIntegrationParams
    {
        [JsonProperty("expire_behavior")]
        public Optional<int> ExpireBehavior { get; set; }
        [JsonProperty("expire_grace_period")]
        public Optional<int> ExpireGracePeriod { get; set; }
        [JsonProperty("enable_emoticons")]
        public Optional<bool> EnableEmoticons { get; set; }
    }
}
