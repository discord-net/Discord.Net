using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildPruneCount
{
    [JsonPropertyName("pruned")]
    public int Pruned { get; set; }
}
