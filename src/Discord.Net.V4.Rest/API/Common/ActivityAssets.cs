using System.Text.Json.Serialization;

namespace Discord.API;

public class ActivityAssets
{
    [JsonPropertyName("small_text")]
    public Optional<string> SmallText { get; set; }

    [JsonPropertyName("small_image")]
    public Optional<string> SmallImage { get; set; }

    [JsonPropertyName("large_text")]
    public Optional<string> LargeText { get; set; }

    [JsonPropertyName("large_image")]
    public Optional<string> LargeImage { get; set; }
}
