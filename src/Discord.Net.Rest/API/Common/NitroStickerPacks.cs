using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class NitroStickerPacks
    {
        [JsonProperty("sticker_packs")]
        public List<StickerPack> StickerPacks { get; set; }
    }
}
