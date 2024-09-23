using Discord.Models;

namespace Discord.Rest;

public sealed class RestActorProvider<TId, TActor>(
    DiscordRestClient client,
    Func<DiscordRestClient, TId, TActor> actorFactory
) :
    IRestClientProvider,
    IActorProvider<TActor, TId>,
    IDisposable
    where TActor : class, IActor<TId, IEntity<TId>>
    where TId : IEquatable<TId>
{
    public DiscordRestClient Client { get; } = client;

    private readonly WeakDictionary<TId, TActor> _actorCache = new();

    public TActor GetActor(TId id)
        => _actorCache.GetOrAdd(id, actorFactory, Client);

    public void Dispose()
    {
        _actorCache.Clear();
    }
}

internal static class RestActorProvider
{
    private static readonly WeakDictionary<Type, object> _rootProviders = new();
    private static readonly WeakDictionary<int, object> _stateProviders = new();

    internal static RestActorProvider<TId, TActor> GetOrCreate<TActor, TId, TEntity, TModel>(
        DiscordRestClient client,
        Template<IIdentifiable<TId, TEntity, TActor, TModel>> template
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor :
        class,
        IActor<TId, TEntity>,
        IFactory<TActor, DiscordRestClient, IIdentifiable<TId, TEntity, TActor, TModel>>
    {
        return (RestActorProvider<TId, TActor>)_rootProviders.GetOrAdd(
            typeof(TActor),
            (client, _) => new RestActorProvider<TId, TActor>(
                client,
                (client, id) => TActor.Factory(client, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id))
            ),
            client
        );
    }

    internal static RestActorProvider<TId, TActor> GetOrCreate<TActor, TId, TEntity, TModel, T1>(
        DiscordRestClient client,
        Template<IIdentifiable<TId, TEntity, TActor, TModel>> template,
        T1 state
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor :
        class,
        IActor<TId, TEntity>,
        IFactory<TActor, DiscordRestClient, T1, IIdentifiable<TId, TEntity, TActor, TModel>>
        => CreateStateful<TActor, TId, TEntity, TModel>(
            client,
            HashCode.Combine(state),
            (client, id) => TActor.Factory(client, state, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id))
        );
    
    internal static RestActorProvider<TId, TActor> GetOrCreate<TActor, TId, TEntity, TModel, T1, T2>(
        DiscordRestClient client,
        Template<IIdentifiable<TId, TEntity, TActor, TModel>> template,
        T1 state1,
        T2 state2
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor :
        class,
        IActor<TId, TEntity>,
        IFactory<TActor, DiscordRestClient, T1, T2, IIdentifiable<TId, TEntity, TActor, TModel>>
        => CreateStateful<TActor, TId, TEntity, TModel>(
            client,
            HashCode.Combine(state1, state2),
            (client, id) => TActor.Factory(client, state1, state2, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id))
        );
    
    internal static RestActorProvider<TId, TActor> GetOrCreate<TActor, TId, TEntity, TModel, T1, T2, T3>(
        DiscordRestClient client,
        Template<IIdentifiable<TId, TEntity, TActor, TModel>> template,
        T1 state1,
        T2 state2,
        T3 state3
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor :
        class,
        IActor<TId, TEntity>,
        IFactory<TActor, DiscordRestClient, T1, T2, T3, IIdentifiable<TId, TEntity, TActor, TModel>>
        => CreateStateful<TActor, TId, TEntity, TModel>(
            client,
            HashCode.Combine(state1, state2, state3),
            (client, id) => TActor.Factory(client, state1, state2, state3, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id))
        );

    private static RestActorProvider<TId, TActor> CreateStateful<TActor, TId, TEntity, TModel>(
        DiscordRestClient client,
        int key,
        Func<DiscordRestClient,TId,TActor> factory
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor :
        class,
        IActor<TId, TEntity>
    {
        return (RestActorProvider<TId, TActor>)_stateProviders.GetOrAdd(
            key,
            (client, _) => new RestActorProvider<TId, TActor>(
                client,
                factory
            ),
            client
        );
    }
}