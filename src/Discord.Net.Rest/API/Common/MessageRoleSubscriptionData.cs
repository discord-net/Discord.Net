using Newtonsoft.Json;

namespace Discord.API;

internal class MessageRoleSubscriptionData
{
    [JsonProperty("role_subscription_listing_id")]
    public ulong SubscriptionListingId { get; set; }

    [JsonProperty("tier_name")]
    public string TierName { get; set; }

    [JsonProperty("total_months_subscribed")]
    public int MonthsSubscribed { get; set; }

    [JsonProperty("is_renewal")]
    public bool IsRenewal { get; set; }
}
