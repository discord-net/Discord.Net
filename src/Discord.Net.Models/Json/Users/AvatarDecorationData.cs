using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AvatarDecorationData : IAvatarDecorationDataModel
{
    [JsonPropertyName("asset")]
    public required string Asset { get; set; }

    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }
}
