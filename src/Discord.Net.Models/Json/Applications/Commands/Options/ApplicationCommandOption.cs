using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionRootType(nameof(Type))]
public class ApplicationCommandOption : IApplicationCommandOptionModel
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

    IReadOnlyDictionary<string, string>? IApplicationCommandOptionModel.NameLocalizations => ~NameLocalizations;

    IReadOnlyDictionary<string, string>? IApplicationCommandOptionModel.DescriptionLocalizations
        => ~DescriptionLocalizations;

}
