using Discord.Gateway;
using Discord.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Discord.Gateway.State;

internal sealed class EntityBroker<TId, TEntity, TActor, TModel> : IEntityBroker<TId, TEntity, TActor, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
{
    private sealed class LatentEntityPromise(IDisposable scope, SemaphoreSlim semaphore) : IDisposable
    {
        public void Dispose()
        {
            scope.Dispose();
            semaphore.Release();
        }
    }

    private sealed class EntityReference : IEntityReference<TId, TEntity>
    {
        private sealed class EntityReferenceCompanion(EntityReference reference)
        {
            ~EntityReferenceCompanion()
            {
                reference.DestroyReference();
            }
        }

        public TId Id { get; }

        public bool IsAlive => !_isKilled && _handle.IsAllocated;

        private bool _isKilled;

        private DependentHandle _handle;
        private readonly EntityBroker<TId, TEntity, TActor, TModel> _broker;
        private readonly List<IEntityHandle<TId, TEntity>> _handles;

        public EntityReference(EntityBroker<TId, TEntity, TActor, TModel> broker,
            TId id,
            TEntity entity,
            params IEntityHandle<TId, TEntity>[] handles)
        {
            _broker = broker;
            Id = id;
            _handles = handles.ToList();
            _handle = new DependentHandle(entity, new EntityReferenceCompanion(this));
        }

        private TEntity? GetReference()
        {
            if (_isKilled) return null;

            return Unsafe.As<TEntity>(
                DependantHandleUtils.UnsafeGetTargetAndDependent(
                    ref _handle
                )
            );
        }

        public bool TryGetReference([MaybeNullWhen(false)] out TEntity entity)
        {
            entity = GetReference();
            return entity is not null;
        }

        public IEntityHandle<TId, TEntity>? AllocateHandle()
        {
            if (!IsAlive) return null;

            var entity = GetReference();

            if (entity is null)
            {
                DestroyReference();
                return null;
            }

            var handle = new EntityHandle<TId, TEntity, TModel>(_broker, Id, entity);
            _handles.Add(handle);
            return handle;
        }

        public bool RemoveHandle(IEntityHandle<TId, TEntity> handle)
            => _handles.Remove(handle);

        public void DestroyReference()
        {
            if (!IsAlive) return;

            _isKilled = true;

            if (_broker.RemoveReference(this)) _handle.Dispose();
        }

        TEntity? IEntityReference<TId, TEntity>.GetReference(out bool isSuccess)
        {
            isSuccess = TryGetReference(out var entity);
            return entity;
        }
    }

    private readonly DiscordGatewayClient _client;
    private readonly StateController _controller;
    private readonly Dictionary<TId, EntityReference> _references;
    private readonly KeyedSemaphoreSlim<TId> _keyedSemaphore;
    private readonly SemaphoreSlim _enumerationSemaphore = new(1, 1);
    private readonly object _syncRoot = new();

    private readonly ILogger<EntityBroker<TId, TEntity, TActor, TModel>> _logger;

    public EntityBroker(
        DiscordGatewayClient client,
        StateController stateController)
    {
        _keyedSemaphore = new(1, 1);
        _client = client;
        _controller = stateController;
        _logger = client.LoggerFactory.CreateLogger<EntityBroker<TId, TEntity, TActor, TModel>>();
        _references = new();
    }

    public async ValueTask UpdateInReferenceEntityAsync(TModel model, CancellationToken token)
    {
        // sanity
        if (model.GetType() != typeof(TModel))
            throw new InvalidOperationException(
                $"Attempted to update an entity in reference with the model {model.GetType()}, but this broker expects " +
                $"the model to be of type {typeof(TModel)}"
            );

        if (TryGetReference(model.Id, out var reference) && reference.TryGetReference(out var entity))
            await entity.UpdateAsync(model, false, token);
    }

    public async ValueTask UpdateInReferenceEntitiesAsync(IEnumerable<TModel> models, CancellationToken token)
        => await Parallel.ForEachAsync(models, token, UpdateInReferenceEntityAsync);

