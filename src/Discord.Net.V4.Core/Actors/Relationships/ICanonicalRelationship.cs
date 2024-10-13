namespace Discord;

public interface ICanonicalRelationship<out TActor, out TId, out TEntity> :
    IRelationship<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;