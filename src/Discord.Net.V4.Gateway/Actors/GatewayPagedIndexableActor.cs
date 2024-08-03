using Discord.Models;
using Discord.Paging;

namespace Discord.Gateway;

public sealed class GatewayPagedIndexableActor<TActor, TId, TEntity, TPagedEntity, TPageParams, TModel>(
    DiscordGatewayClient client,
    Func<TId, TActor> actorFactory,
    Func<TPageParams?, IApiOutRoute<TModel>> initial,
    Func<EntityPager<TPagedEntity, TModel>, TModel, IEnumerable<TPagedEntity>> factory,
    Func<EntityPager<TPagedEntity, TModel>, TModel, TPageParams?, IApiOutRoute<TModel>?> nextPage,
    int? defaultPageSize = null
) :
    GatewayPagedActor<TId, TPagedEntity, TModel, TPageParams>(client, initial, factory, nextPage, defaultPageSize),
    IPagedIndexableActor<TActor, TId, TEntity, TPagedEntity, TPageParams>
    where TActor : class, IGatewayActor<TId, TEntity, IIdentifiable<TId>>
    where TEntity : GatewayEntity<TId>
    where TId : IEquatable<TId>
    where TPagedEntity : class, IEntity<TId>
    where TPageParams : IPagingParams
    where TModel : class
{
    public TActor this[TId id] => Specifically(id);

    private readonly WeakDictionary<TId, TActor> _cache = new();

    public TActor Specifically(TId id)
        => _cache.GetOrAdd(id, actorFactory);
}

public sealed class GatewayPagedIndexableActor<TActor, TId, TEntity, TParams, TModel>(
    DiscordGatewayClient client,
    Func<TId, TActor> actorFactory,
    Func<TParams?, IApiOutRoute<TModel>> initial,
    Func<EntityPager<TEntity, TModel>, TModel, IEnumerable<TEntity>> factory,
    Func<EntityPager<TEntity, TModel>, TModel, TParams?, IApiOutRoute<TModel>?> nextPage,
    int? defaultPageSize = null
) :
    GatewayPagedActor<TId, TEntity, TModel, TParams>(
        client,
        initial,
        factory,
        nextPage,
        defaultPageSize
    ),
    IPagedIndexableActor<TActor, TId, TEntity, TParams>
    where TActor : class, IGatewayActor<TId, TEntity, IIdentifiable<TId>>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>
    where TModel : class
    where TParams : IPagingParams
{
    public TActor this[TId id] => Specifically(id);

    private readonly WeakDictionary<TId, TActor> _cache = new();

    public TActor Specifically(TId id)
        => _cache.GetOrAdd(id, actorFactory);
}

public class GatewayPagedActor<TId, TEntity, TModel, TParams>(
    DiscordGatewayClient client,
    Func<TParams?, IApiOutRoute<TModel>> initial,
    Func<EntityPager<TEntity, TModel>, TModel, IEnumerable<TEntity>> factory,
    Func<EntityPager<TEntity, TModel>, TModel, TParams?, IApiOutRoute<TModel>?> nextPage,
    int? defaultPageSize = null
):
    IPagedActor<TId, TEntity, TParams>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>
    where TModel : class
    where TParams : IPagingParams
{
    public IAsyncPaged<TEntity> PagedAsync(TParams? pagingParams = default, RequestOptions? options = null,
        CancellationToken token = default)
        => new EntityPager<TEntity, TModel>(
            client,
            pagingParams?.PageSize ?? defaultPageSize,
            pagingParams?.Total,
            initial(pagingParams),
            factory,
            (pager, model) => nextPage(pager, model, pagingParams),
            options
        );
}
