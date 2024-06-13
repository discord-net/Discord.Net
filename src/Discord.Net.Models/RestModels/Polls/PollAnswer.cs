using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class PollAnswer
{
    [JsonPropertyName("answer_id")]
    public uint AnswerId { get; set; }

    [JsonPropertyName("poll_media")]
    public required PollMedia PollMedia { get; set; }
}
