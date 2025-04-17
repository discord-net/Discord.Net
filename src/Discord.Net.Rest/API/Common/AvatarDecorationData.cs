using Newtonsoft.Json;

namespace Discord.API;

internal class AvatarDecorationData
{
    [JsonProperty("asset")]
    public string Asset { get; set; }

    [JsonProperty("sku_id")]
    public ulong SkuId { get; set; }
}
