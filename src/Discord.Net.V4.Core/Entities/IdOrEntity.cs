using Discord.Rest;

namespace Discord;

public interface IIdLike<out TId>
{
    TId Id { get; }
}

/// <summary>
///     Represents a container that contains an ID of an entity, either from the id itself or from the entity.
/// </summary>
/// <typeparam name="TId">The ID type of the entity.</typeparam>
/// <typeparam name="TEntity">The entity type represented by the ID.</typeparam>
public readonly struct IdOrEntity<TId, TEntity> : 
    IIdLike<TId>,
    IEquatable<IdOrEntity<TId, TEntity>> 
    where TId : IEquatable<TId>
    where TEntity : IIdentifiable<TId>
{
    /// <summary>
    ///     The ID of the entity.
    /// </summary>
    public TId Id { get; }

    /// <summary>
    ///     Constructs a new <see cref="IdOrEntity{TId,TEntity}" />.
    /// </summary>
    /// <param name="id">The ID of the entity</param>
    public IdOrEntity(TId id)
    {
        Id = id;
    }

    /// <summary>
    ///     Constructs a new <see cref="IdOrEntity{TId,TEntity}" />.
    /// </summary>
    /// <param name="entity">The entity of which to get the ID from.</param>
    public IdOrEntity(TEntity entity)
    {
        Id = entity.Id;
    }

    public static implicit operator IdOrEntity<TId, TEntity>(TId id) => new(id);
    public static implicit operator IdOrEntity<TId, TEntity>(TEntity entity) => new(entity);

    public static implicit operator TId(IdOrEntity<TId, TEntity> idOrEntity) => idOrEntity.Id;

    public bool Equals(IdOrEntity<TId, TEntity> other)
    {
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is IdOrEntity<TId, TEntity> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TId>.Default.GetHashCode(Id);
    }

    public static bool operator ==(IdOrEntity<TId, TEntity> left, IdOrEntity<TId, TEntity> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IdOrEntity<TId, TEntity> left, IdOrEntity<TId, TEntity> right)
    {
        return !left.Equals(right);
    }
}

public static class IdOrEntityExtensions
{
    public static IEnumerable<TId> Ids<TId, TEntity>(this IEnumerable<IdOrEntity<TId, TEntity>> enumerable)
        where TId : IEquatable<TId>
        where TEntity : IIdentifiable<TId>
        => enumerable.Select(x => x.Id);
}
