using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class RoleTags
{
    [JsonPropertyName("bot_id")]
    public Optional<ulong> BotId { get; set; }

    [JsonPropertyName("integration_id")]
    public Optional<ulong> IntegrationId { get; set; }

    [JsonPropertyName("premium_subscriber")]
    public Optional<bool?> IsPremiumSubscriber { get; set; }

    [JsonPropertyName("subscription_listing_id")]
    public Optional<ulong> SubscriptionListingId { get; set; }

    [JsonPropertyName("available_for_purchase")]
    public Optional<bool?> IsAvailableForPurchase { get; set; }

    [JsonPropertyName("guild_connections")]
    public Optional<bool?> HasGuildConnections { get; set; }
}
