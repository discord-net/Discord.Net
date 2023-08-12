using Discord.WebSocket.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.State
{
    internal sealed class EntityBroker<TId, TEntity, TModel> : EntityBroker<TId, TEntity, TModel, object?>
        where TEntity : class, ICacheableEntity<TId, TModel>
        where TModel : IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        public EntityBroker(StateController controller,
            StoreFactory getStore,
            EntityFactory factory)
            : base(controller, getStore, factory, (_, __, ___) => ValueTask.FromResult<object?>(null))
        {}
    }

    internal class EntityBroker<TId, TEntity, TModel, TFactoryArgs> : IEntityBroker<TId, TEntity>
        where TEntity : class, ICacheableEntity<TId, TModel>
        where TModel : IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        public delegate ValueTask<TFactoryArgs> CreateEntityFactoryArguments(Optional<TId> id, Optional<TId> parent, CancellationToken token);
        public delegate ValueTask<TEntity> EntityFactory(TFactoryArgs? args, Optional<TId> parent, TModel model, CancellationToken token);
        public delegate ValueTask<IEntityStore<TId>> StoreFactory(Optional<TId> parent, CancellationToken token);

        private readonly ConcurrentDictionary<TId, EntityReference> _referenceCache;

        private readonly StoreFactory _getStore;
        private readonly StateController _controller;
        private readonly EntityFactory _factory;
        private readonly CreateEntityFactoryArguments _argsFactory;

        private readonly SemaphoreSlim _cleanupSemaphore;

        public EntityBroker(
            StateController controller,
            StoreFactory getStore,
            EntityFactory factory,
            CreateEntityFactoryArguments? argsFactory = null)
        {
            _cleanupSemaphore = new(1, 1);
            _referenceCache = new();
            _getStore = getStore;
            _controller = controller;
            _factory = factory;
            _argsFactory = argsFactory ?? NullArgumentFactory;
        }

        private static ValueTask<TFactoryArgs> NullArgumentFactory(Optional<TId> _, Optional<TId> __, CancellationToken ___)
            => ValueTask.FromResult<TFactoryArgs>(default!);

        public bool TryGetReferenced(TId id, [NotNullWhen(true)] out TEntity? entity)
        {
            if(_referenceCache.TryGetValue(id, out var entityRef))
            {
                if(entityRef.Reference.IsAlive && entityRef.Reference.Target is not null)
                {
                    entity = (TEntity)entityRef.Reference.Target!;
                    return true;
                }
            }

            entity = null;
            return false;
        }

        private async Task<TEntity?> TryGetInScopeAsync(TId id, CancellationToken token)
        {
            // wait for any cleanup of the reference cache
            await _cleanupSemaphore.WaitAsync(token);

            try
            {
                if(_referenceCache.TryGetValue(id, out var entityReference) && entityReference.Reference.IsAlive && entityReference.Reference.Target is TEntity entity)
                {
                    return entity;
                }

                return null;
            }
            finally
            {
                _cleanupSemaphore.Release();
            }
        }

        #region CreateAsync
        public async ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(TModel model, CancellationToken token = default)
            => await CreateAsync(model, await _argsFactory(model.Id, Optional<TId>.Unspecified, token), Optional<TId>.Unspecified);

        public async ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(TModel model, Optional<TId> parent, CancellationToken token = default)
            => await CreateAsync(model, await _argsFactory(model.Id, parent, token), parent, token);

        public ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(TModel model, TFactoryArgs factoryArgs, CancellationToken token = default)
            => CreateAsync(model, factoryArgs, Optional<TId>.Unspecified, token);

        public async ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(TModel model, TFactoryArgs factoryArgs, Optional<TId> parent, CancellationToken token = default)
        {
            var store = await _getStore(parent, token);

            // if we have the entity in scope already, get it, create a new handle, and update the entity
            var existing = await TryGetInScopeAsync(model.Id, token);

            if(existing is not null)
            {
                var handle = _controller.AllocateHandle(store, existing);
                existing.Update(model);
                return handle;
            }

            await store.AddOrUpdateAsync(model, token);
            return await CreateAndAllocateHandle(store, parent, factoryArgs, model, token);
        }
        #endregion

        #region GetAsync
        public async ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId id, CancellationToken token = default)
            => await GetAsync(id, Optional<TId>.Unspecified, await _argsFactory(id, Optional<TId>.Unspecified, token), token);

        public async ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId id, Optional<TId> parent, CancellationToken token = default)
            => await GetAsync(id, parent, await _argsFactory(id, parent, token), token);

        public ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId id, TFactoryArgs args, CancellationToken token = default)
            => GetAsync(id, Optional<TId>.Unspecified, args, token);

        public async ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId id, Optional<TId> parent, TFactoryArgs args, CancellationToken token = default)
        {
            var store = await _getStore(parent, token);

            var existing = await TryGetInScopeAsync(id, token);

            if(existing is not null)
            {
                return _controller.AllocateHandle(store, existing);
            }

            var model = await store.GetAsync(id, token);

            if(model is not TModel entityModel)
            {
                throw new InvalidCastException("The returned entity model is not applicable for the target entity");
            }

            if (model is null)
                return null;

            return await CreateAndAllocateHandle(store, parent, args, entityModel, token);
        }
        #endregion

        #region GetAllAsync
        public async ValueTask<IAsyncEnumerable<IEntityHandle<TId, TEntity>>> GetAllAsync(CancellationToken token = default)
            => await GetAllAsync(Optional<TId>.Unspecified, await _argsFactory(Optional<TId>.Unspecified, Optional<TId>.Unspecified, token), token);

        public async ValueTask<IAsyncEnumerable<IEntityHandle<TId, TEntity>>> GetAllAsync(Optional<TId> parent, CancellationToken token = default)
            => await GetAllAsync(parent, await _argsFactory(Optional<TId>.Unspecified, parent, token), token);

        public ValueTask<IAsyncEnumerable<IEntityHandle<TId, TEntity>>> GetAllAsync(TFactoryArgs args, CancellationToken token = default)
            => GetAllAsync(Optional<TId>.Unspecified, args, token);

        public async ValueTask<IAsyncEnumerable<IEntityHandle<TId, TEntity>>> GetAllAsync(Optional<TId> parent, TFactoryArgs args, CancellationToken token = default)
        {
            var store = await _getStore(parent, token);

            return store.GetAllAsync(token)
                .Select(VerifyCorrectModel)
                .SelectAwait(entity => CreateAndAllocateHandle(store, parent, args, entity, token));
        }
        #endregion

        #region GetAllIdsAsync
        public ValueTask<IAsyncEnumerable<TId>> GetAllIdsAsync(CancellationToken token = default)
            => GetAllIdsAsync(Optional<TId>.Unspecified, token);

        public async ValueTask<IAsyncEnumerable<TId>> GetAllIdsAsync(Optional<TId> parent, CancellationToken token = default)
        {
            var store = await _getStore(parent, token);

            return store.GetAllIdsAsync(token);
        }
        #endregion

        private TModel VerifyCorrectModel(IEntityModel<TId> raw)
        {
            if (raw is not TModel entityModel)
            {
                throw new InvalidCastException("The returned entity model is not applicable for the target entity");
            }

            return entityModel;
        }

        private async ValueTask<IEntityHandle<TId, TEntity>> CreateAndAllocateHandle(IEntityStore<TId> store, Optional<TId> parent, TFactoryArgs? args, TModel model, CancellationToken token = default)
        {
            var handle = _controller.AllocateHandle(store, await _factory(args, parent, model, token));
            _ = _referenceCache.GetOrAdd(model.Id, CreateTrackedReference, handle);
            return handle;
        }

        private EntityReference CreateTrackedReference(TId id, IEntityHandle<TId, TEntity> handle)
        {
            var entity = handle.Entity ?? throw new NullReferenceException("Entity was null from a fresh handle");

            var reference = new EntityReference(entity);

            reference.RegisterHandle(handle);

            return reference;
        }

        public async Task CleanAsync(CancellationToken token)
        {
            await _cleanupSemaphore.WaitAsync(token);

            try
            {
                foreach (var entity in _referenceCache.ToArray())
                {
                    if (!entity.Value.Reference.IsAlive)
                    {
                        // cleanup any handles
                        if (entity.Value.HandleReferenceCount > 0)
                        {
                            await _controller.FreeHandles(entity.Value.Handles);
                        }

                        _referenceCache.Remove(entity.Key, out _);
                    }
                }
            }
            finally
            {
                _cleanupSemaphore.Release();
            }
        }
    }

    internal interface IEntityBroker<TId, TEntity> : IEntityBroker
        where TEntity : class, ICacheableEntity<TId>
        where TId : IEquatable<TId>
    {
        bool TryGetReferenced(TId id, [NotNullWhen(true)] out TEntity? entity);

        ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId id, Optional<TId> parent = default, CancellationToken token = default);

        ValueTask<IAsyncEnumerable<TId>> GetAllIdsAsync(Optional<TId> parent = default, CancellationToken token = default);

        ValueTask<IAsyncEnumerable<IEntityHandle<TId, TEntity>>> GetAllAsync(Optional<TId> parent = default, CancellationToken token = default);
    }

    internal interface IEntityBroker
    {
        Task CleanAsync(CancellationToken token);
    }
}
