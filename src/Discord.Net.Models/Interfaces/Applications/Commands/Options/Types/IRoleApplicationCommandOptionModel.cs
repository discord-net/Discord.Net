namespace Discord.Models;

[ModelEquality]
public partial interface IRoleApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
}