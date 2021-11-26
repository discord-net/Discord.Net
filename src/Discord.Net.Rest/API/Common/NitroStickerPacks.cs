using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class NitroStickerPacks
    {
        [JsonProperty("sticker_packs")]
        public List<StickerPack> StickerPacks { get; set; }
    }
}
