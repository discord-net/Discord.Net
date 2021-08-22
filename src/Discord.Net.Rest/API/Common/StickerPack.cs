using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class StickerPack
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("stickers")]
        public Sticker[] Stickers { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("sku_id")]
        public ulong SkuId { get; set; }
        [JsonProperty("cover_sticker_id")]
        public Optional<ulong> CoverStickerId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("banner_asset_id")]
        public ulong BannerAssetId { get; set; }
    }
}
