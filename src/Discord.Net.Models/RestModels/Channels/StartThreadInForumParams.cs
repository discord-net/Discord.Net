using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class StartThreadInForumParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public Optional<int> AutoArchiveDuration { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> RateLimitPerUser { get; set; }

    [JsonPropertyName("message")]
    public required ForumThreadMessage Message { get; set; }

    [JsonPropertyName("applied_tags")]
    public Optional<ulong[]> AppliedTags { get; set; }
}
