using Discord.Rest;

namespace Discord.API.AuditLogs;

internal class ChannelInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("name")]
    public string Name { get; set; }

    [JsonField("type")]
    public ChannelType? Type { get; set; }

    [JsonField("permission_overwrites")]
    public Overwrite[] Overwrites { get; set; }

    [JsonField("flags")]
    public ChannelFlags? Flags { get; set; }

    [JsonField("default_thread_rate_limit_per_user")]
    public int? DefaultThreadRateLimitPerUser { get; set; }

    [JsonField("default_auto_archive_duration")]
    public ThreadArchiveDuration? DefaultArchiveDuration { get; set; }

    [JsonField("rate_limit_per_user")]
    public int? RateLimitPerUser { get; set; }

    [JsonField("auto_archive_duration")]
    public ThreadArchiveDuration? AutoArchiveDuration { get; set; }

    [JsonField("nsfw")]
    public bool? IsNsfw { get; set; }

    [JsonField("topic")]
    public string Topic { get; set; }

    // Forum channels
    [JsonField("available_tags")]
    public ForumTag[] AvailableTags { get; set; }

    [JsonField("default_reaction_emoji")]
    public ForumReactionEmoji DefaultEmoji { get; set; }

    // Voice channels

    [JsonField("user_limit")]
    public int? UserLimit { get; set; }

    [JsonField("rtc_region")]
    public string Region { get; set; }

    [JsonField("video_quality_mode")]
    public VideoQualityMode? VideoQualityMode { get; set; }

    [JsonField("bitrate")]
    public int? Bitrate { get; set; }
}
