using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 3)]
public sealed class StringApplicationCommandOption :
    ApplicationCommandOption,
    IStringApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    [JsonPropertyName("choices")]
    public Optional<ApplicationCommandOptionChoice<string>[]> Choices { get; set; }
    
    [JsonPropertyName("min_length")]
    public Optional<int> MinLength { get; set; }
    
    [JsonPropertyName("max_length")]
    public Optional<int> MaxLength { get; set; }
    
    [JsonPropertyName("autocomplete")]
    public Optional<bool> Autocomplete { get; set; }
    
    bool? IStringApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();

    IReadOnlyCollection<IApplicationCommandOptionChoiceModel<string>>? IStringApplicationCommandOptionModel.Choices
        => ~Choices;

    int? IStringApplicationCommandOptionModel.MinLength => MinLength.ToNullable();

    int? IStringApplicationCommandOptionModel.MaxLength => MaxLength.ToNullable();

    bool? IStringApplicationCommandOptionModel.Autocomplete => Autocomplete.ToNullable();
}