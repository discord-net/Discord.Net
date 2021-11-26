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
        public string Description { get; set; }
        [JsonProperty("tags")]
        public Optional<string> Tags { get; set; }
        [JsonProperty("type")]
        public StickerType Type { get; set; }
        [JsonProperty("format_type")]
        public StickerFormatType FormatType { get; set; }
        [JsonProperty("available")]
        public bool? Available { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("user")]
        public Optional<User> User { get; set; }
        [JsonProperty("sort_value")]
        public int? SortValue { get; set; }
    }
}
