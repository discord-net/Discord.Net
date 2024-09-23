namespace Discord.Models;

[ModelEquality]
public partial interface IPollResultModel : IModel
{
    bool IsFinalized { get; }
    
    IEnumerable<IPollAnswerCountModel> AnswerCounts { get; }
}