using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Discord.Gateway.Cache;
using Microsoft.Extensions.Logging;

namespace Discord.Gateway.State;

internal sealed partial class StateController
{
    private ICacheProvider CacheProvider => _client.CacheProvider;

    private readonly DiscordGatewayClient _client;
    private readonly ILogger<StateController> _logger;

    private readonly Dictionary<Type, IEntityBroker> _brokers = [];
    private readonly KeyedSemaphoreSlim<Type> _brokerSemaphore = new(1, 1);

    public StateController(DiscordGatewayClient client, ILogger<StateController> logger)
    {
        _client = client;
        _logger = logger;

        // client.Guilds[123].Members
    }

    public async ValueTask<IEntityBroker<TId, TEntity, TModel>> GetBrokerAsync<TId, TEntity, TModel>(
        CancellationToken token = default
    )
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IContextConstructable<TEntity, TModel, IPathable, DiscordGatewayClient>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        if (TryGetCachedBroker<TId, TEntity, TModel>(out var broker))
            return broker;

        using var scope = _brokerSemaphore.Get(typeof(TEntity), out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (TryGetCachedBroker(out broker))
                return broker;

            broker = new EntityBroker<TId, TEntity, TModel>(_client, this);
            _brokers[typeof(TEntity)] = broker;
            return broker;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private bool TryGetCachedBroker<TId, TEntity, TModel>(
        [MaybeNullWhen(false)] out IEntityBroker<TId, TEntity, TModel> broker)
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IContextConstructable<TEntity, TModel, IPathable, DiscordGatewayClient>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        broker = null;

        if (_brokers.TryGetValue(typeof(TEntity), out var cachedBroker) &&
            cachedBroker is IEntityBroker<TId, TEntity, TModel> value)
            broker = value;

        return broker is not null;
    }

    public async ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync<
        [TransitiveFill] TIdentity,
        TId,
        TEntity,
        TActor,
        TModel
    >(
        Template<TIdentity> template,
        CancellationToken token = default
    )
        where TIdentity : IIdentifiable<TId, TEntity, TActor, TModel>
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TActor : class, IActor<TId, TEntity>
        where TModel : class, IEntityModel<TId>
    {
        return await CacheProvider.GetStoreAsync<TId, TModel>(token);
    }
}
