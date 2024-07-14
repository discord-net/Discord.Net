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
        // TODO: fix this covariant problem
        // IRelationship<ulong, RestGuild, ...>;
        // IRelationship<ulong, IGuild, ...>
        if (this is not IRelationship<TId, TEntity, ILoadableEntity<TId, TEntity>> relationship)
            throw new KeyNotFoundException($"Cannot find path from {GetType().Name} to {typeof(TEntity).Name}");

        return relationship.RelationshipLoadable.Id;
    }

    bool TryGet<TId, TEntity>(out TId? id)
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        id = default;

        if (this is not IRelationship<TId, TEntity, ILoadableEntity<TId, TEntity>> relationship)
            return false;

        id = relationship.RelationshipLoadable.Id;
        return true;
    }
}
