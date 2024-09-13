using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 4)]
public sealed class IntegerApplicationCommandOption :
    ApplicationCommandOption,
    IIntegerApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    [JsonPropertyName("choices")]
    public Optional<ApplicationCommandOptionChoice<long>[]> Choices { get; set; }
    
    [JsonPropertyName("min_value")]
    public Optional<long> MinValue { get; set; }
    
    [JsonPropertyName("max_value")]
    public Optional<long> MaxValue { get; set; }
    
    [JsonPropertyName("autocomplete")]
    public Optional<bool> Autocomplete { get; set; }

    IReadOnlyCollection<IApplicationCommandOptionChoiceModel<long>>? IIntegerApplicationCommandOptionModel.Choices
        => ~Choices;
    
    long? IIntegerApplicationCommandOptionModel.MinValue => MinValue.ToNullable();
    long? IIntegerApplicationCommandOptionModel.MaxValue => MaxValue.ToNullable();
    bool? IIntegerApplicationCommandOptionModel.Autocomplete => Autocomplete.ToNullable();
    bool? IIntegerApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();
}