using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationCommandInteractionInteractionDataOption : IApplicationCommandInteractionOptionModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("value")]
    public Optional<object> Value { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandInteractionInteractionDataOption[]> Options { get; set; }

    [JsonPropertyName("focused")]
    public bool IsFocused { get; set; }

    object? IApplicationCommandInteractionOptionModel.Value => ~Value;
    IEnumerable<IApplicationCommandInteractionOptionModel>? IApplicationCommandInteractionOptionModel.Options => ~Options;
    bool? IApplicationCommandInteractionOptionModel.IsFocused => IsFocused;
}
