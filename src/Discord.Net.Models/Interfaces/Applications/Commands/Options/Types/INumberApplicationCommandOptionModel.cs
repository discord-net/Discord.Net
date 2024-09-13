namespace Discord.Models;

[ModelEquality]
public partial interface INumberApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
    IReadOnlyCollection<IApplicationCommandOptionChoiceModel<double>>? Choices { get; }
    double? MinValue { get; }
    double? MaxValue { get; }
    bool? Autocomplete { get; }
}