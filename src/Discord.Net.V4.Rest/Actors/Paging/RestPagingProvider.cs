using Discord.Models;
using MorseCode.ITask;

namespace Discord.Rest;

internal sealed class RestPagingProvider<TModel, TParams, TEntity>(
    DiscordRestClient client,
    Func<TModel, IEnumerable<TModel>, TEntity> entityFactory,
    IPathable? path = null
):
    IPagingProvider<TParams, TEntity, TModel>
    where TModel : IModel
    where TParams : class, IPagingParams<TParams, IEnumerable<TModel>>
    where TEntity : class, IEntityOf<TModel>
{
    public IAsyncPaged<TEntity> CreatePagedAsync(TParams? pageParams = default, RequestOptions? options = null)
    {
        return new RestPager<TEntity, TModel, IEnumerable<TModel>, TParams>(
            client,
            path ?? IPathable.Empty,
            options ?? client.DefaultRequestOptions,
            m => m,
            entityFactory,
            pageParams
        );
    }
}

internal sealed class RestPagingProvider<TModel, TApiModel, TParams, TEntity>(
    DiscordRestClient client,
    Func<TApiModel, IEnumerable<TModel>> mapper,
    Func<TModel, TApiModel, TEntity> entityFactory,
    IPathable? path = null
) :
    IPagingProvider<TParams, TEntity, TModel>
    where TModel : IModel
    where TParams : class, IPagingParams<TParams, TApiModel>
    where TApiModel : class
    where TEntity : class, IEntityOf<TModel>
{
    public IAsyncPaged<TEntity> CreatePagedAsync(TParams? pageParams = default, RequestOptions? options = null)
    {
        return new RestPager<TEntity, TModel, TApiModel, TParams>(
            client,
            path ?? IPathable.Empty,
            options ?? client.DefaultRequestOptions,
            mapper,
            entityFactory,
            pageParams
        );
    }
}

internal static class RestPagingProvider
{
    public static RestPagingProvider<TModel, IEnumerable<TModel>, TParams, TEntity> FromActor
        <TId, TActor, TModel, TParams, TEntity>
        (
            RestActor<TActor, TId, TEntity, TModel> actor,
            Template<TParams> paramsTemplate
        )
        where TModel : class, IEntityModel<TId>
        where TParams : class, IPagingParams<TParams, IEnumerable<TModel>>
        where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
        where TId : IEquatable<TId>
        where TActor : RestActor<TActor, TId, TEntity, TModel>
    {
        return new RestPagingProvider<TModel, IEnumerable<TModel>, TParams, TEntity>(
            actor.Client,
            m => m,
            (model, _) => TEntity.Construct(actor.Client, (TActor)actor, model)
        );
    }
}