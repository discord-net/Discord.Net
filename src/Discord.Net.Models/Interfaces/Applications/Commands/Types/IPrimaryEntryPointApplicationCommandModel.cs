namespace Discord.Models;

[ModelEquality]
public partial interface IPrimaryEntryPointApplicationCommandModel : IApplicationCommandModel
{
    int? Handler { get; }
}