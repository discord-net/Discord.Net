namespace Discord;

/// <summary>
///     Represents a generic channel that is private to select recipients.
/// </summary>
public interface IPrivateChannel : IChannel
{
    /// <summary>
    ///     Gets the users that can access this channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="ILoadableEntityEnumerable{TId,TEntity}" /> of users that can access this channel.
    /// </returns>
    ILoadableEntityEnumerable<ulong, IUser> Recipients { get; }
}
