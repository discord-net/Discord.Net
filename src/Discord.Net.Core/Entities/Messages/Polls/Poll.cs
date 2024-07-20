using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a poll object.
/// </summary>
public readonly struct Poll
{
    /// <summary>
    ///     Gets the question for the poll.
    /// </summary>
    public readonly PollMedia Question;

    /// <summary>
    ///     Gets the answers for the poll.
    /// </summary>
    public readonly IReadOnlyCollection<PollAnswer> Answers;

    /// <summary>
    ///     Gets the expiration date for the poll.
    /// </summary>
    public readonly DateTimeOffset ExpiresAt;

    /// <summary>
    ///     Gets whether the poll allows multiple answers.
    /// </summary>
    public readonly bool AllowMultiselect;

    /// <summary>
    ///     Gets the layout type for the poll.
    /// </summary>
    public readonly PollLayout LayoutType;

    /// <summary>
    ///     Gets the results of the poll. This is <see langword="null"/> if the poll is not finalized.
    /// </summary>
    public readonly PollResults? Results;

    internal Poll(PollMedia question, IReadOnlyCollection<PollAnswer> answers, DateTimeOffset expiresAt, bool allowMultiselect, PollLayout layoutType, PollResults? results)
    {
        Question = question;
        Answers = answers;
        ExpiresAt = expiresAt;
        AllowMultiselect = allowMultiselect;
        LayoutType = layoutType;
        Results = results;
    }
}
