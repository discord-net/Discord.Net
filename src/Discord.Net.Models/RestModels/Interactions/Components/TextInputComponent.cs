using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ComponentType(ComponentTypes.TextInput)]
public sealed class TextInputComponent : MessageComponent
{
    [JsonPropertyName("style")]
    public int Style { get; set; }

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
