namespace Discord;

/// <summary>
///     Represents a poll answer counts object.
/// </summary>
public readonly struct PollAnswerCounts
{
    /// <summary>
    ///     Gets the Id of the answer.
    /// </summary>
    public readonly ulong AnswerId;

    /// <summary>
    ///     Gets the count of votes for this answer.
    /// </summary>
    public readonly uint Count;

    /// <summary>
    ///     Gets whether the current user voted for this answer.
    /// </summary>
    public readonly bool MeVoted;

    internal PollAnswerCounts(ulong answerId, uint count, bool meVoted)
    {
        AnswerId = answerId;
        Count = count;
        MeVoted = meVoted;
    }
}
