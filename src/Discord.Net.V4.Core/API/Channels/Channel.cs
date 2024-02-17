using Discord.Entities.Channels.Threads;
using System.Text.Json.Serialization;

namespace Discord.API;

public class Channel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public ChannelType Type { get; set; }

    // [JsonPropertyName("last_message_id")]
    // public Optional<ulong?> LastMessageId { get; set; }
    //
    // //GuildChannel
    // [JsonPropertyName("guild_id")]
    // public Optional<ulong> GuildId { get; set; }
    //
    // [JsonPropertyName("position")]
    // public Optional<int> Position { get; set; }
    //
    // [JsonPropertyName("permission_overwrites")]
    // public Optional<Overwrite[]> PermissionOverwrites { get; set; }
    //
    // [JsonPropertyName("name")]
    // public Optional<string?> Name { get; set; }
    //
    // [JsonPropertyName("parent_id")]
    // public Optional<ulong?> ParentId { get; set; }
    //
    // //TextChannel
    // [JsonPropertyName("topic")]
    // public Optional<string?> Topic { get; set; }
    //
    // [JsonPropertyName("last_pin_timestamp")]
    // public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }
    //
    // [JsonPropertyName("nsfw")]
    // public Optional<bool> Nsfw { get; set; }
    //
    // [JsonPropertyName("rate_limit_per_user")]
    // public Optional<int> SlowMode { get; set; }
    //
    // //VoiceChannel
    // [JsonPropertyName("bitrate")]
    // public Optional<int> Bitrate { get; set; }
    //
    // [JsonPropertyName("user_limit")]
    // public Optional<int> UserLimit { get; set; }
    //
    // [JsonPropertyName("rtc_region")]
    // public Optional<string?> RTCRegion { get; set; }
    //
    // [JsonPropertyName("video_quality_mode")]
    // public Optional<VideoQualityMode> VideoQualityMode { get; set; }
    //
    // //PrivateChannel
    // [JsonPropertyName("recipients")]
    // public Optional<User[]> Recipients { get; set; }
    //
    // //GroupChannel
    // [JsonPropertyName("icon")]
    // public Optional<string?> Icon { get; set; }
    //
    // [JsonPropertyName("managed")]
    // public Optional<bool> IsManaged { get; set; }
    //
    // //ThreadChannel
    // [JsonPropertyName("member")]
    // public Optional<ThreadMember> ThreadMember { get; set; }
    //
    // [JsonPropertyName("thread_metadata")]
    // public Optional<ThreadMetadata> ThreadMetadata { get; set; }
    //
    // [JsonPropertyName("owner_id")]
    // public Optional<ulong> OwnerId { get; set; }
    //
    // [JsonPropertyName("message_count")]
    // public Optional<int> MessageCount { get; set; }
    //
    // [JsonPropertyName("total_messages_sent")]
    // public Optional<int> TotalMessagesSent { get; set; }
    //
    // [JsonPropertyName("member_count")]
    // public Optional<int> MemberCount { get; set; }
    //
    // [JsonPropertyName("applied_tags")]
    // public Optional<ulong[]> AppliedTags { get; set; }
    //
    // //ForumChannel
    // [JsonPropertyName("available_tags")]
    // public Optional<ForumTag[]> ForumTags { get; set; }
    //
    // [JsonPropertyName("default_auto_archive_duration")]
    // public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }
    //
    // [JsonPropertyName("default_thread_rate_limit_per_user")]
    // public Optional<int> ThreadRateLimitPerUser { get; set; }
    //
    // [JsonPropertyName("flags")]
    // public Optional<ChannelFlags> Flags { get; set; }
    //
    // [JsonPropertyName("default_sort_order")]
    // public Optional<ForumSortOrder?> DefaultSortOrder { get; set; }
    //
    // [JsonPropertyName("default_reaction_emoji")]
    // public Optional<ForumReactionEmoji?> DefaultReactionEmoji { get; set; }
    //
    // [JsonPropertyName("default_forum_layout")]
    // public Optional<ForumLayout> DefaultForumLayout { get; set; }
    //
    // // Resolved slash command data
    // [JsonPropertyName("permissions")]
    // public Optional<ChannelPermission> Permissions { get; set; }
}
