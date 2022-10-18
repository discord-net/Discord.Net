using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class RoleTags
    {
        [JsonPropertyName("bot_id")]
        public Optional<ulong> BotId { get; set; }
        [JsonPropertyName("integration_id")]
        public Optional<ulong> IntegrationId { get; set; }
        [JsonPropertyName("premium_subscriber")]
        public Optional<bool?> IsPremiumSubscriber { get; set; }
    }
}
