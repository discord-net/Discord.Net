namespace Discord;

/// <summary>
///     Represents a container that contains an ID of an entity, either from the id itself or from the entity.
/// </summary>
/// <typeparam name="TId">The ID type of the entity.</typeparam>
/// <typeparam name="TEntity">The entity type represented by the ID.</typeparam>
public readonly struct EntityOrId<TId, TEntity>
    where TId : notnull, IEquatable<TId>
    where TEntity : IEntity<TId>
{
    /// <summary>
    ///     The ID of the entity.
    /// </summary>
    public readonly TId Id;

    /// <summary>
    ///     Constructs a new <see cref="EntityOrId{TId, TEntity}"/>.
    /// </summary>
    /// <param name="id">The ID of the entity</param>
    public EntityOrId(TId id)
    {
        Id = id;
    }

    /// <summary>
    ///     Constructs a new <see cref="EntityOrId{TId, TEntity}"/>.
    /// </summary>
    /// <param name="entity">The entity of which to get the ID from.</param>
    public EntityOrId(TEntity entity)
    {
        Id = entity.Id;
    }

    public static implicit operator EntityOrId<TId, TEntity>(TId id) => new(id);
    public static implicit operator EntityOrId<TId, TEntity>(TEntity entity) => new(entity);
}
