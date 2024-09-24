using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 10)]
public sealed class NumberApplicationCommandOption :
    ApplicationCommandOption,
    INumberApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    [JsonPropertyName("choices")]
    public Optional<ApplicationCommandOptionChoice<double>[]> Choices { get; set; }
    
    [JsonPropertyName("min_value")]
    public Optional<double> MinValue { get; set; }
    
    [JsonPropertyName("max_value")]
    public Optional<double> MaxValue { get; set; }
    
    [JsonPropertyName("autocomplete")]
    public Optional<bool> Autocomplete { get; set; }

    IReadOnlyCollection<IApplicationCommandOptionChoiceModel<double>>? INumberApplicationCommandOptionModel.Choices
        => ~Choices;
    
    double? INumberApplicationCommandOptionModel.MinValue => MinValue.ToNullable();
    double? INumberApplicationCommandOptionModel.MaxValue => MaxValue.ToNullable();
    bool? INumberApplicationCommandOptionModel.Autocomplete => Autocomplete.ToNullable();
    bool? INumberApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();
}