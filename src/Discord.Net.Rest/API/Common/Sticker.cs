#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Sticker
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("pack_id")]
        public ulong PackId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Desription { get; set; }
        [JsonProperty("tags")]
        public Optional<string> Tags { get; set; }
        [JsonProperty("asset")]
        public string Asset { get; set; }
        [JsonProperty("preview_asset")]
        public string PreviewAsset { get; set; }
        [JsonProperty("format_type")]
        public StickerFormatType FormatType { get; set; }
    }
}
