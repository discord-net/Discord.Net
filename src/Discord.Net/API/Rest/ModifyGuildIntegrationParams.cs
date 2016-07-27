using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildIntegrationParams
    {
        [JsonProperty("expire_behavior")]
        internal Optional<int> _expireBehavior;
        public int ExpireBehavior { set { _expireBehavior = value; } }

        [JsonProperty("expire_grace_period")]
        internal Optional<int> _expireGracePeriod;
        public int ExpireGracePeriod { set { _expireGracePeriod = value; } }

        [JsonProperty("enable_emoticons")]
        internal Optional<bool> _enableEmoticons;
        public bool EnableEmoticons { set { _enableEmoticons = value; } }
    }
}
