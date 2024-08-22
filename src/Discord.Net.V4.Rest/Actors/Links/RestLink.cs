using Discord.Models;

namespace Discord.Rest;

public readonly struct RestActorProvider<TId, TActor>(
    Func<DiscordRestClient, TId, TActor> actorFactory
) : 
    IDisposable
    where TActor : class
    where TId : IEquatable<TId>
{
    private readonly WeakDictionary<TId, TActor> _actorCache = new();

    public TActor GetActor(DiscordRestClient client, TId id)
        => _actorCache.GetOrAdd(id, actorFactory, client);

    public void Dispose()
    {
        _actorCache.Clear();
    }
        
    public static implicit operator RestActorProvider<TId, TActor>(Func<DiscordRestClient, TId, TActor> actorFactory) 
        => new(actorFactory);
}

public partial class RestLink<TActor, TId, TEntity, TModel> :
    IRestClientProvider,
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IRestActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TModel : IEntityModel<TId>
{
    public DiscordRestClient Client { get; }

    internal RestActorProvider<TId, TActor> Provider;

    internal RestLink(DiscordRestClient client, RestActorProvider<TId, TActor> provider)
    {
        Client = client;
        Provider = provider;
    }

    protected TActor GetActor(TId id) => Provider.GetActor(Client, id);

    [SourceOfTruth]
    internal TEntity CreateEntity(TModel model)
        => TEntity.Construct(Client, GetActor(model.Id), model);
}