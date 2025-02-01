using Newtonsoft.Json;
using System;

namespace Discord.API;

internal class Subscription
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("sku_ids")]
    public ulong[] SKUIds { get; set; }

    [JsonProperty("entitlement_ids")]
    public ulong[] EntitlementIds { get; set; }

    [JsonProperty("renewal_sku_ids")]
    public ulong[] RenewalSKUIds { get; set; }

    [JsonProperty("current_period_start")]
    public DateTimeOffset CurrentPeriodStart { get; set; }

    [JsonProperty("current_period_end")]
    public DateTimeOffset CurrentPeriodEnd { get; set; }

    [JsonProperty("status")]
    public SubscriptionStatus Status { get; set; }

    [JsonProperty("canceled_at")]
    public DateTimeOffset? CanceledAt { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }
}
