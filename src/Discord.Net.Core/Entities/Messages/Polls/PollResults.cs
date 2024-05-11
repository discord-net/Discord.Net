using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the results of a poll.
/// </summary>
public readonly struct PollResults
{
    /// <summary>
    ///     Gets whether the poll results are finalized.
    /// </summary>
    public readonly bool IsFinalized;

    /// <summary>
    ///     Gets the answer counts for the poll.
    /// </summary>
    public readonly IReadOnlyCollection<PollAnswerCounts> AnswerCounts;

    internal PollResults(bool isFinalized, IReadOnlyCollection<PollAnswerCounts> answerCounts)
    {
        IsFinalized = isFinalized;
        AnswerCounts = answerCounts;
    }
}
