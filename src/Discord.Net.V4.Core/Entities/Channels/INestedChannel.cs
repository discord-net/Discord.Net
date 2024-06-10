namespace Discord;

/// <summary>
///     Represents a type of guild channel that can be nested within a category.
/// </summary>
public interface INestedChannel : IGuildChannel, IInvitableChannel
{
    /// <summary>
    ///     Gets the parent (category) of this channel in the guild's channel list.
    /// </summary>
    /// <returns>
    ///     A <see cref="ILoadableEntity{TId,TEntity}" /> representing the category of this channel;
    ///     <see langword="null" /> if none is set.
    /// </returns>
    ILoadableEntity<ulong, ICategoryChannel>? Category { get; }

    /// <summary>
    ///     Syncs the permissions of this nested channel with its parent's.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for syncing channel permissions with its parent's.
    /// </returns>
    Task SyncPermissionsAsync(RequestOptions? options = null, CancellationToken token = default);
}
