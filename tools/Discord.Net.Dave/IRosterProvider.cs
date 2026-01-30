namespace Discord.LibDave;

/// <summary>
///     An interface providing roster details.
/// </summary>
public interface IRosterProvider : IDisposable
{
    /// <summary>
    ///     Gets the signature of a given user based on their ID.
    /// </summary>
    /// <param name="userId">The snowflake identifier of the user whose signature to get.</param>
    /// <returns>The signature of the given user.</returns>
    DaveAllocatedSpan<byte> GetRosterMemberSignature(ulong userId);

    /// <summary>
    ///     Gets the user ids within this roster.
    /// </summary>
    /// <returns>The snowflake identifiers of the users within this roster.</returns>
    DaveAllocatedSpan<ulong> GetRosterMemberIds();
}
