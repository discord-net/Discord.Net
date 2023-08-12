using System.Text.Json.Serialization;

namespace Discord.API;

internal class NitroStickerPacks
{
    [JsonPropertyName("sticker_packs")]
    public List<StickerPack> StickerPacks { get; set; }
}
