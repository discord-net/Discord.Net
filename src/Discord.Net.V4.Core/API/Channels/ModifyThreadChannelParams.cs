using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyThreadChannelParams : ModifyChannelParams
{
    [JsonPropertyName("archived")]
    public Optional<bool> IsArchived { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

    [JsonPropertyName("locked")]
    public Optional<bool> IsLocked { get; set; }

    [JsonPropertyName("invitable")]
    public Optional<bool> IsInvitable { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> RateLimitPerUser { get; set; }

    [JsonPropertyName("flags")]
    public Optional<ChannelFlags> Flags { get; set; }

    [JsonPropertyName("applied_tags")]
    public Optional<ForumTag[]> AppliedTags { get; set; }
}
