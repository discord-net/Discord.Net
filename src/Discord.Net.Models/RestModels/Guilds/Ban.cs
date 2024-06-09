using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Ban
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }
}
