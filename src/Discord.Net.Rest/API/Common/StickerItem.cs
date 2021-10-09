using Newtonsoft.Json;

namespace Discord.API
{
    internal class StickerItem
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("format_type")]
        public StickerFormatType FormatType { get; set; }
    }
}
