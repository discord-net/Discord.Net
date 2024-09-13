namespace Discord.Models;

[ModelEquality]
public partial interface IApplicationCommandOptionModel : IModel
{
    int Type { get; }
    string Name { get; }
    IReadOnlyDictionary<string, string>? NameLocalizations { get; }
    string Description { get; }
    IReadOnlyDictionary<string, string>? DescriptionLocalizations { get; }
}