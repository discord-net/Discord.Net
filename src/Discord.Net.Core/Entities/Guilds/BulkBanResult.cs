using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a result of a bulk ban.
/// </summary>
public readonly struct BulkBanResult
{
    /// <summary>
    ///     Gets the collection of user IDs that were successfully banned.
    /// </summary>
    public IReadOnlyCollection<ulong> BannedUsers { get; }

    /// <summary>
    ///     Gets the collection of user IDs that failed to be banned.
    /// </summary>
    public IReadOnlyCollection<ulong> FailedUsers { get; }

    internal BulkBanResult(IReadOnlyCollection<ulong> bannedUsers, IReadOnlyCollection<ulong> failedUsers)
    {
        BannedUsers = bannedUsers;
        FailedUsers = failedUsers;
    }
}
