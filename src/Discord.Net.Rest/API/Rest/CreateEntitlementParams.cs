using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class CreateEntitlementParams
{
    [JsonProperty("sku_id")]
    public ulong SkuId { get; set; }

    [JsonProperty("owner_id")]
    public ulong OwnerId { get; set; }

    [JsonProperty("owner_type")]
    public SubscriptionOwnerType Type { get; set; }
}
