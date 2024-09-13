namespace Discord.Models;

[ModelEquality]
public partial interface IChatInputApplicationCommandModel : IApplicationCommandModel
{
    IReadOnlyCollection<IApplicationCommandOptionModel>? Options { get; }
}