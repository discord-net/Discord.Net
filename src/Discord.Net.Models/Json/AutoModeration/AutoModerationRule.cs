using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AutoModerationRule : IAutoModerationRuleModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("creator_id")]
    public ulong CreatorId { get; set; }

    [JsonPropertyName("event_type")]
    public int EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public int TriggerType { get; set; }

    [
        JsonPropertyName("trigger_metadata"),
        JsonIgnore,
        DiscriminatedUnion(nameof(TriggerType)),
        DiscriminatedUnionEntry<KeywordTriggerMetadata>(1),
        DiscriminatedUnionEntry<KeywordPresetTriggerMetadata>(4),
        DiscriminatedUnionEntry<MentionSpamTriggerMetadata>(5),
        DiscriminatedUnionEntry<MemberProfileTriggerMetadata>(6),
    ]
    public Optional<TriggerMetadata?> TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public required AutoModerationAction[] Actions { get; set; }

    [JsonPropertyName("enabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("exempt_roles")]
    public required ulong[] ExemptRoles { get; set; }

    [JsonPropertyName("exempt_channels")]
    public required ulong[] ExemptChannels { get; set; }

    ITriggerMetadataModel? IAutoModerationRuleModel.TriggerMetadata => ~TriggerMetadata;
    IEnumerable<IAutoModerationActionModel> IAutoModerationRuleModel.Actions => Actions;
}
