using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Rest.Actors;

public static partial class RestDefinedEnumerableActor
{
    public static RestDefinedEnumerableActor<TActor, TId, TEntity, TCoreEntity, IEnumerable<TModel>> Create<
        [TransitiveFill]TActor,
        TId,
        TEntity,
        [Not(nameof(TEntity)), Interface]TCoreEntity,
        TModel,
        [TransitiveFill] TSource
    >(
        TSource source,
        IEnumerable<TId> ids,
        [VariableFuncArgs(InsertAt = 1)] Func<DiscordRestClient, IIdentifiable<TId, TEntity, TActor, TModel>, TActor> actorFactory,
        [VariableFuncArgs(InsertAt = 1)] Func<DiscordRestClient, TModel, TEntity> entityFactory
     )
        where TSource : IRestClientProvider, IPathable
        where TActor :
            class,
            IRestActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>>
        where TId : IEquatable<TId>
        where TEntity : RestEntity<TId>, TCoreEntity
        where TCoreEntity : class, IEntity<TId>, IEntityOf<TModel>, IFetchableOfMany<TId, TModel>
        where TModel : class, IEntityModel<TId>
    {
        return new RestDefinedEnumerableActor<TActor, TId, TEntity, TCoreEntity, IEnumerable<TModel>>(
            ids,
            id => actorFactory(source.Client, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),
            models => models.Select(model => entityFactory(source.Client, model)),
            (options, token) => source.Client.RestApiClient.ExecuteAsync(
                TCoreEntity.FetchManyRoute(source),
                options ?? source.Client.DefaultRequestOptions,
                token
            )
        );
    }
}

public class RestDefinedEnumerableActor<TActor, TId, TEntity, TCoreEntity, TModel> :
    RestEnumerableIndexableActor<TActor, TId, TEntity, TCoreEntity, TModel>,
    IDefinedEnumerableActor<TActor, TId, TEntity>
    where TActor : class, IRestActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, TCoreEntity
    where TCoreEntity : class, IEntity<TId>
    where TModel : class
{
    public IReadOnlyCollection<TId> Ids => _ids;

    private ImmutableArray<TId> _ids;

    internal RestDefinedEnumerableActor(
        IEnumerable<TId> ids,
        Func<TId, TActor> actorFactory,
        Func<TModel, IEnumerable<TEntity>> factory,
        Func<RequestOptions?, CancellationToken, Task<TModel?>> fetch
    ) : base(actorFactory, factory, fetch)
    {
        _ids = ids.ToImmutableArray();
    }

    internal void Update(IEnumerable<TId> ids)
    {
        var immutableIds = ids.ToImmutableArray();
        if (!_ids.SequenceEqual(immutableIds))
            _ids = immutableIds;
    }

    public override async ValueTask<IReadOnlyCollection<TEntity>> AllAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await base.AllAsync(options, token);

        Update(result.Select(x => x.Id));

        return result;
    }

    public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
        => ids.Select(Specifically);
}
