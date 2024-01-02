using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class CreateAutoModRuleParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("event_type")]
    public AutoModEventType EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public AutoModTriggerType TriggerType { get; set; }

    [JsonPropertyName("trigger_metadata")]
    public Optional<TriggerMetadata> TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public required AutoModerationAction[] Actions { get; set; }

    [JsonPropertyName("enabled")]
    public Optional<bool> IsEnabled { get; set; }

    [JsonPropertyName("exempt_roles")]
    public Optional<ulong[]> ExemptRoles { get; set; }

    [JsonPropertyName("exempt_channels")]
    public Optional<ulong[]> ExemptChannels { get; set; }
}
