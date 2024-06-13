using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class PollResults
{
    [JsonPropertyName("is_finalized")]
    public bool IsFinalized { get; set; }

    [JsonPropertyName("answer_counts")]
    public required PollAnswerCount[] AnswerCounts { get; set; }
}
