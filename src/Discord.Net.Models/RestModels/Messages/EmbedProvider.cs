using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class EmbedProvider
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }
}
