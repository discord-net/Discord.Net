using Discord.Models;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>> Fetchable<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        [Not(nameof(TEntity)), Interface] TCore,
        TModel,
        TIdentity
    >(
        Template<TActor> actor,
        DiscordRestClient client,
        [VariableFuncArgs(InsertAt = 1)] Func<DiscordRestClient, IIdentifiableEntityOrModel<TId, TEntity, TModel>, TActor> actorFactory,
        [VariableFuncArgs(InsertAt = 1)] Func<DiscordRestClient, TModel, TEntity> entityFactory,
        IApiOutRoute<IEnumerable<TModel>> route
    )
        where TEntity : RestEntity<TId>, TCore
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TActor : IRestActor<TId, TEntity, TIdentity>
        where TCore : class, IEntity<TId>, IEntityOf<TModel>
    {
        return new RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>>(
            (id) => actorFactory(client, IIdentifiableEntityOrModel<TId, TEntity, TModel>.Of(id)),
            models => models.Select(x => entityFactory(client, x)),
            (options, token) => client.ApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token)
        );
    }
}
