using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AutoModerationAction
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("metadata")]
    public Optional<ActionMetadata> Metadata { get; set; }
}
