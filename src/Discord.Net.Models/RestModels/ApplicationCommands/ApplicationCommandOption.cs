using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationCommandOption
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }

    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    [JsonPropertyName("choices")]
    public Optional<ApplicationCommandOptionChoice[]> Choices { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandOption[]> Options { get; set; }

    [JsonPropertyName("channel_types")]
    public Optional<int[]> ChannelTypes { get; set; }

    [JsonPropertyName("autocomplete")]
    public Optional<bool> Autocomplete { get; set; }

    [JsonPropertyName("min_value")]
    public Optional<double> MinValue { get; set; }

    [JsonPropertyName("max_value")]
    public Optional<double> MaxValue { get; set; }

    [JsonPropertyName("min_length")]
    public Optional<int> MinLength { get; set; }

    [JsonPropertyName("max_length")]
    public Optional<int> MaxLength { get; set; }
}
