using Discord.Models;
using Discord.Rest.Actors;
using System.Collections.Immutable;

namespace Discord.Rest;
//
// public class RestEnumerableActor<TId, TEntity, TModel>(
//     DiscordRestClient client,
//     Func<TModel, IEnumerable<TEntity>> factory,
//     IApiOutRoute<TModel> route
// ):
//     IEnumerableActor<TId, TEntity>
//     where TEntity : class, IEntity<TId>
//     where TId : IEquatable<TId>
//     where TModel : class
// {
//     public async ValueTask<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default)
//     {
//         var model = await client.ApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token);
//
//         if (model is null)
//             return [];
//
//         return factory(model).ToImmutableArray();
//     }
// }

public static class RestEnumerableIndexableActor
{
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
        where TActor : IActor<TId, TEntity>
        where TCore : class, IEntity<TId>
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
        where TActor : IActor<TId, TEntity>
        where TCore : class, IEntity<TId>
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

internal sealed class RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, TApiModel, TModel>(
    DiscordRestClient client,
    Func<TId, TActor> actorFactory,
    Func<TModel, IEnumerable<TEntity>> factory,
    IApiOutRoute<TApiModel> route,
    Func<TApiModel, TModel> transform
):
    RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, TModel>(actorFactory,
        factory,
        (options, token) => FetchAsync(client, route, transform, options, token))
    where TEntity : RestEntity<TId>, TCore
    where TId : IEquatable<TId>
    where TModel : class
    where TActor : IActor<TId, TEntity>
    where TCore : class, IEntity<TId>
    where TApiModel : class
{
    private static async Task<TModel?> FetchAsync(
        DiscordRestClient client,
        IApiOutRoute<TApiModel> route,
        Func<TApiModel, TModel> transform,
        RequestOptions? options,
        CancellationToken token)
    {
        var model = await client.RestApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token);

        return model is not null ? transform(model) : null;
    }
}

public class RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, TModel>(
    Func<TId, TActor> actorFactory,
    Func<TModel, IEnumerable<TEntity>> factory,
    Func<RequestOptions?, CancellationToken, Task<TModel?>> fetch
) :
    IEnumerableIndexableActor<TActor, TId, TCore>
    where TEntity : RestEntity<TId>, TCore
    where TId : IEquatable<TId>
    where TModel : class
    where TActor : IActor<TId, TEntity>
    where TCore : class, IEntity<TId>
{
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
        CancellationToken token
    ) => await AllAsync(options, token);
}
