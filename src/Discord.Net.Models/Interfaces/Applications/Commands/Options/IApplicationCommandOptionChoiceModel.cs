namespace Discord.Models;

public interface IApplicationCommandOptionChoiceModel<
    [OneOf(typeof(long), typeof(string), typeof(double))]
    out T
> : IApplicationCommandOptionChoiceModel
    where T : notnull
{
    new T Value { get; }

    object IApplicationCommandOptionChoiceModel.Value => Value;
}

[ModelEquality]
public partial interface IApplicationCommandOptionChoiceModel : IModel
{
    string Name { get; }
    IReadOnlyDictionary<string, string>? NameLocalization { get; }
    object Value { get; }
}