namespace Discord;

/// <summary>
///     Represents a wrapper of a relation between 2 entities, this
///     provides an implementation-agnostic way to lazily represent
///     a relation.
/// </summary>
/// <typeparam name="TEntity">The entity type of the source.</typeparam>
/// <typeparam name="TId">The ID type of the source.</typeparam>
public interface IEntitySource<TId, TEntity> : ILoadableEntity<TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     Gets the unique identifier of the entity being represented.
    /// </summary>
    TId Id { get; }
}
