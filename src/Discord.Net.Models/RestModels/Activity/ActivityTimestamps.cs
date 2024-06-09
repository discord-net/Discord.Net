using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ActivityTimestamps
{
    [JsonPropertyName("start")]
    [JsonConverter(typeof(MillisecondEpocConverter))]
    public Optional<DateTimeOffset> Start { get; set; }

    [JsonPropertyName("end")]
    [JsonConverter(typeof(MillisecondEpocConverter))]
    public Optional<DateTimeOffset> End { get; set; }
}
