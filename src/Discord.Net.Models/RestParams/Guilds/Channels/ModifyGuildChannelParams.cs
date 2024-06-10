using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyGuildChannelParams : ModifyChannelParams
{
    [JsonPropertyName("type")]
    public Optional<int> Type { get; set; }

    [JsonPropertyName("position")]
    public Optional<int?> Position { get; set; }

    [JsonPropertyName("topic")]
    public Optional<string?> Topic { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool?> Nsfw { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> RateLimitPerUser { get; set; }

    [JsonPropertyName("bitrate")]
    public Optional<int?> Bitrate { get; set; }

    [JsonPropertyName("user_limit")]
    public Optional<int?> UserLimit { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public Optional<Overwrite[]> PermissionOverwrites { get; set; }

    [JsonPropertyName("parent_id")]
    public Optional<ulong?> ParentId { get; set; }

    [JsonPropertyName("rtc_region")]
    public Optional<string?> RtcRegion { get; set; }

    [JsonPropertyName("video_quality_mode")]
    public Optional<int?> VideoQualityMode { get; set; }

    [JsonPropertyName("default_auto_archive_duration")]
    public Optional<int?> DefaultAutoArchiveDuration { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("available_tags")]
    public Optional<ForumTag[]> AvailableTags { get; set; }

    [JsonPropertyName("default_reaction_emoji")]
    public Optional<DefaultReaction?> DefaultReactionEmoji { get; set; }

    [JsonPropertyName("default_thread_rate_limit_per_user")]
    public Optional<int?> DefaultThreadRateLimitPerUser { get; set; }

    [JsonPropertyName("default_sort_order")]
    public Optional<int?> DefaultForumSortOrder { get; set; }

    [JsonPropertyName("default_forum_layout")]
    public Optional<int> DefaultForumLayout { get; set; }
}
