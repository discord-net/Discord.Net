namespace Discord;

public interface IActor<out TId> :
    IClientProvider,
    IIdentifiable<TId>
    where TId : IEquatable<TId>;

public interface IActor<out TId, out TEntity> : IActor<TId>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;