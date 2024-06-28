using Discord.Models;
using Discord.Models.Json;
using Discord.Paging;
using Discord.Rest.Actors;
using Discord.Rest.Channels;

namespace Discord.Rest;

public sealed class RestPagedIndexableActor<TActor, TId, TEntity, TModel, TParams>(
    DiscordRestClient client,
    Func<TId, TActor> actorFactory,
    Func<TParams?, IApiOutRoute<TModel>> initial,
    Func<EntityPager<TEntity, TModel>, TModel, IEnumerable<TEntity>> factory,
    Func<EntityPager<TEntity, TModel>, TModel, TParams?, IApiOutRoute<TModel>?> nextPage,
    int? defaultPageSize = null
):
    RestPagedActor<TId, TEntity, TModel, TParams>(client, initial, factory, nextPage, defaultPageSize),
    IPagedIndexableActor<TActor, TId, TEntity, TParams>
    where TActor : IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TModel : class
    where TParams : IPagingParams
{
    internal RestIndexableActor<TActor, TId, TEntity> IndexerActor { get; } = new(actorFactory);
    public TActor Specifically(TId id) => IndexerActor.Specifically(id);
}

public class RestPagedActor<TId, TEntity, TModel, TParams>(
    DiscordRestClient client,
    Func<TParams?, IApiOutRoute<TModel>> initial,
    Func<EntityPager<TEntity, TModel>, TModel, IEnumerable<TEntity>> factory,
    Func<EntityPager<TEntity, TModel>, TModel, TParams?, IApiOutRoute<TModel>?> nextPage,
    int? defaultPageSize = null
):
    IPagedActor<TId, TEntity, TParams>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TModel : class
    where TParams : IPagingParams
{

    public IAsyncPaged<TEntity> PagedAsync(TParams? pagingParams = default, RequestOptions? options = null)
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
