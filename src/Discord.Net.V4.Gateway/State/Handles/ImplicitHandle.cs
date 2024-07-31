using Discord.Gateway;
using Discord.Models;

namespace Discord.Gateway.State.Handles;

internal sealed class ImplicitHandle<TId, TEntity, TActor, TModel> : IEntityHandle<TId, TEntity>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    public TId Id { get; }

    public TEntity Entity =>
        _weakReference?.TryGetTarget(out var target) ?? false
            ? target
            : throw new InvalidOperationException("The implicit handle was disposed of");

    private WeakReference<TEntity>? _weakReference;
    private IEntityBroker<TId, TEntity>? _broker;
    private bool _disposed;

    public ImplicitHandle(
        DiscordGatewayClient client,
        IEntityBroker<TId, TEntity> broker,
        CachePathable path,
        TId id,
        TModel model,
        TActor? actor,
        out TEntity entity)
    {
        _broker = broker;
        Id = id;

        ICacheConstructionContext<TId, TEntity> context = actor is not null
            ? new CacheConstructionContext<TId, TEntity, TActor>(actor, path, this)
            : new CacheConstructionContext<TId, TEntity>(path, this);

        entity = TEntity.Construct(client, context, model);
        _weakReference = new(entity);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        _weakReference = null!;

        if (_broker is null) return;

        // notify broker that we've released the handle
        await _broker.ReleaseHandleAsync(this);

        _broker = null;
    }
}
