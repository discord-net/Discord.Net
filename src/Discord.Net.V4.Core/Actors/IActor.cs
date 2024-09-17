namespace Discord;

public interface IActor<out TId, out TEntity> : 
    IAbstractionActor<TId>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;

public interface IAbstractionActor<out TId> :
    IClientProvider,
    IPathable,
    IIdentifiable<TId>
    where TId : IEquatable<TId>;
