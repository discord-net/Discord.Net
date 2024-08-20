using Discord.Models;

namespace Discord.Rest;

[BackLinkable]
public partial class RestIndexableLink<TActor, TId, TEntity, TModel>(
    DiscordRestClient client,
    Func<TId, TActor> actorFactory
) :
    IRestClientProvider,
    IIndexableLink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    public DiscordRestClient Client { get; } = client;

    internal Func<TId, TActor> ActorFactory { get; } = actorFactory;
    
    public TActor this[TId id] => Specifically(id);
    
    private readonly WeakDictionary<TId, TActor> _cache = new();
    
    public TActor Specifically(TId id)
        => _cache.GetOrAdd(id, ActorFactory);
}
