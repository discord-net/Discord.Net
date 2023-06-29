using System.Text.Json.Serialization;

namespace Discord.API;

internal class Ban
{
    [JsonPropertyName("user")]
    public User User { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
