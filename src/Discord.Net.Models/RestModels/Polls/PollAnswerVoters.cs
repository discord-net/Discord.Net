using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class PollAnswerVoters
{
    [JsonPropertyName("users")]
    public required User[] Users { get; set; }
}
