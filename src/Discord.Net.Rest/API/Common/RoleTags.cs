using Newtonsoft.Json;

namespace Discord.API;

internal class RoleTags
{
    [JsonProperty("bot_id")]
    public Optional<ulong> BotId { get; set; }

    [JsonProperty("integration_id")]
    public Optional<ulong> IntegrationId { get; set; }

    [JsonProperty("premium_subscriber")]
    public Optional<bool?> IsPremiumSubscriber { get; set; }

    [JsonProperty("subscription_listing_id")]
    public Optional<ulong> SubscriptionListingId { get; set; }

    [JsonProperty("available_for_purchase")]
    public Optional<bool?> IsAvailableForPurchase { get; set; }

    [JsonProperty("guild_connections")]
    public Optional<bool?> GuildConnections { get; set; }
}
