namespace Discord.Models;

[ModelEquality]
public partial interface IIntegerApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
    IReadOnlyCollection<IApplicationCommandOptionChoiceModel<long>>? Choices { get; }
    long? MinValue { get; }
    long? MaxValue { get; }
    bool? Autocomplete { get; }
}