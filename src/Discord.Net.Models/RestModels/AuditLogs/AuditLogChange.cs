using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AuditLogChange
{
    [JsonPropertyName("key")]
    public required string ChangedProperty { get; set; }

    [JsonPropertyName("new_value")]
    public required JsonNode NewValue { get; set; }

    [JsonPropertyName("old_value")]
    public required JsonNode OldValue { get; set; }
}
