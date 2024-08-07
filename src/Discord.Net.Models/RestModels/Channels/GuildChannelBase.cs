using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public abstract class GuildChannelBase : Channel, IGuildChannelModel, IModelSource, IModelSourceOf<IEmojiModel?>
{
    [JsonPropertyName("last_pin_timestamp")]
    public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }

    [JsonPropertyName("last_message_id")]
    public Optional<ulong?> LastMessageId { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("parent_id")]
    public Optional<ulong?> ParentId { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int> RatelimitPerUser { get; set; }

    [JsonPropertyName("bitrate")]
    public Optional<int> Bitrate { get; set; }

    [JsonPropertyName("user_limit")]
    public Optional<int> UserLimit { get; set; }

    [JsonPropertyName("rtc_region")]
    public Optional<string?> RTCRegion { get; set; }

    [JsonPropertyName("video_quality_mode")]
    public Optional<int> VideoQualityMode { get; set; }

    [JsonPropertyName("permissions")]
    public Optional<ulong> Permissions { get; set; }

    [JsonPropertyName("topic")]
    public Optional<string?> Topic { get; set; }

    [JsonPropertyName("default_auto_archive_duration")]
    public Optional<int> DefaultAutoArchiveDuration { get; set; }

    [JsonPropertyName("default_thread_rate_limit_per_user")]
    public Optional<int> DefaultThreadRatelimitPerUser { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public Optional<Overwrite[]> PermissionOverwrites { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool> Nsfw { get; set; }

    [JsonPropertyName("available_tags")]
    public Optional<ForumTag[]> AvailableTags { get; set; }

    [JsonPropertyName("default_reaction_emoji")]
    public Optional<Emoji> DefaultReactionEmoji { get; set; }

    [JsonPropertyName("default_sort_order")]
    public Optional<int?> DefaultSortOrder { get; set; }

    [JsonPropertyName("default_forum_layout")]
    public Optional<int> DefaultForumLayout { get; set; }

    ulong? IGuildChannelModel.ParentId => ~ParentId;

    IEnumerable<IOverwriteModel> IGuildChannelModel.Permissions => PermissionOverwrites.Or([]);

    int? IGuildChannelModel.Flags => Flags;

    public virtual IEnumerable<IEntityModel> GetDefinedModels()
    {
        if (DefaultReactionEmoji is {IsSpecified: true, Value: not null})
            yield return DefaultReactionEmoji.Value;
    }

    IEmojiModel? IModelSourceOf<IEmojiModel?>.Model => ~DefaultReactionEmoji;
}
