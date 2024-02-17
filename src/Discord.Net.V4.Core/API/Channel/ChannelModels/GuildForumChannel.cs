using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.API;

[ChannelTypeOf(ChannelType.Forum)]
public sealed class GuildForumChannel : Channel
{
    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("last_message_id")]
    public Optional<ulong?> LastMessageId { get; set; }

    [JsonPropertyName("flags")]
    public Optional<ChannelFlags> Flags { get; set; }

    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    [JsonPropertyName("parent_id")]
    public Optional<ulong?> ParentId { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int> SlowMode { get; set; }

    [JsonPropertyName("topic")]
    public Optional<string?> Topic { get; set; }

    [JsonPropertyName("position")]
    public Optional<int> Position { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public Optional<Overwrite[]> PermissionOverwrites { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool> Nsfw { get; set; }

    [JsonPropertyName("default_auto_archive_duration")]
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

    [JsonPropertyName("default_thread_rate_limit_per_user")]
    public Optional<int> ThreadRateLimitPerUser { get; set; }

    [JsonPropertyName("available_tags")]
    public Optional<ForumTag[]> ForumTags { get; set; }

     [JsonPropertyName("default_sort_order")]
    public Optional<ForumSortOrder?> DefaultSortOrder { get; set; }

    [JsonPropertyName("default_reaction_emoji")]
    public Optional<ForumReactionEmoji?> DefaultReactionEmoji { get; set; }

    [JsonPropertyName("default_forum_layout")]
    public Optional<ForumLayout> DefaultForumLayout { get; set; }

}
