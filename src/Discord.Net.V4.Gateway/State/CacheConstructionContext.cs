namespace Discord.Gateway.State;

public sealed record CacheConstructionContext(
    CachePathable Path
) : ICacheConstructionContext;

public sealed record CacheConstructionContext<TActor>(
    TActor Actor,
    CachePathable Path
) : ICacheConstructionContext<TActor>
    where TActor : class;

public interface ICacheConstructionContext<out TActor> :
    ICacheConstructionContext
    where TActor : class
{
    TActor Actor { get; }
}

public interface ICacheConstructionContext
{
    CachePathable Path { get; }
}

internal static class CacheConstructionContextExtensions
{
    public static TActor? TryGetActor<TActor>(
        this ICacheConstructionContext context
    )
        where TActor : class
    {
        if (context is ICacheConstructionContext<TActor> actorContext)
            return actorContext.Actor;
        return null;
    }

    public static T? TryMapActor<TId, TEntity, TActor, T>(
        this ICacheConstructionContext context,
        Template<TActor> template,
        Func<TActor, T> mapper
    )
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        where TActor : class, IActor<TId, TEntity>
        where T : class
    {
        if (context is ICacheConstructionContext<TActor> actorContext)
            return mapper(actorContext.Actor);
        return null;
    }
}
