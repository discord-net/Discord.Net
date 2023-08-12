using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildChannelParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("type")]
    public ChannelType Type { get; set; }
    [JsonPropertyName("parent_id")]
    public Optional<ulong?> CategoryId { get; set; }
    [JsonPropertyName("position")]
    public Optional<int> Position { get; set; }
    [JsonPropertyName("permission_overwrites")]
    public Optional<Overwrite[]> Overwrites { get; set; }

    //Text channels
    [JsonPropertyName("topic")]
    public Optional<string> Topic { get; set; }
    [JsonPropertyName("nsfw")]
    public Optional<bool> IsNsfw { get; set; }
    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int> SlowModeInterval { get; set; }
    [JsonPropertyName("default_auto_archive_duration")]
    public Optional<ThreadArchiveDuration> DefaultAutoArchiveDuration { get; set; }

    //Voice channels
    [JsonPropertyName("bitrate")]
    public Optional<int> Bitrate { get; set; }
    [JsonPropertyName("user_limit")]
    public Optional<int?> UserLimit { get; set; }
    [JsonPropertyName("video_quality_mode")]
    public Optional<VideoQualityMode> VideoQuality { get; set; }
    [JsonPropertyName("rtc_region")]
    public Optional<string> RtcRegion { get; set; }

    //Forum channels
    [JsonPropertyName("default_reaction_emoji")]
    public Optional<ModifyForumReactionEmojiParams> DefaultReactionEmoji { get; set; }
    [JsonPropertyName("default_thread_rate_limit_per_user")]
    public Optional<int> ThreadRateLimitPerUser { get; set; }
    [JsonPropertyName("available_tags")]
    public Optional<ModifyForumTagParams[]> AvailableTags { get; set; }
    [JsonPropertyName("default_sort_order")]
    public Optional<ForumSortOrder?> DefaultSortOrder { get; set; }

    [JsonPropertyName("default_forum_layout")]
    public Optional<ForumLayout> DefaultLayout { get; set; }
}
