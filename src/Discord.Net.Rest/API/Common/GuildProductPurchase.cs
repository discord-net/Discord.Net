using Newtonsoft.Json;

namespace Discord.API;

internal class GuildProductPurchase
{
    [JsonProperty("listing_id")]
    public ulong ListingId { get; set; }

    [JsonProperty("product_name")]
    public string ProductName { get; set; }
}
