using Discord.Gateway.State;

namespace Discord.Gateway;

public sealed record GatewayConstructionContext(
    CachePathable Path
) : IGatewayConstructionContext;

public sealed record GatewayConstructionContext<TActor>(
    TActor Actor,
    CachePathable Path
) : IGatewayConstructionContext<TActor>
    where TActor : class;

public interface IGatewayConstructionContext<out TActor> :
    IGatewayConstructionContext
    where TActor : class
{
    TActor Actor { get; }
}

public interface IGatewayConstructionContext
{
    CachePathable Path { get; }
}

internal static class GatewayConstructionContextExtensions
{
    public static TActor? TryGetActor<TActor>(
        this IGatewayConstructionContext context
    )
        where TActor : class
    {
        if (context is IGatewayConstructionContext<TActor> actorContext)
            return actorContext.Actor;
        return null;
    }

    public static T? TryMapActor<TId, TEntity, TActor, T>(
        this IGatewayConstructionContext context,
        Template<TActor> template,
        Func<TActor, T> mapper
    )
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        where TActor : class, IActor<TId, TEntity>
        where T : class
    {
        if (context is IGatewayConstructionContext<TActor> actorContext)
            return mapper(actorContext.Actor);
        return null;
    }
}
