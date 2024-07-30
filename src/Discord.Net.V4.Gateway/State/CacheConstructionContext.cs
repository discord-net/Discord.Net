namespace Discord.Gateway.State;

public sealed record CacheConstructionContext<TId, TEntity>(
    IPathable Path,
    IEntityHandle<TId, TEntity>? ImplicitHandle = null
) : ICacheConstructionContext<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;

public interface ICacheConstructionContext<out TId, out TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    IPathable Path { get; }
    IEntityHandle<TId, TEntity>? ImplicitHandle { get; }
}
