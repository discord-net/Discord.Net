namespace Discord.Models;

[ModelEquality]
public partial interface IPollAnswerModel : IEntityModel<int>
{
    IPollMediaModel Media { get; }
}