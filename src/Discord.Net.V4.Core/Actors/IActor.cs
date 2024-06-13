namespace Discord;

public interface IActor<out TId, out TEntity> :
    IClientProvider,
    IPathable,
    IIdentifiable<TId>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;

