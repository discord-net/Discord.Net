using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildStickersUpdated : IGuildStickersUpdatedPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("stickers")]
    public required Sticker[] Stickers { get; set; }

    IEnumerable<IGuildStickerModel> IGuildStickersUpdatedPayloadData.Stickers => Stickers;
}
