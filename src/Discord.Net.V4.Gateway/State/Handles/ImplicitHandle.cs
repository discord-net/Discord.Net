using Discord.Gateway.Cache;
using Discord.Models;

namespace Discord.Gateway.State.Handles;

internal sealed class ImplicitHandle<TId, TEntity, TModel> : IEntityHandle<TId, TEntity>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    public TId Id { get; }

    public TEntity Entity { get; private set; }

    private IEntityBroker<TId, TEntity>? _broker;
    private bool _disposed;

    public ImplicitHandle(
        DiscordGatewayClient client,
        IEntityBroker<TId, TEntity> broker,
        IPathable path,
        TId id,
        TModel model)
    {
        _broker = broker;
        Id = id;
        Entity = TEntity.Construct(client, new CacheConstructionContext<TId, TEntity>(path, this), model);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        Entity = null!;

        if (_broker is null) return;

        // notify broker that we've released the handle
        await _broker.ReleaseHandleAsync(this);

        _broker = null;
    }
}
