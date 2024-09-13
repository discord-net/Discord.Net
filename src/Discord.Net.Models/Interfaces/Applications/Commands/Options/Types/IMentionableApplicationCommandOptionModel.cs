namespace Discord.Models;

[ModelEquality]
public partial interface IMentionableApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
}