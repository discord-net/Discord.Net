using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IPollModel : IModel
{
    IPollMediaModel Question { get; }

    IReadOnlyList<IPollAnswerModel> Answers { get; }

    DateTimeOffset? Expiry { get; }

    bool AllowMultiselect { get; }

    PollLayoutType LayoutType { get; }

    Optional<IPollResultsModel> Results { get; }
}

[APIModel]
public interface IPollAnswerCountModel : IModel
{
    int Id { get; }

    int Count { get; }

    bool MeVoted { get; }
}