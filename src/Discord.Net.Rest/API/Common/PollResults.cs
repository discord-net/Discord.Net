using Newtonsoft.Json;

namespace Discord.API;

internal class PollResults
{
    [JsonProperty("is_finalized")]
    public bool IsFinalized { get; set; }

    [JsonProperty("answer_counts")]
    public PollAnswerCount[] AnswerCounts { get; set; }
}
