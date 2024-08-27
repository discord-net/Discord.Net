using Discord.Models;

namespace Discord.Rest;

public sealed class RestActorProvider<TId, TActor>(
    Func<DiscordRestClient, TId, TActor> actorFactory
) :
    IActorProvider<DiscordRestClient, TActor, TId>,
    IDisposable
    where TActor : class, IActor<TId, IEntity<TId>>
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
    IActorProvider<DiscordRestClient, TActor, TId>,
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IRestActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TModel : IEntityModel<TId>
{
    public DiscordRestClient Client { get; }

    internal IActorProvider<DiscordRestClient, TActor, TId> Provider;

    internal RestLink(DiscordRestClient client, IActorProvider<DiscordRestClient, TActor, TId> provider)
    {
        Client = client;
        Provider = provider;
    }

    protected TActor GetActor(TId id) => Provider.GetActor(Client, id);

    [SourceOfTruth]
    internal TEntity CreateEntity(TModel model)
        => TEntity.Construct(Client, GetActor(model.Id), model);

    TActor IActorProvider<IDiscordClient, TActor, TId>.GetActor(IDiscordClient client, TId id)
        => client is DiscordRestClient restClient ? Provider.GetActor(restClient, id) : GetActor(id);

    TActor IActorProvider<DiscordRestClient, TActor, TId>.GetActor(DiscordRestClient client, TId id)
        => Provider.GetActor(Client, id);
}