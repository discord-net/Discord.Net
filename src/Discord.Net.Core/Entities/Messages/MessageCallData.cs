using System;

namespace Discord;

/// <summary>
///     Represents the call data of a message.
/// </summary>
public readonly struct MessageCallData
{
    /// <summary>
    ///     Gets the participants of the call.
    /// </summary>
    public readonly ulong[] Participants;

    /// <summary>
    ///     Gets the timestamp when the call ended. This is <see langword="null"/> if the call is still ongoing.
    /// </summary>
    public readonly DateTimeOffset? EndedTimestamp;

    internal MessageCallData(ulong[] participants, DateTimeOffset? endedTimestamp)
    {
        Participants = participants;
        EndedTimestamp = endedTimestamp;
    }
}
