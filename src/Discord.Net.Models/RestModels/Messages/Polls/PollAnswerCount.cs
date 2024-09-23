using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class PollAnswerCount : IPollAnswerCountModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("me_voted")]
    public bool MeVoted { get; set; }
}