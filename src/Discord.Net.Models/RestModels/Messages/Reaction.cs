using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Reaction
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("me")]
    public bool IsMe { get; set; }

    [JsonPropertyName("me_burst")]
    public bool IsMeBurst { get; set; }

    [JsonPropertyName("emoji")]
    public required Emoji Emoji { get; set; }

    [JsonPropertyName("count_details")]
    public required ReactionCountDetails CountDetails { get; set; }

    [JsonPropertyName("burst_colors")]
    public required string[] Colors { get; set; }
}
