using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class PollVoters
{
    [JsonPropertyName("users")]
    public required User[] Users { get; set; }
}