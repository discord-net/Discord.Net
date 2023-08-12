using System.Text.Json.Serialization;

namespace Discord.API;

internal class Overwrite
{
    [JsonPropertyName("id")]
    public ulong TargetId { get; set; }

    [JsonPropertyName("type")]
    public PermissionTarget TargetType { get; set; }

    [JsonPropertyName("deny")]
    public ulong Deny { get; set; }

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }
}
