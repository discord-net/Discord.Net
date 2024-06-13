using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class PollMedia
{
    [JsonPropertyName("text")]
    public Optional<string> Text { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<Emoji> Emoji { get; set; }
}
