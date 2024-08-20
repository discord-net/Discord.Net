using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Rest;

[BackLinkable]
public partial class RestEnumerableIndexableLink<TActor, TId, TEntity, TCore, TModel>(
    DiscordRestClient client,
    RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink,
    ApiModelProviderDelegate<IEnumerable<TModel>> apiProvider
) :
    IRestClientProvider,
    IEnumerableIndexableLink<TActor, TId, TCore, TModel>
    where TEntity :
    RestEntity<TId>,
    IEntity<TId, TModel>,
    IRestConstructable<TEntity, TActor, TModel>
    where TActor :
    class,
    IRestActor<TId, TEntity>,
    IActor<TId, TCore>,
    IEntityProvider<TEntity, TModel>,
    IEntityProvider<TCore, TModel>
    where TId : IEquatable<TId>
    where TCore : class, IEntity<TId, TModel>
    where TModel : IEntityModel<TId>
{
    public DiscordRestClient Client { get; } = client;
    public TActor this[TId id] => Specifically(id);
    public TActor Specifically(TId id) => IndexableLink.Specifically(id);

    internal RestIndexableLink<TActor, TId, TEntity, TModel> IndexableLink { get; } = indexableLink;

    internal RestEnumerableLink<TActor, TId, TEntity, TCore, TModel> EnumerableLink { get; } =
        new(client, indexableLink, apiProvider);

    public RestEnumerableIndexableLink(
        DiscordRestClient client,
        Func<TId, TActor> actorFactory,
        ApiModelProviderDelegate<IEnumerable<TModel>> apiProvider
    ) : this(client, new RestIndexableLink<TActor, TId, TEntity, TModel>(client, actorFactory), apiProvider)
    {
    }

    public ValueTask<IReadOnlyCollection<TEntity>> AllAsync(
        RequestOptions? options = null,
        CancellationToken token = default
    ) => EnumerableLink.AllAsync(options, token);

    async ValueTask<IReadOnlyCollection<TCore>> IEnumerableLink<TActor, TId, TCore, TModel>.AllAsync(
        RequestOptions? options,
        CancellationToken token)
    {
        var result = await AllAsync(options, token);

        if (result is IReadOnlyCollection<TCore> core) return core;

        return result.OfType<TCore>().ToList().AsReadOnly();
    }
}