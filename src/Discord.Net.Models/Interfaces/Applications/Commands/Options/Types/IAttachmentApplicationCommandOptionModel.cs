namespace Discord.Models;

[ModelEquality]
public partial interface IAttachmentApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
}