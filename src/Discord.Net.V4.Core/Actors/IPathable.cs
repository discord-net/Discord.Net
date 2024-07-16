namespace Discord;

public interface IPathable
{
    ulong Require<TEntity>()
        where TEntity : class, IEntity<ulong>
        => Require<ulong, TEntity>();

    ulong? Optionally<TEntity>()
        where TEntity : class, IEntity<ulong>
        => TryGet<ulong, TEntity>(out var id) ? id : null;

    TId Require<TId, TEntity>()
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        if (this is not IRelation<TId, TEntity> relationship)
            throw new KeyNotFoundException($"Cannot find path from {GetType().Name} to {typeof(TEntity).Name}");

        return relationship.Id;
    }

    bool TryGet<TId, TEntity>(out TId? id)
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        id = default;

        if (this is not IRelation<TId, TEntity> relationship)
            return false;

        id = relationship.Id;
        return true;
    }
}
