using Discord.Models;
using Discord.Models.Json;
using Discord.Paging;
using Discord.Rest.Actors;
using Discord.Rest.Channels;

namespace Discord.Rest;

public sealed class RestPagedIndexableActor<TActor, TId, TEntity, TModel>(
    DiscordRestClient client,
    Func<TId, TActor> actorFactory,
    IApiOutRoute<TModel> initial,
    Func<EntityPager<TEntity, TModel>, TModel, IEnumerable<TEntity>> factory,
    Func<EntityPager<TEntity, TModel>, TModel, IApiOutRoute<TModel>?> nextPage,
    int? defaultPageSize = null
):
    RestPagedActor<TId, TEntity, TModel>(client, initial, factory, nextPage, defaultPageSize),
    IIndexableActor<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TModel : class
{
    internal RestIndexableActor<TActor, TId, TEntity> IndexerActor { get; } = new(actorFactory);
    public TActor Specifically(TId id) => IndexerActor.Specifically(id);
}

public class RestPagedActor<TId, TEntity, TModel>(
    DiscordRestClient client,
    IApiOutRoute<TModel> initial,
    Func<EntityPager<TEntity, TModel>, TModel, IEnumerable<TEntity>> factory,
    Func<EntityPager<TEntity, TModel>, TModel, IApiOutRoute<TModel>?> nextPage,
    int? defaultPageSize = null
):
    IPagedActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TModel : class
{

    public IAsyncPaged<TEntity> PagedAsync(int? pageSize, RequestOptions? options = null)
        => new EntityPager<TEntity, TModel>(client, pageSize ?? defaultPageSize, initial, factory, nextPage, options);
}
