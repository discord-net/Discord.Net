using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Discord.API
{
    internal class NitroStickerPacks
    {
        [JsonPropertyName("sticker_packs")]
        public List<StickerPack> StickerPacks { get; set; }
    }
}
