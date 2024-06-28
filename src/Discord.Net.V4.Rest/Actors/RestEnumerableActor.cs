using Discord.Rest.Actors;
using System.Collections.Immutable;

namespace Discord.Rest;

public sealed class RestEnumerableActor<TId, TEntity, TModel>(
    DiscordRestClient client,
    Func<TModel, IEnumerable<TEntity>> factory,
    IApiOutRoute<TModel> route
):
    IEnumerableActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TModel : class
{
    public async Task<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await client.ApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token);

        if (model is null)
            return [];

        return factory(model).ToImmutableArray();
    }
}

public sealed class RestEnumerableIndexableActor<TActor, TId, TEntity, TModel>(
    DiscordRestClient client,
    Func<TId, TActor> actorFactory,
    Func<TModel, IEnumerable<TEntity>> factory,
    IApiOutRoute<TModel> route
):
    IEnumerableIndexableActor<TActor, TId, TEntity>
    where TEntity : RestEntity<TId>
    where TId : IEquatable<TId>
    where TModel : class
    where TActor : IActor<TId, TEntity>
{
    internal RestIndexableActor<TActor, TId, TEntity> IndexableActor { get; } = new(actorFactory);

    public async Task<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await client.ApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token);

        if (model is null)
            return [];

        return factory(model).ToImmutableArray();
    }

    public TActor Specifically(TId id) => IndexableActor.Specifically(id);
}
