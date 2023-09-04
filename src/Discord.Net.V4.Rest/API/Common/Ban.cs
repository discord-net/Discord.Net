using System.Text.Json.Serialization;

namespace Discord.API;

public class Ban
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }
}
