using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ReactionCountDetails
{
    [JsonPropertyName("normal")]
    public int NormalCount { get; set; }

    [JsonPropertyName("burst")]
    public int BurstCount { get; set; }
}
