using System.Text.Json.Serialization;

namespace Discord.API;

public class EmbedProvider
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }
}
