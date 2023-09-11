using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class Ratelimit
{
    [JsonPropertyName("global")]
    public bool Global { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("retry_after")]
    public double RetryAfter { get; set; }
}
