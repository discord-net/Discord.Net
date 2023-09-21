using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class AutoModerationRule
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
    public AutoModEventType EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public AutoModTriggerType TriggerType { get; set; }

    [JsonPropertyName("trigger_metadata")]
    public required TriggerMetadata TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public required AutoModerationAction[] Actions { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("exempt_roles")]
    public required ulong[] ExemptRoles { get; set; }

    [JsonPropertyName("exempt_channels")]
    public required ulong[] ExemptChannels { get; set; }
}
