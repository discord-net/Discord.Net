namespace Discord.EntityRelationships;

public interface ILoadableRelationship<TId, TEntity> : IRelationship<TId, TEntity, ILoadableEntity<TId, TEntity>>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>;

public interface IRelationship<TId, TEntity, out TLoadable>
    where TLoadable : ILoadableEntity<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>
{
    TLoadable RelationshipLoadable { get; }
}
