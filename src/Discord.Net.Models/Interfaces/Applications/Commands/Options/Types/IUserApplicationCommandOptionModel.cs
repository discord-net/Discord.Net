namespace Discord.Models;

[ModelEquality]
public partial interface IUserApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
}