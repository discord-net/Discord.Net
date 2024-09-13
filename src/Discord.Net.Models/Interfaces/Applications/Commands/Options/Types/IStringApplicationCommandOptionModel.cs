namespace Discord.Models;

[ModelEquality]
public partial interface IStringApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
    IReadOnlyCollection<IApplicationCommandOptionChoiceModel<string>>? Choices { get; }
    int? MinLength { get; }
    int? MaxLength { get; }
    bool? Autocomplete { get; }
}