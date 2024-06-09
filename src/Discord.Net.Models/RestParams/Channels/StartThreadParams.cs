using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class StartThreadParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public Optional<int> AutoArchiveDuration { get; set; }

    [JsonPropertyName("type")]
    public required int Type { get; set; }

    [JsonPropertyName("invitable")]
    public Optional<bool> IsInvitable { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> RateLimitPerUser { get; set; }

}
