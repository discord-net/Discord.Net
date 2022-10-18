using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Sticker
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("pack_id")]
        public ulong PackId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("tags")]
        public Optional<string> Tags { get; set; }
        [JsonPropertyName("type")]
        public StickerType Type { get; set; }
        [JsonPropertyName("format_type")]
        public StickerFormatType FormatType { get; set; }
        [JsonPropertyName("available")]
        public bool? Available { get; set; }
        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }
        [JsonPropertyName("sort_value")]
        public int? SortValue { get; set; }
    }
}
