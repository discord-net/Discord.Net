using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class GetGuildPruneCountResponse
{
    [JsonPropertyName("pruned")]
    public int Pruned { get; set; }
}
