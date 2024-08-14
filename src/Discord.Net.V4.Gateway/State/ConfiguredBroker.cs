using Discord.Models;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway.State;

internal readonly struct ConfiguredBroker<TId, TEntity, TActor, TModel>(
    IStoreInfo<TId, TModel> storeInfo,
    IEntityBroker<TId, TEntity, TActor, TModel> broker
)
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TActor : class, IGatewayCachedActor<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(
        TModel model,
        CachePathable path,
        TActor? actor = null,
        CancellationToken token = default
    ) => broker.CreateAsync(model, path, actor, token);

    public ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        TActor? actor = null,
        CancellationToken token = default
    ) => broker.GetAsync(path, identity, storeInfo, actor, token);

    public ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(
        TModel model,
        CachePathable path,
        CancellationToken token = default
    ) => broker.CreateAsync(model, path, token);

    public ValueTask AttachLatentEntityAsync(
        TId id,
        TEntity entity,
        CancellationToken token
    ) => broker.AttachLatentEntityAsync(id, entity, storeInfo, token);

    public bool TryCreateLatentHandle(
        TModel model,
        [MaybeNullWhen(true)] out TEntity entity,
        [MaybeNullWhen(false)] out IDisposable handle,
        CancellationToken token
    ) => broker.TryCreateLatentHandle(model, out entity, out handle, token);

    public ValueTask UpdateAsync(TModel model, CancellationToken token)
        => broker.UpdateAsync(model, storeInfo, token);
    
    public ValueTask UpdateAsync(IPartial<TModel> model, CancellationToken token)
        => broker.UpdateAsync(model, storeInfo, token);

    public ValueTask BatchUpdateAsync(
        IEnumerable<TModel> models,
        CancellationToken token
    ) => broker.BatchUpdateAsync(models, storeInfo, token);
    
    public ValueTask BatchUpdateAsync(
        IEnumerable<IPartial<TModel>> models,
        CancellationToken token
    ) => broker.BatchUpdateAsync(models, storeInfo, token);

    public ValueTask<IEnumerable<IEntityHandle<TId, TEntity>>> BatchCreateOrUpdateAsync(
        CachePathable path,
        IEnumerable<TModel> models,
        CancellationToken token = default
    ) => broker.BatchCreateOrUpdateAsync(path, models, storeInfo, token);

    public ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        CancellationToken token = default
    ) => broker.GetAsync(path, identity, storeInfo, token);

    public IAsyncEnumerable<IEntityHandle<TId, TEntity>> GetAllAsync(
        CachePathable path,
        CancellationToken token = default
    ) => broker.GetAllAsync(path, storeInfo, token);

    public IAsyncEnumerable<IEntityHandle<TId, TEntity>> QueryAsync(
        CachePathable path,
        TId from,
        Optional<TId> to,
        Direction direction,
        int? limit = null,
        CancellationToken token = default
    ) => broker.QueryAsync(path, storeInfo, from, to, direction, limit, token);

    public IAsyncEnumerable<TId> GetAllIdsAsync(
        CancellationToken token = default
    ) => broker.GetAllIdsAsync(storeInfo, token);
}
