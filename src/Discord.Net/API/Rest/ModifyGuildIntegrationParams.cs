#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildIntegrationParams
    {
        [JsonProperty("expire_behavior")]
        internal Optional<int> _expireBehavior { get; set; }
        public int ExpireBehavior { set { _expireBehavior = value; } }

        [JsonProperty("expire_grace_period")]
        internal Optional<int> _expireGracePeriod { get; set; }
        public int ExpireGracePeriod { set { _expireGracePeriod = value; } }

        [JsonProperty("enable_emoticons")]
        internal Optional<bool> _enableEmoticons { get; set; }
        public bool EnableEmoticons { set { _enableEmoticons = value; } }
    }
}
