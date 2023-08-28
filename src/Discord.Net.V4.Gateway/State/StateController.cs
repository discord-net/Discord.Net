using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Discord.Gateway.Cache;
using Microsoft.Extensions.Logging;

using CleanupFunction = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>;

namespace Discord.Gateway.State
{
    internal sealed partial class StateController
    {
        private readonly struct CleanupTask
        {
            public readonly string Name;
            public readonly CleanupFunction Task;

            public CleanupTask(CleanupFunction task)
                : this("Unnamed clean step", task)
            {
                 
            }

            public CleanupTask(string name, CleanupFunction task)
            {
                Name = name;
                Task = task;
            }
        }

        private readonly ICacheProvider _cache;
        private readonly ConcurrentQueue<CleanupTask> _cleanQueue;
        private readonly ConcurrentDictionary<Guid, IEntityHandle> _handles;

        private readonly ConcurrentDictionary<ulong, EntityReference> _snowflakeReferenceCache;
        private readonly ConcurrentDictionary<object, EntityReference> _genericReferenceCache;

        private readonly DiscordGatewayClient _client;

        private readonly SemaphoreSlim _cleanupSemaphore;

        public StateController(DiscordGatewayClient client, in ICacheProvider cache)
        {
            _client = client;
            _cache = cache;
            _cleanQueue = new();
            _snowflakeReferenceCache = new();
            _genericReferenceCache = new();
            _cleanupSemaphore = new(1, 1);
            _handles = new();
        }

        internal void AddCleanupTask(string name, CleanupFunction task)
            => _cleanQueue.Enqueue(new CleanupTask(name, task));
        internal void AddCleanupTask(CleanupFunction task)
           => _cleanQueue.Enqueue(new CleanupTask(task));

        #region GetStores
        public ValueTask<IEntityStore<ulong>> GetStoreAsync(StoreType type, CancellationToken token = default)
            => _cache.GetStoreAsync<ulong>(type, token);

        public ValueTask<IEntityStore<TId>> GetStoreAsync<TId>(StoreType type, CancellationToken token = default)
            where TId : IEquatable<TId>
            => _cache.GetStoreAsync<TId>(type, token);

        public ValueTask<IEntityStore<ulong>> GetSubStoreAsync(ulong id, StoreType type, CancellationToken token = default)
            => _cache.GetSubStoreAsync(type, id, token);

        public ValueTask<IEntityStore<TId>> GetSubStoreAsync<TId>(TId id, StoreType type, CancellationToken token = default)
            where TId : IEquatable<TId>
            => _cache.GetSubStoreAsync(type, id, token);

        public ValueTask<IEntityStore<TId>> GetGenericStoreAsync<TId>(Optional<TId> parent, StoreType type, CancellationToken token = default)
            where TId : IEquatable<TId>
            => parent.IsSpecified
                ? GetSubStoreAsync(parent.Value, type, token)
                : GetStoreAsync<TId>(type, token);
        #endregion

        public bool TryGetReference<TId>(TId id, [NotNullWhen(true)] out EntityReference? reference)
            where TId : notnull
        {
            if (id is ulong snowflake)
                return _snowflakeReferenceCache.TryGetValue(snowflake, out reference);

            return _genericReferenceCache.TryGetValue(id, out reference);
        }

        public void AddOrUpdateReference<TId>(TId id, Func<EntityReference> factory, Action<EntityReference> update)
            where TId : notnull
        {
            if (id is ulong snowflake)
                _snowflakeReferenceCache.AddOrUpdate(snowflake, _ => factory(), (_, old) => { update(old); return old; });
            else
                _genericReferenceCache.AddOrUpdate(id, _ => factory(), (_, old) => { update(old); return old; });
        }

        public void AddReference<TId>(TId id, EntityReference reference)
            where TId : notnull
        {
            var result = id is ulong snowflake
                ? _snowflakeReferenceCache.TryAdd(snowflake, reference)
                : _genericReferenceCache.TryAdd(id, reference);

            if (!result)
                throw new ArgumentException("Entity reference with the provided ID already exists", nameof(id));
        }

        public void UpdateActiveEntities<TId, TModel>(CacheOperation operation, TModel model)
            where TModel : IEntityModel<TId>
            where TId : IEquatable<TId>
        {
            if(typeof(TId) == typeof(ulong))
            {
                UpdateActiveEntities<TId, TModel, ulong>(in _snowflakeReferenceCache, operation, model);
                return;
            }

            UpdateActiveEntities<TId, TModel, object>(in _genericReferenceCache, operation, model);
        }

        private void UpdateActiveEntities<TId, TModel, TKey>(
            in ConcurrentDictionary<TKey, EntityReference> dict, CacheOperation operation, TModel model)
            where TKey : notnull
            where TModel : IEntityModel<TId>
            where TId : IEquatable<TId>
        {
            using var _ = _client.Logger.BeginScope(model.Id);

            _client.Logger.LogTrace("Updating in-scope entities...");

            foreach ((var Id, var Entity) in dict)
            {
                if (Entity.Reference.IsAlive && Entity.Reference.Target is ICacheUpdatable<TId, TModel> cacheUpdatable)
                {
                    _client.Logger.LogTrace("HIT: {EntityType} ({EntityId})", Entity.Reference.Target.GetType().Name, cacheUpdatable.Id);
                    cacheUpdatable.Update(model, operation);
                }
            }
        }

        public IEntityHandle<TId, TEntity> AllocateHandle<TId, TEntity>(
            IEntityStore<TId> store, TEntity entity,
            EntityHandleFlags flags = EntityHandleFlags.None)
            where TEntity : class, ICacheableEntity<TId>
            where TId : IEquatable<TId>
        {
            // TODO: store and track
            var handle = new EntityHandle<TId, TEntity>(this, entity, store, flags);

            if(!_handles.TryAdd(handle.HandleId, handle))
            {
                // TODO: should be fatal
                _client.Logger.LogWarning("Failed to store handle of entity {Id}", entity.Id);
            }

            _client.Logger.LogDebug("Handle {} created for entity {}", handle.HandleId, handle.EntityId);

            return handle;
        }

        public async Task RunCleanupAsync(CancellationToken token = default)
        {
            await _cleanupSemaphore.WaitAsync(token);

            try
            {
                // clean dead references
                await CleanReferenceCacheAsync(_snowflakeReferenceCache, token);
                await CleanReferenceCacheAsync(_genericReferenceCache, token);

                // run the queued dispose tasks
                while(_cleanQueue.TryDequeue(out var clean))
                {
                    // TODO: trace log the clean
                    await clean.Task(token);
                } 
            }
            finally
            {
                _cleanupSemaphore.Release();
            }
        }

        private async Task CleanReferenceCacheAsync<TKey>(ConcurrentDictionary<TKey, EntityReference> dict, CancellationToken token = default)
            where TKey : notnull
        {
            foreach ((var Id, var Entity) in _snowflakeReferenceCache)
            {
                if (!Entity.Reference.IsAlive)
                {
                    await FreeHandlesAsync(Entity.Handles, token);
                    _snowflakeReferenceCache.TryRemove(Id, out _);
                }
            }
        }

        internal async Task FreeHandlesAsync(IEnumerable<Guid> handles, CancellationToken token = default)
        {
            foreach(var handleId in handles)
            {
                if(_handles.TryGetValue(handleId, out var handle))
                {
                    await handle.DisposeAsync(token);
                } 
            }
        }
    }
}
