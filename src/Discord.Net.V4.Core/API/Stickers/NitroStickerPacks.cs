using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class NitroStickerPacks
{
    [JsonPropertyName("sticker_packs")]
    public required StickerPack[] StickerPacks { get; set; }
}
