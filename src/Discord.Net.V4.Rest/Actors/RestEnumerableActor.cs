using Discord.Models;
using Discord.Rest.Actors;
using System.Collections.Immutable;

namespace Discord.Rest;

public static class RestEnumerableIndexableActor
{
    public static RestEnumerableIndexableActor<
        TTraitActor,
        TId,
        TEntity,
        TCore,
        IEnumerable<TModel>
    > CreateTrait<
        TTraitActor,
        TId,
        TEntity,
        TCore,
        TApiModel,
        TModel
    >(
        DiscordRestClient client,
        Func<TId, TTraitActor> actorFactory,
        Func<IEnumerable<TModel>, IEnumerable<TEntity>> factory,
        IApiOutRoute<TApiModel> route,
        Func<TApiModel, IEnumerable<TModel>> transform
    )
        where TEntity : RestEntity<TId>, IEntity<TId, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TTraitActor : class, IRestTrait<TId, TEntity>, IActor<TId, TCore>
        where TCore : class, IEntity<TId, TModel>
        where TApiModel : class
    {
        return new RestEnumerableIndexableActor<TTraitActor, TId, TEntity, TCore, TApiModel, IEnumerable<TModel>>(
            client,
            actorFactory,
            factory,
            route,
            transform
        );
    }

    public static RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>>
        Create<TActor, TId, TEntity, TCore, TApiModel, TModel>(
            DiscordRestClient client,
            Func<TId, TActor> actorFactory,
            Func<IEnumerable<TModel>, IEnumerable<TEntity>> factory,
            IApiOutRoute<TApiModel> route,
            Func<TApiModel, IEnumerable<TModel>> transform
        )
        where TEntity : RestEntity<TId>, TCore
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IRestActor<TId, TEntity>, IActor<TId, TCore>
        where TCore : class, IEntity<TId, TModel>
        where TApiModel : class
    {
        return new RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, TApiModel, IEnumerable<TModel>>(
            client,
            actorFactory,
            factory,
            route,
            transform
        );
    }

    public static RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>>
        Create<TActor, TId, TEntity, TCore, TModel>(
            DiscordRestClient client,
            Func<TId, TActor> actorFactory,
            Func<IEnumerable<TModel>, IEnumerable<TEntity>> factory,
            IApiOutRoute<IEnumerable<TModel>> route
        )
        where TEntity : RestEntity<TId>, TCore
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IRestActor<TId, TEntity>, IActor<TId, TCore>
        where TCore : class, IEntity<TId, TModel>
    {
        return new RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>>(
            actorFactory,
            factory,
            (options, token) => FetchAsync(client, route, options, token)
        );
    }

    internal static Task<IEnumerable<TModel>?> FetchAsync<TModel>(
        DiscordRestClient client,
        IApiOutRoute<IEnumerable<TModel>> route,
        RequestOptions? options,
        CancellationToken token)
        where TModel : class, IEntityModel
    {
        return client.RestApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token);
    }
}

internal sealed class RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, TApiModel, TApi>(
    DiscordRestClient client,
    Func<TId, TActor> actorFactory,
    Func<TApi, IEnumerable<TEntity>> factory,
    IApiOutRoute<TApiModel> route,
    Func<TApiModel, TApi> transform
) :
    RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, TApi>(
        actorFactory,
        factory,
        (options, token) => FetchAsync(client, route, transform, options, token)
    )
    where TEntity : RestEntity<TId>, IEntity<TId, IEntityModel<TId>>
    where TId : IEquatable<TId>
    where TApi : class
    where TActor : class, IRestActor<TId, TEntity>, IActor<TId, TCore>
    where TCore : class, IEntity<TId, IEntityModel<TId>>
    where TApiModel : class
{
    private static async Task<TApi?> FetchAsync(
        DiscordRestClient client,
        IApiOutRoute<TApiModel> route,
        Func<TApiModel, TApi> transform,
        RequestOptions? options,
        CancellationToken token)
    {
        var model = await client.RestApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token);

        return model is not null ? transform(model) : null;
    }
}

public class RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, TApi>(
    Func<TId, TActor> actorFactory,
    Func<TApi, IEnumerable<TEntity>> factory,
    Func<RequestOptions?, CancellationToken, Task<TApi?>> fetch
) :
    IEnumerableIndexableActor<TActor, TId, TCore>
    where TEntity : RestEntity<TId>, IEntity<TId, IEntityModel<TId>>
    where TId : IEquatable<TId>
    where TApi : class
    where TActor : class, IRestActor<TId, TEntity>, IActor<TId, TCore>
    where TCore : class, IEntity<TId, IEntityModel<TId>>
{
    public TActor this[TId id] => Specifically(id);
    internal RestIndexableActor<TActor, TId, TEntity> IndexableActor { get; } = new(actorFactory);

    public virtual async ValueTask<IReadOnlyCollection<TEntity>> AllAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await fetch(options, token);

        if (model is null)
            return [];

        return factory(model).ToImmutableArray();
    }

    public virtual TActor Specifically(TId id) => IndexableActor.Specifically(id);

    async ValueTask<IReadOnlyCollection<TCore>> IEnumerableActor<TId, TCore>.AllAsync(
        RequestOptions? options,
        CancellationToken token)
    {
        var result = await AllAsync(options, token);

        if (result is IReadOnlyCollection<TCore> core) return core;

        return result.OfType<TCore>().ToList().AsReadOnly();
    }
}
