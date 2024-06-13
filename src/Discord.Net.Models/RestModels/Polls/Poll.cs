using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class Poll
{
    [JsonPropertyName("question")]
    public required PollMedia Question { get; set; }

    [JsonPropertyName("answers")]
    public required PollAnswer[] Answers { get; set; }

    [JsonPropertyName("expiry")]
    public DateTimeOffset Expiry { get; set; }

    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }

    [JsonPropertyName("layout_type")]
    public int LayoutType { get; set; }

    [JsonPropertyName("results")]
    public Optional<PollResults> PollResults { get; set; }
}
