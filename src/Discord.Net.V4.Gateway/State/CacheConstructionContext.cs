namespace Discord.Gateway.State;

public sealed record CacheConstructionContext<TId, TEntity>(
    IPathable Path,
    IEntityHandle<TId, TEntity>? ImplicitHandle = null
) : ICacheConstructionContext<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;

public sealed record CacheConstructionContext<TId, TEntity, TActor>(
    TActor Actor,
    IPathable Path,
    IEntityHandle<TId, TEntity>? ImplicitHandle = null
) : ICacheConstructionContext<TId, TEntity, TActor>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;

public interface ICacheConstructionContext<out TId, out TEntity, out TActor> :
    ICacheConstructionContext<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TActor : IActor<TId, TEntity>
{
    TActor Actor { get; }
}

public interface ICacheConstructionContext<out TId, out TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    IPathable Path { get; }
    IEntityHandle<TId, TEntity>? ImplicitHandle { get; }
}

internal static class CacheConstructionContextExtensions
{
    public static TActor? TryGetActor<TId, TEntity, TActor>(this ICacheConstructionContext<TId, TEntity> context,
        Template<TActor> template)
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        if (context is ICacheConstructionContext<TId, TEntity, TActor> actorContext)
            return actorContext.Actor;
        return null;
    }

    public static T? TryMapActor<TId, TEntity, TActor, T>(
        this ICacheConstructionContext<TId, TEntity> context,
        Template<TActor> template,
        Func<TActor, T> mapper
    )
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        where TActor : class, IActor<TId, TEntity>
        where T : class
    {
        if (context is ICacheConstructionContext<TId, TEntity, TActor> actorContext)
            return mapper(actorContext.Actor);
        return null;
    }

}
