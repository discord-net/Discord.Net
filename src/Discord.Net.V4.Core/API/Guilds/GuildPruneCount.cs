using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class GuildPruneCount
{
    [JsonPropertyName("pruned")]
    public int Pruned { get; set; }
}
