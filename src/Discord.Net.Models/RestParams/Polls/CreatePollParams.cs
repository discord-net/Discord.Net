using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class CreatePollParams
{
    [JsonPropertyName("question")]
    public required PollMedia Question { get; set; }

    [JsonPropertyName("answers")]
    public required PollAnswer[] Answers { get; set; }

    [JsonPropertyName("duration")]
    public uint Duration { get; set; }

    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }

    [JsonPropertyName("layout_type")]
    public Optional<int> LayoutType { get; set; }
}
