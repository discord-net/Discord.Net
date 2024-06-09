using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AutocompleteInteractionDataOption
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("options")]
    public Optional<AutocompleteInteractionDataOption[]> Options { get; set; }

    [JsonPropertyName("value")]
    public Optional<object> Value { get; set; }

    [JsonPropertyName("focused")]
    public Optional<bool> IsFocused { get; set; }
}
