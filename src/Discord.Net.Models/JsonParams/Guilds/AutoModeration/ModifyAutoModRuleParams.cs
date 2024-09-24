using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyAutoModRuleParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("event_type")]
    public Optional<int> EventType { get; set; }

    [JsonPropertyName("trigger_metadata")]
    public Optional<TriggerMetadata> TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public Optional<AutoModerationAction[]> Actions { get; set; }

    [JsonPropertyName("enabled")]
    public Optional<bool> IsEnabled { get; set; }

    [JsonPropertyName("exempt_roles")]
    public Optional<ulong[]> ExemptRoles { get; set; }

    [JsonPropertyName("exempt_channels")]
    public Optional<ulong[]> ExemptChannels { get; set; }
}
