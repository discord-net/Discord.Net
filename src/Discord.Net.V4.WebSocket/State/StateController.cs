using System.Collections.Concurrent;
using Discord.WebSocket.Cache;

using CleanupFunction = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>;

namespace Discord.WebSocket.State
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
        private readonly DiscordSocketClient _client;

        private readonly SemaphoreSlim _cleanupSemaphore;

        public StateController(DiscordSocketClient client, ICacheProvider cache)
        {
            _client = client;
            _cache = cache;
            _cleanQueue = new();
            _cleanupSemaphore = new(1, 1);
            _handles = new();
        }

        internal void AddCleanupTask(string name, CleanupFunction task)
            => _cleanQueue.Enqueue(new CleanupTask(name, task));
        internal void AddCleanupTask(CleanupFunction task)
           => _cleanQueue.Enqueue(new CleanupTask(task));

        #region GetStores
        public ValueTask<IEntityStore<ulong>> GetStoreAsync(StoreType type)
            => _cache.GetStoreAsync<ulong>(type);

        public ValueTask<IEntityStore<TId>> GetStoreAsync<TId>(StoreType type)
            where TId : IEquatable<TId>
            => _cache.GetStoreAsync<TId>(type);

        public ValueTask<IEntityStore<ulong>> GetSubStoreAsync(ulong id, StoreType type)
            => _cache.GetSubStoreAsync(type, id);

        public ValueTask<IEntityStore<TId>> GetSubStoreAsync<TId>(TId id, StoreType type)
            where TId : IEquatable<TId>
            => _cache.GetSubStoreAsync(type, id);

        public ValueTask<IEntityStore<TId>> GetGenericStoreAsync<TId>(Optional<TId> parent, StoreType type)
            where TId : IEquatable<TId>
            => parent.IsSpecified
                ? GetSubStoreAsync(parent.Value, type)
                : GetStoreAsync<TId>(type);
        #endregion

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
            }

            return handle;
        }

        public async Task RunCleanup(CancellationToken token = default)
        {
            await _cleanupSemaphore.WaitAsync(token);

            try
            {
                // clean the different brokers
                await Users.CleanAsync(token);
                

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

        internal async Task FreeHandles(IEnumerable<Guid> handles)
        {
            foreach(var handleId in handles)
            {
                if(_handles.TryGetValue(handleId, out var handle))
                {
                    await handle.DisposeAsync();
                } 
            }
        }
    }
}
