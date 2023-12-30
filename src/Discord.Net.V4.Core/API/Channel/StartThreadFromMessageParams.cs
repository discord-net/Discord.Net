using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class StartThreadFromMessageParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> RateLimitPerUser { get; set; }
}
