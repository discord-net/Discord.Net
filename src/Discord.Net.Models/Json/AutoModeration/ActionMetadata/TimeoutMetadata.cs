using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class TimeoutMetadata : ActionMetadata, ITimeoutMetadataModel
{
    [JsonPropertyName("duration_seconds")]
    public int TimeoutDuration { get; set; }
}
