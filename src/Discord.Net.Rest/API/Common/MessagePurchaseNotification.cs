using Newtonsoft.Json;

namespace Discord.API;

internal class MessagePurchaseNotification
{
    [JsonProperty("type")]
    public PurchaseType Type { get; set; }

    [JsonProperty("guild_product_purchase")]
    public Optional<GuildProductPurchase> ProductPurchase { get; set; }
}
