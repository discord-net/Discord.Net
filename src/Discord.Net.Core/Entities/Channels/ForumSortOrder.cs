namespace Discord;

/// <summary>
/// Defines the rule used to order posts in forum channels.
/// </summary>
public enum ForumSortOrder
{
    /// <summary>
    ///     Sort forum posts by activity.
    /// </summary>
    LatestActivity = 0,

    /// <summary>
    ///     Sort forum posts by creation time (from most recent to oldest).
    /// </summary>
    CreationDate = 1
}
