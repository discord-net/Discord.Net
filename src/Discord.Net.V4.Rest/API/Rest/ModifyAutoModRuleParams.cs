using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyAutoModRuleParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("event_type")]
    public Optional<AutoModEventType> EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public Optional<AutoModTriggerType> TriggerType { get; set; }

    [JsonPropertyName("trigger_metadata")]
    public Optional<TriggerMetadata> TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public Optional<AutoModAction[]> Actions { get; set; }

    [JsonPropertyName("enabled")]
    public Optional<bool> Enabled { get; set; }

    [JsonPropertyName("exempt_roles")]
    public Optional<ulong[]> ExemptRoles { get; set; }

    [JsonPropertyName("exempt_channels")]
    public Optional<ulong[]> ExemptChannels { get; set; }
}
