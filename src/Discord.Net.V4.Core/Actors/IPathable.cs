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
        return this switch
        {
            TEntity entity => entity.Id,
            IActor<TId, TEntity> actor => actor.Id,
            IRelation<TId, TEntity> relationship => relationship.RelationshipId,
            _ => throw new KeyNotFoundException($"Cannot find path from {GetType().Name} to {typeof(TEntity).Name}")
        };
    }

    bool TryGet<TId, TEntity>(out TId? id)
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        switch (this)
        {
            case TEntity entity:
                id = entity.Id;
                return true;
            case IActor<TId, TEntity> actor:
                id = actor.Id;
                return true;
            case IRelation<TId, TEntity> relationship:
                id = relationship.Id;
                return true;
            default:
                id = default;
                return false;
        }
    }
}
