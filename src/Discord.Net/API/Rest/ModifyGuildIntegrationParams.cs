using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildIntegrationParams
    {
        [JsonProperty("expire_behavior")]
        public Optional<int> ExpireBehavior { get; set; }
        [JsonProperty("expire_grace_period")]
        public Optional<int> ExpireGracePeriod { get; set; }
        [JsonProperty("enable_emoticons")]
        public Optional<bool> EnableEmoticons { get; set; }
    }
}
