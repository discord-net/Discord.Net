using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Rest;

[BackLinkable]
public partial class RestDefinedEnumerableIndexableLink<TActor, TId, TEntity, TCoreEntity, TModel> :
    IRestClientProvider,
    IDefinedEnumerableIndexableLink<TActor, TId, TEntity, TModel>
    where TActor :
    class,
    IRestActor<TId, TEntity>,
    IActor<TId, TCoreEntity>,
    IEntityProvider<TEntity, TModel>,
    IEntityProvider<TCoreEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TCoreEntity : class, IEntity<TId, TModel>
    where TModel : IEntityModel<TId>
{
    public IReadOnlyCollection<TId> Ids => DefinedLink.Ids;

    public TActor this[TId id] => IndexableLink[id];
    
    public DiscordRestClient Client { get; }

    internal RestDefinedLink<TActor, TId, TEntity, TModel> DefinedLink { get; }
    internal RestEnumerableLink<TActor, TId, TEntity, TCoreEntity, TModel> EnumerableLink { get; }
    internal RestIndexableLink<TActor, TId, TEntity, TModel> IndexableLink { get; }

    public RestDefinedEnumerableIndexableLink(
        DiscordRestClient client,
        RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink,
        ApiModelProviderDelegate<IEnumerable<TModel>> apiProvider,
        IReadOnlyCollection<TId> ids)
    {
        Client = client;

        DefinedLink = new RestDefinedLink<TActor, TId, TEntity, TModel>(client, indexableLink, ids);
        EnumerableLink = new(client, indexableLink, apiProvider);
        IndexableLink = indexableLink;
    }

    public ValueTask<IReadOnlyCollection<TEntity>> AllAsync(
        RequestOptions? options = null,
        CancellationToken token = default
    ) => EnumerableLink.AllAsync(options, token);

    public TActor Specifically(TId id)
        => IndexableLink.Specifically(id);

    public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
        => ids.Select(Specifically);
}