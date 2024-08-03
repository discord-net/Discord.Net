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
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
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

        if (TryGetFromReference(model.Id, out var entity))
            await entity.UpdateAsync(model, false, token);
    }

    public async ValueTask UpdateInReferenceEntitiesAsync(IEnumerable<TModel> models, CancellationToken token)
    {
        foreach (var model in models)
        {
            // sanity
            if (model.GetType() != typeof(TModel))
                throw new InvalidOperationException(
                    $"Attempted to update an entity in reference with the model {model.GetType()}, but this broker expects " +
                    $"the model to be of type {typeof(TModel)}"

                );
            if (TryGetFromReference(model.Id, out var entity))
                await entity.UpdateAsync(model, false, token);
        }
    }

    public async ValueTask<IEntityHandle<TId>> TransferConstructionOfEntity(
        TModel model,
        ICacheConstructionContext context,
        CancellationToken token)
    {
        IEntityHandle<TId, TEntity>? handle;
        EntityReference? reference;

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

        bool hasReference;

        lock (_syncRoot)
            hasReference = _references.TryGetValue(model.Id, out reference);

        if (hasReference)
        {
            handle = reference!.AllocateHandle();

            if (handle is not null)
                return handle;
        }

        using var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            lock (_syncRoot)
                hasReference = _references.TryGetValue(model.Id, out reference);

            if (hasReference)
            {
                handle = reference!.AllocateHandle();

                if (handle is not null)
                    return handle;
            }

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

        if (TryGetFromReference(model.Id, out entity))
        {
            semaphoreSlim.Release();
            scope.Dispose();
            handle = null;
            return false;
        }

        handle = new LatentEntityPromise(scope, semaphoreSlim);
        entity = null;
        return true;
    }

    private async ValueTask<IEntityHandle<TId>?> ValidateBrokerForModel(
        TModel model,
        ICacheConstructionContext context,
        CancellationToken token)
    {
        if (model.GetType() == typeof(TModel))
            return null;

        var broker = await TEntity.GetBrokerForModelAsync(_client, model.GetType(), token);

        if (broker != this)
            return await broker.TransferConstructionOfEntity(model, context, token);

        return null;
    }

    private async ValueTask<IEntityHandle<TId, TEntity>> CreateReferenceAndHandleAsync(
        CachePathable path,
        TId id,
        TModel model,
        TActor? actor = null,
        CancellationToken token = default)
    {
        ICacheConstructionContext context = actor is not null
            ? new CacheConstructionContext<TActor>(actor, path)
            : new CacheConstructionContext(path);

        if (await ValidateBrokerForModel(model, context, token) is IEntityHandle<TId, TEntity> handle)
            return handle;

        var entity = TEntity.Construct(_client, context, model);

        handle = new EntityHandle<TId, TEntity, TModel>(this, id, entity);

        lock (_syncRoot)
            _references[id] = new EntityReference(this, id, entity, handle);

        return handle;
    }

    private async ValueTask<TEntity> CreateReferenceAndImplicitHandleAsync(
        CachePathable path,
        TId id,
        TModel model,
        TActor? actor = null,
        CancellationToken token = default)
    {
        ICacheConstructionContext context = actor is not null
            ? new CacheConstructionContext<TActor>(actor, path)
            : new CacheConstructionContext(path);

        using var handle = await ValidateBrokerForModel(model, context, token) as IEntityHandle<TId, TEntity>;

        if (handle is not null)
            return handle.Entity;

        var entity = TEntity.Construct(_client, context, model);

        lock (_syncRoot)
            _references[id] = new(this, id, entity);

        return entity;
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
            if (TryGetFromReference(id, out var existing) && existing != entity)
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

    public async ValueTask<TEntity?> GetImplicitAsync(
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
            if (TryGetFromReference(identity.Id, out var entity))
                return entity;

            TModel? model = null;

            await foreach (var store in storeInfo.EnumerateAllAsync(token))
            {
                model = await store.GetAsync(identity.Id, token);

                if (model is not null)
                    break;
            }

            if (model is null)
                return null;

            return await CreateReferenceAndImplicitHandleAsync(path, identity.Id, model, actor, token);
        }
        finally
        {
            semaphoreSlim.Release();
        }
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

            await foreach (var store in storeInfo.EnumerateAllAsync(token))
            {
                model = await store.GetAsync(identity.Id, token);

                if (model is not null)
                    break;
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


    public async ValueTask<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllHandlesAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default)
    {
        await _enumerationSemaphore.WaitAsync(token);

        var result = new List<IEntityHandle<TId, TEntity>>();

        try
        {
            await foreach (var store in storeInfo.EnumerateAllAsync(token))
            await foreach (var model in store.GetAllAsync(token))
            {
                if (TryGetHandleFromReference(model.Id, out var handle))
                {
                    result.Add(handle);
                    continue;
                }

                result.Add(
                    await CreateReferenceAndHandleAsync(
                        path,
                        model.Id,
                        model,
                        token: token
                    )
                );
            }

            return result.AsReadOnly();
        }
        finally
        {
            _enumerationSemaphore.Release();
        }
    }

    public async ValueTask<IReadOnlyCollection<TEntity>> GetAllImplicitAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default)
    {
        await _enumerationSemaphore.WaitAsync(token);

        var result = new List<TEntity>();

        try
        {
            await foreach (var store in storeInfo.EnumerateAllAsync(token))
            await foreach (var model in store.GetAllAsync(token))
            {
                if (TryGetFromReference(model.Id, out var entity))
                {
                    result.Add(entity);
                    continue;
                }

                result.Add(
                    await CreateReferenceAndImplicitHandleAsync(
                        path,
                        model.Id,
                        model,
                        token: token
                    )
                );
            }

            return result.AsReadOnly();
        }
        finally
        {
            _enumerationSemaphore.Release();
        }
    }

    public async ValueTask<IReadOnlyCollection<TId>> GetAllIdsAsync(IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default)
    {
        return
            (
                await EnumerateOnAllStoresAsync(
                    storeInfo,
                    async (store, token) => await store.GetAllIdsAsync(token).ToArrayAsync(token),
                    token
                )
            )
            .SelectMany(x => x)
            .ToImmutableList();
    }

    private static async ValueTask<IEnumerable<T>> EnumerateOnAllStoresAsync<T>(
        IStoreInfo<TId, TModel> storeInfo,
        Func<IEntityModelStore<TId, TModel>, CancellationToken, ValueTask<T>> func,
        CancellationToken token = default)
    {
        if (storeInfo.EnabledStoresForHierarchy.Count == 0)
            return [await func(storeInfo.Store, token)];

        var results = new T[storeInfo.EnabledStoresForHierarchy.Count + 1];

        await Parallel.ForAsync(0, results.Length, token, async (i, token) =>
        {
            if (i == 0)
                results[i] = await func(storeInfo.Store, token);
            else
            {
                results[i] = await func(
                    await storeInfo.GetOrComputeStoreAsync(storeInfo.EnabledStoresForHierarchy[i - 1], token),
                    token
                );
            }
        });

        return results;
    }

    private bool TryGetFromReference(TId id, [MaybeNullWhen(false)] out TEntity entity)
    {
        EntityReference? reference;

        lock (_syncRoot)
            _references.TryGetValue(id, out reference);

        if (reference?.TryGetReference(out entity) ?? false)
            return true;

        entity = null;
        return false;
    }

    private bool TryGetHandleFromReference(TId id, [MaybeNullWhen(false)] out IEntityHandle<TId, TEntity> handle)
    {
        EntityReference? reference;

        lock (_syncRoot)
            _references.TryGetValue(id, out reference);

        handle = reference?.AllocateHandle();
        return handle is not null;
    }
}

