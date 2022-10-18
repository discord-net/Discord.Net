using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class UserGuild
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("owner")]
        public bool Owner { get; set; }
        [JsonPropertyName("permissions"), Int53]
        public string Permissions { get; set; }
    }
}
