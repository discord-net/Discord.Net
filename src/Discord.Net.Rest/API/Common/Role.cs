using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Role
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("icon")]
        public Optional<string> Icon { get; set; }
        [JsonPropertyName("unicode_emoji")]
        public Optional<string> Emoji { get; set; }
        [JsonPropertyName("color")]
        public uint Color { get; set; }
        [JsonPropertyName("hoist")]
        public bool Hoist { get; set; }
        [JsonPropertyName("mentionable")]
        public bool Mentionable { get; set; }
        [JsonPropertyName("position")]
        public int Position { get; set; }
        [JsonPropertyName("permissions"), Int53]
        public string Permissions { get; set; }
        [JsonPropertyName("managed")]
        public bool Managed { get; set; }
        [JsonPropertyName("tags")]
        public Optional<RoleTags> Tags { get; set; }
    }
}
