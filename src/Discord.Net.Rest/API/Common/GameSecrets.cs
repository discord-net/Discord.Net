using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class GameSecrets
    {
        [JsonPropertyName("match")]
        public string Match { get; set; }
        [JsonPropertyName("join")]
        public string Join { get; set; }
        [JsonPropertyName("spectate")]
        public string Spectate { get; set; }
    }
}
