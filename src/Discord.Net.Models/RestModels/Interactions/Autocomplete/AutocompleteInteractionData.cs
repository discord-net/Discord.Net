using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AutocompleteInteractionData
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("version")]
    public ulong Version { get; set; }

    [JsonPropertyName("options")]
    public required AutocompleteInteractionDataOption[] Options { get; set; }
}
