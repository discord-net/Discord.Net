using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyChannelPermissionsParams
{
    [JsonPropertyName("allow")]
    public Optional<ulong?> Allow { get; set; }

    [JsonPropertyName("deny")]
    public Optional<ulong?> Deny { get; set; }

    [JsonPropertyName("type")]
    public Optional<int?> Type { get; set; }
}
