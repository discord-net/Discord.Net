namespace Discord;

/// <summary>
///     Defines the rule used to order posts in forum and media channels.
/// </summary>
public enum SortOrder
{
    /// <summary>
    ///     Sort posts by activity.
    /// </summary>
    LatestActivity = 0,

    /// <summary>
    ///     Sort posts by creation time (from most recent to oldest).
    /// </summary>
    CreationDate = 1
}
