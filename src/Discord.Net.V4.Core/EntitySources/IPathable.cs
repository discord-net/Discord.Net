using Discord.EntityRelationships;

namespace Discord;

public interface IPathable
{
    ulong Require<TEntity>()
        where TEntity : class, IEntity<ulong>
        => Require<ulong, TEntity>();

    TId Require<TId, TEntity>()
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        if (this is not IRelationship<TId, TEntity, ILoadableEntity<TId, TEntity>> relationship)
            throw new KeyNotFoundException($"Cannot find path from {GetType().Name} to {typeof(TEntity).Name}");

        return relationship.RelationshipLoadable.Id;
    }
}
