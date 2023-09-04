using System.Text.Json.Serialization;

namespace Discord.API;

public class AutocompleteInteractionDataOption
{
    [JsonPropertyName("type")]
    public ApplicationCommandOptionType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("options")]
    public Optional<AutocompleteInteractionDataOption[]> Options { get; set; }

    [JsonPropertyName("value")]
    public Optional<object> Value { get; set; }

    [JsonPropertyName("focused")]
    public Optional<bool> Focused { get; set; }
}
