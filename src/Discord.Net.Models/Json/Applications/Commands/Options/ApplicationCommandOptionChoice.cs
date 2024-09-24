using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationCommandOptionChoice<T> : IApplicationCommandOptionChoiceModel<T>
    where T : notnull
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

    [JsonPropertyName("value")]
    public required T Value { get; set; }

    IReadOnlyDictionary<string, string>? IApplicationCommandOptionChoiceModel.NameLocalization
        => ~NameLocalizations;

}
