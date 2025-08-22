namespace Discord.Models;

public interface IActor<out TId, out TEntity> :
    IClientProvider,
    IIdentifiable<TId>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;