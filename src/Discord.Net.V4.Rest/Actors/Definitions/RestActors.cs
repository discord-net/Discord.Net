using Discord.Models;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>> FromFetchable<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        [Not(nameof(TEntity)), Interface] TCore,
        TModel,
        TPathable
    >(
        Template<TActor> actor,
        TPathable pathable,
        [VariableFuncArgs(InsertAt = 1)]
        Func<DiscordRestClient, IIdentifiable<TId, TEntity, TActor, TModel>, TActor> actorFactory,
        [VariableFuncArgs(InsertAt = 1)] Func<DiscordRestClient, TModel, TEntity> entityFactory
    )
        where TPathable : IRestClientProvider, IPathable
        where TEntity : RestEntity<TId>, TCore
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IRestActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>>
        where TCore : class, IEntity<TId, TModel>, IFetchableOfMany<TId, TModel>
    {
        return Fetchable<TActor, TId, TEntity, TCore, TModel>(
            actor,
            pathable.Client,
            actorFactory,
            entityFactory,
            TCore.FetchManyRoute(pathable)
        );
    }

    public static RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>> Fetchable<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        [Not(nameof(TEntity)), Interface, Shrink] TCore,
        TModel
    >(
        Template<TActor> actor,
        DiscordRestClient client,
        [VariableFuncArgs(InsertAt = 1)]
        Func<DiscordRestClient, IIdentifiable<TId, TEntity, TActor, TModel>, TActor> actorFactory,
        [VariableFuncArgs(InsertAt = 1)] Func<DiscordRestClient, TModel, TEntity> entityFactory,
        IApiOutRoute<IEnumerable<TModel>> route
    )
        where TEntity : RestEntity<TId>, TCore
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IRestActor<TId, TEntity>
        where TCore : class, IEntity<TId, TModel>
    {
        return new RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>>(
            (id) => actorFactory(client, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),
            models => models.Select(x => entityFactory(client, x)),
            (options, token) => client.RestApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token)
        );
    }
}