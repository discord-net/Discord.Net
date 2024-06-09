using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationCommandInteractionDataOption
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("value")]
    public Optional<object> Value { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandInteractionDataOption[]> Options { get; set; }
}
