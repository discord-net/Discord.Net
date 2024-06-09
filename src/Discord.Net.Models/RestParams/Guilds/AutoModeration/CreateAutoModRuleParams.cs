using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateAutoModRuleParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("event_type")]
    public int EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public int TriggerType { get; set; }

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
