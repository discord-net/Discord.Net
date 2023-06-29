using System.Text.Json.Serialization;

namespace Discord.API;

internal class ActivitySecrets
{
    [JsonPropertyName("match")]
    public Optional<string> Match { get; set; }

    [JsonPropertyName("join")]
    public Optional<string> Join { get; set; }

    [JsonPropertyName("spectate")]
    public Optional<string> Spectate { get; set; }
}
