using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class AuditLogChange
{
    [JsonPropertyName("key")]
    public string ChangedProperty { get; set; }

    [JsonPropertyName("new_value")]
    public JsonNode NewValue { get; set; }

    [JsonPropertyName("old_value")]
    public JsonNode OldValue { get; set; }
}
