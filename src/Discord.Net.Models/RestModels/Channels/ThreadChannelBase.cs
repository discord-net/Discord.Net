using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public abstract class ThreadChannelBase : Channel
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
    public Optional<int?> RatelimitPerUser { get; set; }

    [JsonPropertyName("permissions")]
    public Optional<ulong> Permissions { get; set; }

    [JsonPropertyName("owner_id")]
    public ulong OwnerId { get; set; }

    [JsonPropertyName("thread_metadata")]
    public Optional<ThreadMetadata> Metadata { get; set; }

    [JsonPropertyName("message_count")]
    public int MessageCount { get; set; }

    [JsonPropertyName("member_count")]
    public int MemberCount { get; set; }

    [JsonPropertyName("total_messages_sent")]
    public int TotalMessagesSent { get; set; }

    [JsonPropertyName("applied_tags")]
    public Optional<ulong[]> AppliedTags { get; set; }

    [JsonPropertyName("member")]
    public Optional<ThreadMember> Member { get; set; }
}
