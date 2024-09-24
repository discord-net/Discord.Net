using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MessageRoleSubscriptionData : IMessageRoleSubscriptionData
{
    [JsonPropertyName("role_subscription_listing_id")]
    public ulong SubscriptionListingId { get; set; }

    [JsonPropertyName("tier_name")]
    public required string TierName { get; set; }

    [JsonPropertyName("total_months_subscribed")]
    public int MonthsSubscribed { get; set; }

    [JsonPropertyName("is_renewal")]
    public bool IsRenewal { get; set; }

    ulong IMessageRoleSubscriptionData.RoleSubscriptionListingId => SubscriptionListingId;

    int IMessageRoleSubscriptionData.TotalMonthsSubscribed => MonthsSubscribed;
}
