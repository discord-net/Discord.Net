namespace Discord;

/// <summary>
///     Represents the sort type for searching members in a guild.
/// </summary>
public enum MemberSearchV2SortType
{
    /// <summary>
    ///    Sort by member since newest first.
    /// </summary>
    MemberSinceNewestFirst = 1,

    /// <summary>
    ///     Sort by member since oldest first.
    /// </summary>
    MemberSinceOldestFirst = 2,

    /// <summary>
    ///     Sort by joined discord since newest first.
    /// </summary>
    JoinedDiscordNewestFirst = 3,

    /// <summary>
    ///     Sort by joined discord since oldest first.
    /// </summary>
    JoinedDiscordOldestFirst = 4,
}
