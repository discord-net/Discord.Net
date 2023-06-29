using Newtonsoft.Json;

namespace Discord.API;

internal class MessageRoleSubscriptionData
{
    [JsonPropertyName("role_subscription_listing_id")]
    public ulong SubscriptionListingId { get; set; }

    [JsonPropertyName("tier_name")]
    public string TierName { get; set; }

    [JsonPropertyName("total_months_subscribed")]
    public int MonthsSubscribed { get; set; }

    [JsonPropertyName("is_renewal")]
    public bool IsRenewal { get; set; }
}
