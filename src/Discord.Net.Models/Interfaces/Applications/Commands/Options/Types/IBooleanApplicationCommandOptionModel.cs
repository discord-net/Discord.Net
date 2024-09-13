namespace Discord.Models;

[ModelEquality]
public partial interface IBooleanApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
}