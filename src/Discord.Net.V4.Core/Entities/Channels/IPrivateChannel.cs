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
    ///     A <see cref="IEntityEnumerableSource{TId, TEntity}" /> of users that can access this channel.
    /// </returns>
    IEntityEnumerableSource<ulong, IUser> Recipients { get; }
}
