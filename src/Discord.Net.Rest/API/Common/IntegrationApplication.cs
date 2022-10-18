using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class IntegrationApplication
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("icon")]
        public Optional<string> Icon { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("summary")]
        public string Summary { get; set; }
        [JsonPropertyName("bot")]
        public Optional<User> Bot { get; set; }
    }
}
