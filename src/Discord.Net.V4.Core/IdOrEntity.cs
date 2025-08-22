namespace Discord.Models;

public readonly record struct IdOrEntity<TId, TEntity>(TId Id)
    where TEntity : IIdentifiable<TId>
    where TId : IEquatable<TId>
{
    public IdOrEntity(TEntity entity) : this(entity.Id)
    {
    }

    public static implicit operator IdOrEntity<TId, TEntity>(TId id) => new(id);
}