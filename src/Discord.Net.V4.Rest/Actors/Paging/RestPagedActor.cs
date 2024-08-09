using Discord.Models;
using Discord.Models.Json;
using Discord.Paging;
using Discord.Rest.Actors;
using Discord.Rest;

namespace Discord.Rest;

public sealed class RestPartialPagedIndexableActor<TActor, TId, TEntity, TPartial, TPartialModel, TApiModel, TParams>(
    DiscordRestClient client,
    Func<TId, TActor> actorFactory,
    IPathable path,
    Func<TApiModel, IEnumerable<TPartialModel>> modelsMapper,
    Func<TPartialModel, TApiModel, TPartial> entityFactory
):
    RestPagedActor<TId, TPartial, TPartialModel, TApiModel, TParams>(client, path, modelsMapper, entityFactory),
    IPagedIndexableActor<TActor, TId, TEntity, TPartial, TParams>
    where TActor : class, IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId, IEntityModel<TId>>
    where TPartial : RestEntity<TId>, IEntity<TId,TPartialModel>
    where TPartialModel : class, IEntityModel<TId>
    where TApiModel : class
    where TParams : class, IPagingParams<TParams, TApiModel>
{
    public TActor this[TId id] => Specifically(id);
    internal RestIndexableActor<TActor, TId, TEntity> IndexerActor { get; } = new(actorFactory);
    public TActor Specifically(TId id) => IndexerActor.Specifically(id);
}

public sealed class RestPagedIndexableActor<TActor, TId, TEntity, TModel, TApiModel, TParams>(
    DiscordRestClient client,
    Func<TId, TActor> actorFactory,
    IPathable path,
    Func<TApiModel, IEnumerable<TModel>> modelsMapper,
    Func<TModel, TApiModel, TEntity> entityFactory
):
    RestPagedActor<TId, TEntity, TModel, TApiModel, TParams>(client, path, modelsMapper, entityFactory),
    IPagedIndexableActor<TActor, TId, TEntity, TParams>
    where TActor : class, IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TApiModel : class
    where TParams : class, IPagingParams<TParams, TApiModel>
{
    public TActor this[TId id] => Specifically(id);
    internal RestIndexableActor<TActor, TId, TEntity> IndexerActor { get; } = new(actorFactory);
    public TActor Specifically(TId id) => IndexerActor.Specifically(id);
}

public class RestPagedActor<TId, TEntity, TModel, TApiModel, TParams>(
    DiscordRestClient client,
    IPathable path,
    Func<TApiModel, IEnumerable<TModel>> modelsMapper,
    Func<TModel, TApiModel, TEntity> entityFactory
):
    IPagedActor<TId, TEntity, TParams>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TModel : class
    where TApiModel : class
    where TParams : class, IPagingParams<TParams, TApiModel>
{
    public IAsyncPaged<TEntity> PagedAsync(TParams? pagingParams = default, RequestOptions? options = null)
    {
        return new RestPager<TId, TEntity, TModel, TApiModel, TParams>(
            client,
            path,
            options ?? client.DefaultRequestOptions,
            modelsMapper,
            entityFactory,
            pagingParams
        );
    }
}
