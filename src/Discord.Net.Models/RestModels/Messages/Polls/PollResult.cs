using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class PollResult : IPollResultModel
{
    [JsonPropertyName("is_finalized")]
    public bool IsFinalized { get; set; }

    [JsonPropertyName("answer_counts")]
    public required PollAnswerCount[] AnswerCounts { get; set; }
    
    IEnumerable<IPollAnswerCountModel> IPollResultModel.AnswerCounts => AnswerCounts;
}