internal interface IEntityReference<out TId>
{
    TId Id { get; }
}

internal interface IEntityReference<out TId, out TEntity> : IEntityReference<TId>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TEntity? GetReference(out bool isSuccess);
    IEntityHandle<TId, TEntity>? AllocateHandle();
}

internal interface IEntityBroker<TId, TEntity, in TActor, TModel> : IEntityBroker<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    ValueTask<TEntity?> GetImplicitAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        TActor? actor = null,
        CancellationToken token = default);

    ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        TActor? actor = null,
        CancellationToken token = default
    );

    ValueTask<TEntity?> IEntityBroker<TId, TEntity, TModel>.GetImplicitAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token
    ) => GetImplicitAsync(path, identity, storeInfo, null, token);

    ValueTask<IEntityHandle<TId, TEntity>?> IEntityBroker<TId, TEntity, TModel>.GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token
    ) => GetAsync(path, identity, storeInfo, null, token);
}

internal interface IEntityBroker<TId, TEntity, TModel> : IManageableEntityBroker<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    ValueTask AttachLatentEntityAsync(TId id, TEntity entity, IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token);

    bool TryCreateLatentHandle(
        TModel model,
        [MaybeNullWhen(true)] out TEntity entity,
        [MaybeNullWhen(false)] out IDisposable handle,
        CancellationToken token
    );

    ValueTask UpdateAsync(TModel model, IStoreInfo<TId, TModel> storeInfo, CancellationToken token);

    ValueTask BatchUpdateAsync(IEnumerable<TModel> model, IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token);

    ValueTask<IEnumerable<IEntityHandle<TId, TEntity>>> BatchCreateOrUpdateAsync(
        CachePathable path,
        IEnumerable<TModel> models,
        IStoreInfo<TId, TModel>? store = null,
        CancellationToken token = default
    );

    async ValueTask<IEnumerable<TEntity>> BatchCreateOrUpdateImplicitAsync(
        CachePathable path,
        IEnumerable<TModel> models,
        IStoreInfo<TId, TModel>? store = null,
        CancellationToken token = default
    )
    {
        // TODO: this seems ugly
        return
            (await BatchCreateOrUpdateAsync(path, models, store, token) as IList<IEntityHandle<TId, TEntity>>)!
            .AsParallel()
            .Select((x, i) =>
            {
                var entity = x.Entity;
                x.Dispose();
                return entity;
            })
            .ToImmutableList();
    }

    ValueTask<TEntity?> GetImplicitAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default);

    ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(
        CachePathable path,
        IIdentifiable<TId, TEntity, TModel> identity,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default
    );

    ValueTask<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllHandlesAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default);

    ValueTask<IReadOnlyCollection<TEntity>> GetAllImplicitAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default);

    ValueTask<IReadOnlyCollection<TId>> GetAllIdsAsync(IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default);
}

internal interface IManageableEntityBroker<TId, in TEntity, in TModel> : IEntityBroker<TId>
    where TId : IEquatable<TId>
    where TEntity : class, ICacheableEntity<TId>, IEntityOf<TModel>
    where TModel : IEntityModel<TId>
{
    void ReleaseHandle(IEntityHandle<TId, TEntity> handle);

    ValueTask<IEntityHandle<TId>> TransferConstructionOfEntity(
        TModel model,
        ICacheConstructionContext context,
        CancellationToken token
    );

    ValueTask UpdateInReferenceEntityAsync(TModel models, CancellationToken token);

    ValueTask UpdateInReferenceEntitiesAsync(IEnumerable<TModel> models, CancellationToken token);

    Type IEntityBroker.EntityType => typeof(TEntity);
}

internal interface IEntityBroker<in TId> : IEntityBroker;

internal interface IEntityBroker
{
    Type EntityType { get; }
}
