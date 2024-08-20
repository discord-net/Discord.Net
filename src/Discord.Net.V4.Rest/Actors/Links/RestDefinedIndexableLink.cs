using Discord.Models;

namespace Discord.Rest;

public sealed class RestDefinedIndexableLink<TActor, TId, TEntity, TModel>(
    DiscordRestClient client,
    IReadOnlyCollection<TId> ids,
    RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink
) :
    IRestClientProvider,
    IDefinedIndexableLink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    public IReadOnlyCollection<TId> Ids { get; internal set; } = ids;

    public DiscordRestClient Client { get; } = client;
    
    public RestDefinedIndexableLink(
        DiscordRestClient client,
        IReadOnlyCollection<TId> ids, 
        Func<TId, TActor> actorFactory)
        : this(client, ids, new RestIndexableLink<TActor, TId, TEntity, TModel>(client, actorFactory))
    {
    }

    public TActor Specifically(TId id) => indexableLink.Specifically(id);

    public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
        => ids.Select(indexableLink.Specifically);
}
