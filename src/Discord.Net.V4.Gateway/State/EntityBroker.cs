using Discord.Gateway;
using Discord.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

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
    private sealed class KeyedSemaphoreHandle(IDisposable scope, SemaphoreSlim semaphore) : IDisposable
    {
        public void Dispose()
        {
            scope.Dispose();
            semaphore.Release();
        }
    }

    private sealed class EntityReference : IEntityReference<TId, TEntity>,
        IEquatable<EntityReference>,
        IEquatable<IEntityReference<TId>>
    {
        private sealed class EntityReferenceCompanion(EntityReference reference)
        {
            ~EntityReferenceCompanion()
            {
                reference.DestroyReference();
            }
        }

        public TId Id { get; }

        private bool IsAlive => !_isKilled && _handle.IsAllocated;

        private bool _isKilled;

        private DependentHandle _handle;
        private readonly EntityBroker<TId, TEntity, TActor, TModel> _broker;
        private readonly HashSet<IEntityHandle<TId, TEntity>> _handles;
        private readonly int _entityReferenceHashCode;

        public EntityReference(
            EntityBroker<TId, TEntity, TActor, TModel> broker,
            TId id,
            TEntity entity)
        {
            _broker = broker;
            Id = id;
            _handles = new();
            _handle = new DependentHandle(entity, new EntityReferenceCompanion(this));

            _entityReferenceHashCode = RuntimeHelpers.GetHashCode(entity);
        }

        [MustDisposeResource]
        public ValueTask<IDisposable> GetMutationLockHandleAsync(CancellationToken token = default)
            => _broker.GetEntityLockHandleAsync(Id, token);

        public bool TryGetEntity([MaybeNullWhen(false)] out TEntity entity)
        {
            entity = GetReference();
            return entity is not null;
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public IEntityHandle<TId, TEntity>? AllocateHandle(TEntity? entity = null)
        {
            if (!IsAlive) return null;

            if (entity is not null && !IsOwnedByUs(entity))
                throw new InvalidOperationException("The provided entity isn't represented by this entity reference");

            entity ??= GetReference();

            if (entity is null)
            {
                DestroyReference();
                return null;
            }

            var handle = new EntityHandle<TId, TEntity>(Id, entity, this);
            _handles.Add(handle);
            return handle;
        }

        public bool RemoveHandle(IEntityHandle<TId, TEntity> handle)
            => _handles.Remove(handle);

        public bool IsOwnedByUs(TEntity entity)
            => RuntimeHelpers.GetHashCode(entity) == _entityReferenceHashCode;

        private void DestroyReference()
        {
            if (!IsAlive) return;

            _isKilled = true;

            if (_broker.RemoveReference(this)) _handle.Dispose();
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

        public bool Equals(IEntityReference<TId>? other)
            => other is not null && other.Id.Equals(Id);

        public bool Equals(EntityReference? other)
            => other is not null && other.Id.Equals(Id);

        public override bool Equals(object? obj)
            => obj is IEntityReference<TId> other && Equals(other);

        public override int GetHashCode()
            => _entityReferenceHashCode;

        TEntity? IEntityReference<TId, TEntity>.GetReference(out bool isSuccess)
        {
            isSuccess = TryGetEntity(out var entity);
            return entity;
        }

        IEntityHandle<TId, TEntity>? IEntityReference<TId, TEntity>.AllocateHandle()
            => AllocateHandle();

        void IEntityReference<TId, TEntity>.ReleaseHandle(IEntityHandle<TId> handle)
        {
            if (handle is not IEntityHandle<TId, TEntity> ourHandle || !Equals(ourHandle.OwningReference))
                throw new InvalidOperationException("Cannot release the provided handle, its not owned by us");

            RemoveHandle(ourHandle);
        }
    }

    public bool HasChildBrokers => _brokerHierarchy.Count > 0;

    private readonly IReadOnlyDictionary<Type, IManageableEntityBroker<TId, TEntity, TModel>> _brokerHierarchy;

    private readonly DiscordGatewayClient _client;
    private readonly StateController _controller;
    private readonly Dictionary<TId, EntityReference> _references;
    private readonly KeyedSemaphoreSlim<TId> _keyedSemaphore;
    private readonly SemaphoreSlim _storeInfoSemaphore = new(1, 1);
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

        _brokerHierarchy = TEntity.GetBrokerHierarchy(_client);
    }

    [MustDisposeResource]
    public async ValueTask<IDisposable> GetEntityLockHandleAsync(TId id, CancellationToken token = default)
    {
        var scope = _keyedSemaphore.Get(id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        return new KeyedSemaphoreHandle(scope, semaphoreSlim);
    }

    public async ValueTask UpdateInReferenceEntityAsync(TModel model, CancellationToken token)
    {
        // sanity
        if (model.GetType() != typeof(TModel))
            throw new InvalidOperationException(
                $"Attempted to update an entity in reference with the model {model.GetType()}, but this broker expects " +
                $"the model to be of type {typeof(TModel)}"
            );

        if (TryGetDirectReference(model.Id, out var entity))
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

        if (TryGetHandle(model.Id, out var handle))
            return handle;

        using var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (TryGetHandle(model.Id, out handle))
                return handle;

            return CreateReferenceAndHandle(
                model.Id,
                TEntity.Construct(_client, context, model)
            );
        }
        finally
        {
            semaphoreSlim.Release();
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
        [MaybeNullWhen(false), MustDisposeResource]
        out IDisposable handle,
        CancellationToken token)
    {
        var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

        semaphoreSlim.Wait(token);

        if (TryGetDirectReference(model.Id, out entity))
        {
            // release the keyed semaphore and dispose the scope for it
            semaphoreSlim.Release();
            scope.Dispose();

            handle = null;
            return false;
        }

        handle = new KeyedSemaphoreHandle(scope, semaphoreSlim);
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

        if (model.GetType() == typeof(TModel))
        {
            return CreateReferenceAndHandle(
                id,
                TEntity.Construct(_client, context, model)
            );
        }

        var broker = GetBrokerForModelType(model.GetType());

        if (broker == this)
        {
            return CreateReferenceAndHandle(
                id,
                TEntity.Construct(_client, context, model)
            );
        }

        var handle = await broker.TransferConstructionOfEntity(model, context, token);

        if (handle is IEntityHandle<TId, TEntity> transferredHandle) return transferredHandle;

        _logger.LogError(
            "Attempted to transfer construction of {Entity} to {Broker}, but the handle returned was not " +
            "the correct type ({Handle})",
            typeof(TEntity),
            broker,
            handle
        );

        throw new InvalidOperationException("Attempted to transfer a non-owned entity");
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
                !reference.IsOwnedByUs(entity))
            {
                // in the time that it took from constructing 'entity' and then registering it,
                // the state controller constructed another form of the entity. We can either
                // lock out this semaphore at construction time or we can allow multiple
                // references.

                _logger.LogError(
                    "Failed to attach latent entity: The latent {Entity} entity that was constructed already has " +
                    "another entity in memory representing it with the id {Id}",
                    typeof(TEntity),
                    id
                );

                throw new InvalidOperationException(
                    "Cannot attach latent entity: another version of it already exists"
                );
            }

            var model = entity.GetModel();

            InterceptSelfUser(ref model);

            lock (_syncRoot)
                _references[id] = new(this, id, entity);

            await storeInfo
                .GetStoreForModelType(model.GetType())
                .AddOrUpdateAsync(model, token);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async ValueTask UpdateAsync(TModel model, IStoreInfo<TId, TModel> storeInfo, CancellationToken token)
    {
        InterceptSelfUser(ref model);

        await storeInfo
            .GetStoreForModelType(model.GetType())
            .AddOrUpdateAsync(model, token);

        if (model.GetType() == typeof(TModel))
        {
            await UpdateInReferenceEntityAsync(model, token);
            return;
        }

        await GetBrokerForModelType(model.GetType()).UpdateInReferenceEntityAsync(model, token);
    }

    public async ValueTask UpdateAsync(IPartial<TModel> partial, IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token)
    {
        if (!partial.TryGet<TId>(nameof(IEntityModel<TId>.Id), out _))
        {
            _logger.LogWarning(
                "Attempted to update a '{Entity}' with a partial '{Model}' model, but the model doesn't contain the " +
                "id of type '{Id}'",
                typeof(TEntity),
                partial.UnderlyingModelType,
                typeof(TId)
            );

            return;
        }

        await UpdatePartialModelInternalAsync(
            partial,
            storeInfo.GetStoreForModelType(partial.UnderlyingModelType),
            GetBrokerForModelType(partial.UnderlyingModelType),
            token
        );
    }

    public async ValueTask BatchUpdateAsync(
        IEnumerable<TModel> models,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token)
    {
        foreach (var grouping in models.GroupBy(x => x.GetType()))
        {
            await storeInfo
                .GetStoreForModelType(grouping.Key)
                .AddOrUpdateBatchAsync(grouping, token);

            if (grouping.Key != typeof(TModel))
            {
                await GetBrokerForModelType(grouping.Key).UpdateInReferenceEntitiesAsync(grouping, token);
                continue;
            }

            await UpdateInReferenceEntitiesAsync(grouping, token);
        }
    }

    public async ValueTask BatchUpdateAsync(
        IEnumerable<IPartial<TModel>> models,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token)
    {
        foreach (var grouping in models.GroupBy(x => x.UnderlyingModelType))
        {
            var store = storeInfo.GetStoreForModelType(grouping.Key);
            var broker = GetBrokerForModelType(grouping.Key);

            await Parallel.ForEachAsync(
                grouping,
                token,
                (partial, token) => UpdatePartialModelInternalAsync(partial, store, broker, token)
            );
        }
    }

    private static async ValueTask UpdatePartialModelInternalAsync(
        IPartial<TModel> partial,
        IEntityModelStore<TId, TModel> store,
        IManageableEntityBroker<TId, TEntity, TModel> broker,
        CancellationToken token = default)
    {
        TModel? model;
        IEntityHandle<TId, TEntity>? handle = null;

        if (!partial.TryGet<TId>(nameof(IEntityModel<TId>.Id), out var id))
            return;

        using var scope = await broker.GetEntityLockHandleAsync(id, token);

        if (broker.TryGetHandle(id, out var brokersHandle) && brokersHandle is IEntityHandle<TId, TEntity> ourHandle)
        {
            model = ourHandle.Entity.GetModel();
            handle = ourHandle;
        }
        else
        {
            model = await store.GetAsync(id, token);
            if (model is null) return;
        }

        if (!partial.ApplyTo(model)) return;

        await store.AddOrUpdateAsync(model, token);

        if (handle is not null)
            await handle.Entity.UpdateAsync(model, false, token);
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
                await storeInfo
                    .GetStoreForModelType(grouping.Key)
                    .AddOrUpdateBatchAsync(grouping, token);
            }
        }

        var results = new List<IEntityHandle<TId, TEntity>>();

        foreach (var model in entityModels)
        {
            if (TryGetHandle(model.Id, out var handle))
            {
                await handle.Entity.UpdateAsync(model, false, token);
                results.Add(handle);
                continue;
            }

            using var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

            await semaphoreSlim.WaitAsync(token);

            try
            {
                if (TryGetHandle(model.Id, out handle))
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

    public async ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(
        TModel model,
        CachePathable path,
        TActor? actor = null,
        CancellationToken token = default)
    {
        if (TryGetHandle(model.Id, out var handle))
            return handle;

        using var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            return await CreateReferenceAndHandleAsync(path, model.Id, model, actor, token);
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
            if (TryGetHandle(identity.Id, out var handle))
                return handle;

            TModel? model = null;

            if (!storeInfo.HasHierarchicStores)
                model = await storeInfo.Store.GetAsync(identity.Id, token);
            else
            {
                using var searchTokenSource = new CancellationTokenSource();
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(searchTokenSource.Token);

                try
                {
                    await Parallel.ForEachAsync(storeInfo.AllStores, linkedTokenSource.Token, async (store, token) =>
                    {
                        model = await store.GetAsync(identity.Id, token);

                        if (model is not null)
                            searchTokenSource.Cancel();
                    });
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == searchTokenSource.Token)
                {
                    // do nothing, search has ended
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

    public IAsyncEnumerable<IEntityHandle<TId, TEntity>> GetAllAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default)
    {
        return AsyncEnumeratorUtils
            .JoinAsync(
                storeInfo.AllStores,
                static (store, token) => store.GetAllAsync(token),
                token
            )
            .SelectAwait(async model =>
                await UpdateOrCreateAsync(model, path, token)
            );
    }

    public IAsyncEnumerable<IEntityHandle<TId, TEntity>> QueryAsync(
        CachePathable path,
        IStoreInfo<TId, TModel> storeInfo,
        TId from,
        Optional<TId> to,
        Direction direction,
        int? limit = null,
        CancellationToken token = default)
    {
        return AsyncEnumeratorUtils
            .JoinAsync(
                storeInfo.AllStores,
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
                token
            )
            .SelectAwait(async model =>
                await UpdateOrCreateAsync(model, path, token)
            );
    }

    private async ValueTask<IEntityHandle<TId, TEntity>> UpdateOrCreateAsync(
        TModel model,
        CachePathable path,
        CancellationToken token)
    {
        if (TryGetHandle(model.Id, out var handle))
        {
            await handle.Entity.UpdateAsync(model, false, token);
            return handle;
        }

        using var scope = _keyedSemaphore.Get(model.Id, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (!TryGetHandle(model.Id, out handle))
                handle = await CreateReferenceAndHandleAsync(
                    path,
                    model.Id,
                    model,
                    token: token
                );
            else
                await handle.Entity.UpdateAsync(model, false, token);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return handle;
    }

    public IAsyncEnumerable<TId> GetAllIdsAsync(
        IStoreInfo<TId, TModel> storeInfo,
        CancellationToken token = default)
    {
        return AsyncEnumeratorUtils
            .JoinAsync(
                storeInfo.AllStores,
                static (store, token) => store.GetAllIdsAsync(token),
                token
            );
    }

    private IManageableEntityBroker<TId, TEntity, TModel> GetBrokerForModelType(Type type)
    {
        if (type == typeof(TModel))
            return this;

        if (_brokerHierarchy.TryGetValue(type, out var broker))
            return broker;

        // TODO: should we error?
        _logger.LogWarning(
            "No broker exists for the model {Model} in the hierarchy of {Entity}",
            type,
            typeof(TEntity)
        );
        return this;
    }

    private bool RemoveReference(EntityReference reference)
    {
        lock (_syncRoot)
        {
            return _references.Remove(reference.Id);
        }
    }

    private IEntityHandle<TId, TEntity> CreateReferenceAndHandle(TId id, TEntity entity)
    {
        EntityReference reference;

        lock (_syncRoot) reference = _references[id] = new(this, id, entity);

        return reference.AllocateHandle(entity);
    }

    private bool TryGetDirectReference(TId id, [MaybeNullWhen(false)] out TEntity entity)
    {
        entity = default!;

        lock (_syncRoot)
            return _references.TryGetValue(id, out var reference) && reference.TryGetEntity(out entity);
    }

    private bool TryGetReference(TId id, [MaybeNullWhen(false)] out EntityReference reference)
    {
        lock (_syncRoot)
            return _references.TryGetValue(id, out reference);
    }

    private bool TryGetHandle(TId id, [MaybeNullWhen(false)] out IEntityHandle<TId, TEntity> handle)
    {
        if (TryGetReference(id, out var reference)) return (handle = reference.AllocateHandle()) is not null;

        handle = null;
        return false;
    }

    bool IManageableEntityBroker<TId, TEntity, TModel>.TryGetHandle(TId id,
        [MaybeNullWhen(false)] out IEntityHandle<TId> handle)
    {
        if (TryGetHandle(id, out var ourHandle))
        {
            handle = ourHandle;
            return true;
        }

        handle = null;
        return false;
    }
}