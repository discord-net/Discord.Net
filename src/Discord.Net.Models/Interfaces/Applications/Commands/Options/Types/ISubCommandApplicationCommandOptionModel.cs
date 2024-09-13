namespace Discord.Models;

[ModelEquality]
public partial interface ISubCommandApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    IReadOnlyCollection<IApplicationCommandOptionModel>? Options { get; } 
}