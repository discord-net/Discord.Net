using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class NitroStickerPacks
{
    [JsonPropertyName("sticker_packs")]
    public required StickerPack[] StickerPacks { get; set; }
}
