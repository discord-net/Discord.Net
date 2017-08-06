#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyGuildIntegrationParams
    {
        [ModelProperty("expire_behavior")]
        public Optional<int> ExpireBehavior { get; set; }
        [ModelProperty("expire_grace_period")]
        public Optional<int> ExpireGracePeriod { get; set; }
        [ModelProperty("enable_emoticons")]
        public Optional<bool> EnableEmoticons { get; set; }
    }
}
