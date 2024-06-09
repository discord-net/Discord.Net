using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Overwrite : IOverwriteModel
{
    [JsonPropertyName("id")]
    public ulong TargetId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("deny")]
    public ulong Deny { get; set; }

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }
}
