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
            Func<TId?, ValueTask<IEntityStore<TId>>> getStore,
            EntityFactory factory)
            : base(controller, getStore, factory)
        {

        }
    }

    internal class EntityBroker<TId, TEntity, TModel, TFactoryArgs> : IEntityBroker<TId, TEntity>
        where TEntity : class, ICacheableEntity<TId, TModel>
        where TModel : IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        public delegate ValueTask<TEntity> EntityFactory(TFactoryArgs? args, TId? parent, TModel model);

        private readonly ConcurrentDictionary<TId, EntityReference> _referenceCache;

        private readonly Func<TId?, ValueTask<IEntityStore<TId>>> _getStore;
        private readonly StateController _controller;
        private readonly EntityFactory _factory;

        private readonly SemaphoreSlim _cleanupSemaphore;

        public EntityBroker(
            StateController controller,
            Func<TId?, ValueTask<IEntityStore<TId>>> getStore,
            EntityFactory factory)
        {
            _cleanupSemaphore = new(1, 1);
            _referenceCache = new();
            _getStore = getStore;
            _controller = controller;
            _factory = factory;
        }

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

        private async Task<TEntity?> TryGetInScopeAsync(TId id)
        {
            // wait for any cleanup of the reference cache
            await _cleanupSemaphore.WaitAsync();

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

        public ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(TFactoryArgs factoryArgs, TModel model)
            => CreateAsync(default, factoryArgs, model);

        public async ValueTask<IEntityHandle<TId, TEntity>> CreateAsync(TId? parent, TFactoryArgs factoryArgs, TModel model)
        {
            var store = await _getStore(parent);

            // if we have the entity in scope already, get it, create a new handle, and update the entity
            var existing = await TryGetInScopeAsync(model.Id);

            if(existing is not null)
            {
                var handle = _controller.AllocateHandle(store, existing);
                existing.Update(model);
                return handle;
            }

            await store.AddOrUpdateAsync(model);
            return await CreateAndAllocateHandle(store, parent, factoryArgs, model);
        }

        public ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId? parent, TId id)
            => GetAsync(parent, default, id);

        public ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId id)
            => GetAsync(default, default, id);

        public ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TFactoryArgs args, TId id)
            => GetAsync(default, args, id);

        public async ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId? parent, TFactoryArgs? args, TId id)
        {
            var store = await _getStore(parent);

            var existing = await TryGetInScopeAsync(id);

            if(existing is not null)
            {
                return _controller.AllocateHandle(store, existing);
            }

            var model = await store.GetAsync(id);

            if(model is not TModel entityModel)
            {
                throw new InvalidCastException("The returned entity model is not applicable for the target entity");
            }

            if (model is null)
                return null;

            return await CreateAndAllocateHandle(store, parent, args, entityModel);
        }

        public ValueTask<IAsyncEnumerable<IEntityHandle<TId, TEntity>>> GetAllAsync(TFactoryArgs args)
            => GetAllAsync(default, args);

        public async ValueTask<IAsyncEnumerable<IEntityHandle<TId, TEntity>>> GetAllAsync(TId? parent, TFactoryArgs args)
        {
            var store = await _getStore(parent);

            return store.GetAllAsync()
                .Select(VerifyCorrectModel)
                .SelectAwait(entity => CreateAndAllocateHandle(store, parent, args, entity));
        }

        private TModel VerifyCorrectModel(IEntityModel<TId> raw)
        {
            if (raw is not TModel entityModel)
            {
                throw new InvalidCastException("The returned entity model is not applicable for the target entity");
            }

            return entityModel;
        }

        private async ValueTask<IEntityHandle<TId, TEntity>> CreateAndAllocateHandle(IEntityStore<TId> store, TId? parent, TFactoryArgs? args, TModel model)
        {
            var handle = _controller.AllocateHandle(store, await _factory(args, parent, model));
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
        ValueTask<IEntityHandle<TId, TEntity>?> GetAsync(TId? parent, TId id);
    }

    internal interface IEntityBroker
    {
        Task CleanAsync(CancellationToken token);
    }
}
