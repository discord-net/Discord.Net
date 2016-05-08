using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildIntegrationParams
    {
        [JsonProperty("expire_behavior")]
        public int ExpireBehavior { get; set; }
        [JsonProperty("expire_grace_period")]
        public int ExpireGracePeriod { get; set; }
        [JsonProperty("enable_emoticons")]
        public bool EnableEmoticons { get; set; }
    }
}
