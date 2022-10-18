using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Emoji
    {
        [JsonPropertyName("id")]
        public ulong? Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("animated")]
        public bool? Animated { get; set; }
        [JsonPropertyName("roles")]
        public ulong[] Roles { get; set; }
        [JsonPropertyName("require_colons")]
        public bool RequireColons { get; set; }
        [JsonPropertyName("managed")]
        public bool Managed { get; set; }
        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }
    }
}
