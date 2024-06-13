using System.Text.Json.Serialization;

namespace Discord.Models.Json;


public class PollAnswerCount
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("count")]
    public uint Count { get; set; }

    [JsonPropertyName("me_voted")]
    public bool MeVoted { get; set; }
}
