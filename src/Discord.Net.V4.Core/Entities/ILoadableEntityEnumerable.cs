namespace Discord;

/// <summary>
///     Represents a collection wrapper of <see cref="ILoadableEntity{TId,TEntity}" />.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The ID type of the entity.</typeparam>
public interface ILoadableEntityEnumerable<TId, TEntity> : IAsyncEnumerable<TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    ILoadableEntity<TId, TEntity> this[TId id] { get => Specifically(id); }

    ILoadableEntity<TId, TEntity> Specifically(TId id);

    /// <summary>
    ///     Gets a specific entity from this collection.
    /// </summary>
    /// <param name="id">The ID of the entity to get.</param>
    /// <returns>
    ///     A <see cref="ValueTask" /> representing the asynchronous operation of
    ///     getting the entity. The result of the <see cref="ValueTask" /> is a
    ///     the entity representing the ID supplied if found; otherwise <see langword="null" />.
    /// </returns>
    ValueTask<TEntity?> GetAsync(TId id, RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Flattens this <see cref="IAsyncEnumerable{T}" /> of <see cref="ILoadableEntity{TId,TEntity}" />
    ///     to a readonly collection of <see cref="TEntity" />s.
    /// </summary>
    /// <returns>
    ///     A <see cref="ValueTask" /> that represents the asynchronous operation
    ///     of flattening this <see cref="IAsyncEnumerable{T}" />. The result of the
    ///     <see cref="ValueTask" /> is a readonly collection of <see cref="TEntity" />s.
    /// </returns>
    ValueTask<IReadOnlyCollection<TEntity>> FlattenAsync(RequestOptions? option = null,
        CancellationToken token = default);
}

/// <summary>
///     Represents a collection wrapper of <see cref="ILoadableEntity{TId,TEntity}" />.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The ID type of the entity.</typeparam>
public interface IDefinedLoadableEntityEnumerable<TId, TEntity> : ILoadableEntityEnumerable<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     Gets a read-only collection of ids that are explicitly defined for this enumerable source.
    /// </summary>
    IReadOnlyCollection<TId> Ids { get; }

    /// <summary>
    ///     Gets a collection of IDs for the entities that this collection represents.
    /// </summary>
    /// <returns>
    ///     A <see cref="ValueTask" /> representing the asynchronous operation of
    ///     getting the IDs. The result of the <see cref="ValueTask" /> is a
    ///     readonly collection of entity IDs.
    /// </returns>
    ValueTask<IReadOnlyCollection<TId>> GetIdsAsync(RequestOptions? options = null, CancellationToken token = default);

    IEnumerable<ILoadableEntity<TId, TEntity>> Specifically(IEnumerable<TId> ids);
    IEnumerable<ILoadableEntity<TId, TEntity>> Specifically(params TId[] ids) => Specifically((IEnumerable<TId>)ids);

    IAsyncEnumerable<ILoadableEntity<TId, TEntity>> EnumerateLoadables();
}


