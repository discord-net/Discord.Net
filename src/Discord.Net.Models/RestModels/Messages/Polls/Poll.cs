using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Poll : IPollModel
{
    [JsonPropertyName("question")]
    public required PollMedia Question { get; set; }

    [JsonPropertyName("answers")]
    public required PollAnswer[] Answers { get; set; }
    
    [JsonPropertyName("expiry")]
    public DateTimeOffset? Expiry { get; set; }
    
    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }
    
    [JsonPropertyName("layout_type")]
    public int LayoutType { get; set; }
    
    [JsonPropertyName("results")]
    public Optional<PollResult> Results { get; set; }
    
    IPollResultModel? IPollModel.Results => ~Results;
    IEnumerable<IPollAnswerModel> IPollModel.Answers => Answers;
    IPollMediaModel IPollModel.Question => Question;
}
