using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Ratelimit
{
    [JsonPropertyName("global")]
    public bool Global { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("retry_after")]
    public double RetryAfter { get; set; }
}
