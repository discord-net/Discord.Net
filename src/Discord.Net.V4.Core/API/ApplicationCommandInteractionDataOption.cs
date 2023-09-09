using System.Text.Json.Serialization;

namespace Discord.API;

public class ApplicationCommandInteractionDataOption
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public ApplicationCommandOptionType Type { get; set; }

    [JsonPropertyName("value")]
    public Optional<object> Value { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandInteractionDataOption[]> Options { get; set; }
}
