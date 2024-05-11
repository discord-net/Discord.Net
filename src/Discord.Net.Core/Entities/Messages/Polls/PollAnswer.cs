namespace Discord;

/// <summary>
///     Represents a poll answer object.
/// </summary>
public readonly struct PollAnswer
{
    /// <summary>
    ///     Gets the Id of the answer.
    /// </summary>
    public readonly uint AnswerId;

    /// <summary>
    ///     Gets the poll media of this answer.
    /// </summary>
    public readonly PollMedia PollMedia;

    internal PollAnswer(uint id, PollMedia media)
    {
        AnswerId = id;
        PollMedia = media;
    }
}
