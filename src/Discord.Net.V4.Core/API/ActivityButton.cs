using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ActivityButton
{
    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
