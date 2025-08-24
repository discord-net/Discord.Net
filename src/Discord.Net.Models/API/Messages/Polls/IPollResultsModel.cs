using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IPollResultsModel : IModel
{
    bool IsFinalized { get; }
    IReadOnlyList<IPollAnswerCountModel> AnswerCounts { get; }
}