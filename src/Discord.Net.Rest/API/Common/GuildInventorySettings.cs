using Newtonsoft.Json;

namespace Discord.API;

internal class GuildInventorySettings
{
    [JsonProperty("is_emoji_pack_collectible")]
    public Optional<bool> IsEmojiPackCollectible { get; set; }
}
