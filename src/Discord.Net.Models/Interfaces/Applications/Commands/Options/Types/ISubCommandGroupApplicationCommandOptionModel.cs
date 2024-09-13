namespace Discord.Models;

[ModelEquality]
public partial interface ISubCommandGroupApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    IReadOnlyCollection<IApplicationCommandOptionModel>? Options { get; } 
}