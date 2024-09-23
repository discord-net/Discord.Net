namespace Discord.Models;

[ModelEquality]
public partial interface IPollModel : IModel
{
    IPollMediaModel Question { get; }
    IEnumerable<IPollAnswerModel> Answers { get; }
    DateTimeOffset? Expiry { get; }
    bool AllowMultiselect { get; }
    int LayoutType { get; }
    IPollResultModel? Results { get; }
}