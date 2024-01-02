using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class StartThreadParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

    [JsonPropertyName("type")]
    public required ChannelType Type { get; set; }

    [JsonPropertyName("invitable")]
    public Optional<bool> IsInvitable { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> RateLimitPerUser { get; set; }

}
