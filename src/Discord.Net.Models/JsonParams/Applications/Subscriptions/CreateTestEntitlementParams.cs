using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class CreateTestEntitlementParams
{
    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }

    [JsonPropertyName("owner_id")]
    public ulong OwnerId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }
}
