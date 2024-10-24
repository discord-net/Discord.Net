using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a container that contains an ID of an entity, either from the id itself or from the entity.
/// </summary>
/// <typeparam name="TId">The ID type of the entity.</typeparam>
/// <typeparam name="TEntity">The entity type represented by the ID.</typeparam>
public readonly struct EntityOrId<TId, TEntity> : 
    IEquatable<EntityOrId<TId, TEntity>> 
    where TId : IEquatable<TId>
    where TEntity : IIdentifiable<TId>
{
    /// <summary>
    ///     The ID of the entity.
    /// </summary>
    public readonly TId Id;

    /// <summary>
    ///     Constructs a new <see cref="EntityOrId{TId, TEntity}" />.
    /// </summary>
    /// <param name="id">The ID of the entity</param>
    public EntityOrId(TId id)
    {
        Id = id;
    }

    /// <summary>
    ///     Constructs a new <see cref="EntityOrId{TId, TEntity}" />.
    /// </summary>
    /// <param name="entity">The entity of which to get the ID from.</param>
    public EntityOrId(TEntity entity)
    {
        Id = entity.Id;
    }

    public static implicit operator EntityOrId<TId, TEntity>(TId id) => new(id);
    public static implicit operator EntityOrId<TId, TEntity>(TEntity entity) => new(entity);

    public static implicit operator TId(EntityOrId<TId, TEntity> entityOrId) => entityOrId.Id;

    public bool Equals(EntityOrId<TId, TEntity> other)
    {
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is EntityOrId<TId, TEntity> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TId>.Default.GetHashCode(Id);
    }

    public static bool operator ==(EntityOrId<TId, TEntity> left, EntityOrId<TId, TEntity> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EntityOrId<TId, TEntity> left, EntityOrId<TId, TEntity> right)
    {
        return !left.Equals(right);
    }
}