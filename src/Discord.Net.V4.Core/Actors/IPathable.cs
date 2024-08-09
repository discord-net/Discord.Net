using System.Diagnostics.CodeAnalysis;

namespace Discord;

file sealed class EmptyPath : IPathable
{
    public static readonly EmptyPath Instance = new();
}

public interface IPathable
{
    static IPathable Empty => EmptyPath.Instance;

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

    bool TryGet<TId, TEntity>([MaybeNullWhen(false)]out TId id)
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
                id = relationship.RelationshipId;
                return true;
            default:
                id = default;
                return false;
        }
    }
}
