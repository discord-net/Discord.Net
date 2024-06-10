using System.Text.Json.Serialization;

namespace Discord.Models;

public sealed class ModifyStageInstanceParams
{
    [JsonPropertyName("topic")]
    public Optional<string?> Topic { get; set; }

    [JsonPropertyName("privacy_level")]
    public Optional<int> PrivacyLevel { get; set; }
}
