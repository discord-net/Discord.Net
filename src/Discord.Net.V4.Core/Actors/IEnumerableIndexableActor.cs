namespace Discord;

public interface IEnumerableIndexableActor<out TActor, in TId, TEntity> :
    IIndexableActor<TActor, TId, TEntity>,
    IEnumerableActor<TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;
