using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class TextInputComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("style")]
    public TextInputStyle Style { get; set; }

    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("placeholder")]
    public Optional<string> Placeholder { get; set; }

    [JsonPropertyName("min_length")]
    public Optional<int> MinLength { get; set; }

    [JsonPropertyName("max_length")]
    public Optional<int> MaxLength { get; set; }

    [JsonPropertyName("value")]
    public Optional<string> Value { get; set; }

    [JsonPropertyName("required")]
    public Optional<bool> Required { get; set; }
}
