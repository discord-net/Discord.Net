using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class PollAnswer : IPollAnswerModel
{
    [JsonPropertyName("answer_id")]
    public int Id { get; set; }

    [JsonPropertyName("poll_media")]
    public required PollMedia PollMedia { get; set; }

    IPollMediaModel IPollAnswerModel.Media => PollMedia;
}