#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class RoleTags
    {
        [JsonProperty("bot_id")]
        public Optional<ulong> BotId { get; set; }
        [JsonProperty("integration_id")]
        public Optional<ulong> IntegrationId { get; set; }
        [JsonProperty("premium_subscriber")]
        public Optional<bool?> IsPremiumSubscriber { get; set; }
    }
}
