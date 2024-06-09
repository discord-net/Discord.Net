using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ActivityTimestamps
{
    [JsonPropertyName("start")]
    public Optional<long> Start { get; set; }

    [JsonPropertyName("end")]
    public Optional<long> End { get; set; }
}
