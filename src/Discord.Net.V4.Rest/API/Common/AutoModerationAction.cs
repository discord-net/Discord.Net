using System.Text.Json.Serialization;

namespace Discord.API;

public class AutoModerationAction
{
    [JsonPropertyName("type")]
    public AutoModActionType Type { get; set; }

    [JsonPropertyName("metadata")]
    public Optional<ActionMetadata> Metadata { get; set; }
}