    public async ValueTask<IEntityHandle<TId>> TransferConstructionOfEntity(
        TModel model,
        IGatewayConstructionContext context,
        CancellationToken token)
    {
        // sanity
        if (model.GetType() != typeof(TModel))
        {
            _logger.LogError(
                "Attempted to transfer an entity with the model type {Type} to the broker for {Ours}",
                model.GetType(),
                typeof(TModel)
            );

            throw new InvalidOperationException(
                "Attempted to transfer a non-owned model"
            );
        }

        if (TryGetHandleFromReference(model.Id, out var handle))
            return handle;

        using var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (TryGetHandleFromReference(model.Id, out handle))
                return handle;

            var entity = TEntity.Construct(_client, context, model);

            handle = new EntityHandle<TId, TEntity, TModel>(this, model.Id, entity);

            lock (_syncRoot)
            {
                _references[model.Id] = new(
                    this,
                    model.Id,
                    entity,
                    handle
                );
            }

            return handle;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private bool RemoveReference(EntityReference reference)
    {
        lock (_syncRoot)
        {
            return _references.Remove(reference.Id);
        }
    }

    private void InterceptSelfUser(ref TModel model)
    {
        if (model is not IUserModel userModel)
            return;

        if (userModel.Id != _client.CurrentUser.Id)
            return;

        if (userModel is ISelfUserModel selfUserModel)
        {
            if (_controller.SelfUserModel is null)
                _controller.SelfUserModel = new(selfUserModel);
            else
            {
                _controller.SelfUserModel.SelfUserModelPart = selfUserModel;
                _controller.SelfUserModel.UserModelPart = selfUserModel;
            }
        }
        else
        {
            if (_controller.SelfUserModel is null)
                return;
        }

        _controller.SelfUserModel.UserModelPart = userModel;

        model = (_controller.SelfUserModel as TModel)!;
    }

    public bool TryCreateLatentHandle(
        TModel model,
        [MaybeNullWhen(true)] out TEntity entity,
        [MaybeNullWhen(false)] out IDisposable handle,
        CancellationToken token)
    {
        var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

        semaphoreSlim.Wait(token);

        if (TryGetHandleFromReference(model.Id, out var entityHandle))
        {
            semaphoreSlim.Release();
            scope.Dispose();
            handle = null;
            entity = entityHandle.Entity;
            entityHandle.Dispose();
            return false;
        }

        handle = new LatentEntityPromise(scope, semaphoreSlim);
        entity = null;
        return true;
    }

    private async ValueTask<IEntityHandle<TId, TEntity>> CreateReferenceAndHandleAsync(
        CachePathable path,
        TId id,
        TModel model,
        TActor? actor = null,
        CancellationToken token = default)
    {
        IGatewayConstructionContext context = actor is not null
            ? new GatewayConstructionContext<TActor>(actor, path)
            : new GatewayConstructionContext(path);

        if (model.GetType() != typeof(TModel))
        {
            var broker = await TEntity.GetBrokerForModelAsync(_client, model.GetType(), token);

            if (broker != this)
            {
                if (
                    await broker.TransferConstructionOfEntity(model, context, token)
                    is IEntityHandle<TId, TEntity> transferredHandle)
                {
                    return transferredHandle;
                }

                // TODO: error state
            }
        }

        var entity = TEntity.Construct(_client, context, model);

        var handle = new EntityHandle<TId, TEntity, TModel>(this, id, entity);

        lock (_syncRoot)
            _references[id] = new EntityReference(this, id, entity, handle);

        return handle;
    }

    public void ReleaseHandle(IEntityHandle<TId, TEntity> handle)
    {
        EntityReference? reference;
        bool hasReference;

        lock (_syncRoot)
            hasReference = _references.TryGetValue(handle.Id, out reference);

        if (!hasReference)
        {
            // TODO: this is a warning state
            return;
        }

        if (!reference!.RemoveHandle(handle))
        {
            // TODO: this is a warning state
        }
    }

    public async ValueTask AttachLatentEntityAsync(TId id, TEntity entity, IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token)
    {
        using var scope = _keyedSemaphore.Get(id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (
                TryGetReference(id, out var reference) &&
                reference.TryGetReference(out var existing) &&
                existing != entity)
            {
                // TODO: this is an error state, mainly caused by a race condition
                // in the time that it took from constructing 'entity' and then registering it,
                // the state controller constructed another form of the entity. We can either
                // lock out this semaphore at construction time or we can allow multiple
                // references.
                return;
            }

            var model = entity.GetModel();

            InterceptSelfUser(ref model);

            lock (_syncRoot)
                _references[id] = new(this, id, entity);

            var store = await storeInfo.GetStoreForModelType(model.GetType(), token);

            await store.AddOrUpdateAsync(model, token);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async ValueTask UpdateAsync(TModel model, IStoreInfo<TId, TModel> storeInfo, CancellationToken token)
    {
        InterceptSelfUser(ref model);

        var store = await storeInfo.GetStoreForModelType(model.GetType(), token);

        await store.AddOrUpdateAsync(model, token);

        var broker = model.GetType() != typeof(TModel)
            ? await TEntity.GetBrokerForModelAsync(_client, model.GetType(), token)
            : this;

        await broker.UpdateInReferenceEntityAsync(model, token);
    }

    public async ValueTask BatchUpdateAsync(
        IEnumerable<TModel> models,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token)
    {
        foreach (var grouping in models.GroupBy(x => x.GetType()))
        {
            if (!storeInfo.HierarchyStoreMap.TryGetValue(grouping.Key, out var storeProviderInfo))
                throw new InvalidOperationException(
                    $"No store info exists for the model type {grouping.Key} under the {typeof(TModel)} broker"
                );

            var store = await storeInfo.GetOrComputeStoreAsync(storeProviderInfo, token);

            await store.AddOrUpdateBatchAsync(grouping, token);

            if (grouping.Key != typeof(TModel))
            {
                var broker = await TEntity.GetBrokerForModelAsync(_client, grouping.Key, token);
                await broker.UpdateInReferenceEntitiesAsync(grouping, token);
                continue;
            }

            await UpdateInReferenceEntitiesAsync(grouping, token);
        }
    }

    public async ValueTask<IEnumerable<IEntityHandle<TId, TEntity>>> BatchCreateOrUpdateAsync(
        CachePathable path,
        IEnumerable<TModel> models,
        IStoreInfo<TId, TModel>? storeInfo = null,
        CancellationToken token = default)
    {
        var entityModels = models as TModel[] ?? models.ToArray();

        if (storeInfo is not null)
        {
            if (!storeInfo.HasHierarchicStores)
            {
                await storeInfo.Store.AddOrUpdateBatchAsync(entityModels, token);
            }
            else
            {
                foreach (var grouping in entityModels.GroupBy(x => x.GetType()))
                {
                    if (!storeInfo.HierarchyStoreMap.TryGetValue(grouping.Key, out var storeProviderInfo))
                        throw new InvalidOperationException(
                            $"No store info exists for the model type {grouping.Key} under the {typeof(TModel)} broker"
                        );

                    var store = await storeInfo.GetOrComputeStoreAsync(storeProviderInfo, token);

                    await store.AddOrUpdateBatchAsync(grouping, token);
                }
            }
        }

        var results = new List<IEntityHandle<TId, TEntity>>();

        foreach (var model in entityModels)
        {
            if (TryGetHandleFromReference(model.Id, out var handle))
            {
                await handle.Entity.UpdateAsync(model, false, token);
                results.Add(handle);
                continue;
            }

            using var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

            await semaphoreSlim.WaitAsync(token);

            try
            {
                if (TryGetHandleFromReference(model.Id, out handle))
                {
                    await handle.Entity.UpdateAsync(model, false, token);
                    results.Add(handle);
                    continue;
                }

                results.Add(
                    await CreateReferenceAndHandleAsync(
                        path,
                        model.Id,
                        model,
                        token: token
                    )
                );
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        return results.ToImmutableList();
    }

    public async ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        TActor? actor = null,
        CancellationToken token = default)
    {
        using var scope = _keyedSemaphore.Get(identity.Id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (TryGetHandleFromReference(identity.Id, out var handle))
                return handle;

            TModel? model = null;

            var stores = await storeInfo.GetAllStoresAsync(token);

            switch (stores.Length)
            {
                case 0:
                    return null;
                case 1:
                    model = await stores[0].GetAsync(identity.Id, token);
                    break;
                default:
                {
                    using var searchTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

                    await Parallel.ForEachAsync(stores, searchTokenSource.Token, async (store, token) =>
                    {
                        model = await store.GetAsync(identity.Id, token);

                        if (model is not null)
                            searchTokenSource.Cancel();
                    });
                    break;
                }
            }

            if (model is null)
                return null;

            return await CreateReferenceAndHandleAsync(path, identity.Id, model, actor, token);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async IAsyncEnumerable<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        await _enumerationSemaphore.WaitAsync(token);

        try
        {
            var stores = await storeInfo.GetAllStoresAsync(token);

            if (stores.Length == 0)
                yield break;

            await foreach
                (var batch in AsyncEnumeratorUtils.JoinAsync(
                    stores,
                    static (store, token) => store.GetAllAsync(token),
                    token)
                )
            {
                // TODO: might be another performant solution
                yield return (await Task.WhenAll(
                    batch.Select(async model =>
                    {
                        if (TryGetHandleFromReference(model.Id, out var handle))
                            return handle;

                        return await CreateReferenceAndHandleAsync(
                            path,
                            model.Id,
                            model,
                            token: token
                        );
                    })
                )).ToImmutableList();
            }
        }
        finally
        {
            _enumerationSemaphore.Release();
        }
    }

    public async IAsyncEnumerable<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> QueryAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        TId from,
        Optional<TId> to,
        Direction direction,
        int? limit = null,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        await _enumerationSemaphore.WaitAsync(token);

        try
        {
            var stores = await storeInfo.GetAllStoresAsync(token);

            if (stores.Length == 0)
                yield break;

            await foreach
                (var batch in AsyncEnumeratorUtils.JoinAsync(
                    stores,
                    static (from, to, direction, limit, store, token) => store.QueryAsync(
                        from,
                        to,
                        direction,
                        limit,
                        token
                    ),
                    from,
                    to,
                    direction,
                    limit,
                    token)
                )
            {
                // TODO: might be another performant solution
                yield return (await Task.WhenAll(
                    batch.Select(async model =>
                    {
                        if (TryGetHandleFromReference(model.Id, out var handle))
                            return handle;

                        return await CreateReferenceAndHandleAsync(
                            path,
                            model.Id,
                            model,
                            token: token
                        );
                    })
                )).ToImmutableList();
            }
        }
        finally
        {
            _enumerationSemaphore.Release();
        }
    }

    public async IAsyncEnumerable<IReadOnlyCollection<TId>> GetAllIdsAsync(
        IStoreInfo<TId, TModel> storeInfo,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        // we don't have to lock since we're not accessing the reference cache
        var stores = await storeInfo.GetAllStoresAsync(token);

        if (stores.Length == 0)
            yield break;

        await foreach
            (var batch in AsyncEnumeratorUtils.JoinAsync(
                stores,
                static (store, token) => store.GetAllIdsAsync(token),
                token)
            )
        {
            yield return batch.ToImmutableList();
        }
    }

    private bool TryGetReference(TId id, [MaybeNullWhen(false)] out EntityReference reference)
    {
        lock (_syncRoot)
            return _references.TryGetValue(id, out reference);
    }

    private bool TryGetHandleFromReference(TId id, [MaybeNullWhen(false)] out IEntityHandle<TId, TEntity> handle)
    {
        if (TryGetReference(id, out var reference)) return (handle = reference.AllocateHandle()) is not null;

        handle = null;
        return false;
    }
}
