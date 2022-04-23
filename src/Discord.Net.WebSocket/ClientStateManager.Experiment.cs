using Discord.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class CacheReference<TType> where TType : class
    {
        public WeakReference<TType> Reference { get; }

        public bool CanRelease
            => !Reference.TryGetTarget(out _) || _referenceCount <= 0;

        private int _referenceCount;

        private readonly object _lock = new object();

        public CacheReference(TType value)
        {
            Reference = new(value);
            _referenceCount = 1;
        }

        public bool TryObtainReference(out TType reference)
        {
            if (Reference.TryGetTarget(out reference))
            {
                Interlocked.Increment(ref _referenceCount);
                return true;
            }
            return false;
        }

        public void ReleaseReference()
        {
            lock (_lock)
            {
                if (_referenceCount > 0)
                    _referenceCount--;
            }
        }
    }
    internal class ReferenceStore<TEntity, TModel, TId, ISharedEntity>
        where TEntity : class, ICached<TModel>, ISharedEntity
        where TModel : IEntityModel<TId>
        where TId : IEquatable<TId>
        where ISharedEntity : class
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ConcurrentDictionary<TId, CacheReference<TEntity>> _references = new();
        private IEntityStore<TModel, TId> _store;
        private Func<TModel, TEntity> _entityBuilder;
        private Func<TId, RequestOptions, Task<ISharedEntity>> _restLookup;
        private readonly bool _allowSyncWaits;
        private readonly object _lock = new();

        public ReferenceStore(ICacheProvider cacheProvider, Func<TModel, TEntity> entityBuilder, Func<TId, RequestOptions, Task<ISharedEntity>> restLookup, bool allowSyncWaits)
        {
            _allowSyncWaits = allowSyncWaits;
            _cacheProvider = cacheProvider;
            _entityBuilder = entityBuilder;
            _restLookup = restLookup;
        }

        internal void ClearDeadReferences()
        {
            lock (_lock)
            {
                var references = _references.Where(x => x.Value.CanRelease).ToArray();
                foreach (var reference in references)
                    _references.TryRemove(reference.Key, out _);
            }
        }

        private TResult RunOrThrowValueTask<TResult>(ValueTask<TResult> t)
        {
            if (_allowSyncWaits)
            {
                return t.GetAwaiter().GetResult();
            }
            else if (t.IsCompleted)
                return t.Result;
            else
                throw new InvalidOperationException("Cannot run asynchronous value task in synchronous context");
        }

        private void RunOrThrowValueTask(ValueTask t)
        {
            if (_allowSyncWaits)
            {
                t.GetAwaiter().GetResult();
            }
            else if (!t.IsCompleted)
                throw new InvalidOperationException("Cannot run asynchronous value task in synchronous context");
        }

        public async ValueTask InitializeAsync()
        {
            _store ??= await _cacheProvider.GetStoreAsync<TModel, TId>().ConfigureAwait(false);
        }

        public async ValueTask InitializeAsync(TId parentId)
        {
            _store ??= await _cacheProvider.GetSubStoreAsync<TModel, TId>(parentId).ConfigureAwait(false);
        }

        private bool TryGetReference(TId id, out TEntity entity)
        {
            entity = null;
            return _references.TryGetValue(id, out var reference) && reference.TryObtainReference(out entity); 
        }

        public TEntity Get(TId id)
        {
            if(TryGetReference(id, out var entity))
            {
                return entity;
            }

            var model = RunOrThrowValueTask(_store.GetAsync(id, CacheRunMode.Sync));

            if (model != null)
            {
                entity = _entityBuilder(model);
                _references.TryAdd(id, new CacheReference<TEntity>(entity));
                return entity;
            }

            return null;
        }

        public async ValueTask<ISharedEntity> GetAsync(TId id, CacheMode mode, RequestOptions options = null)
        {
            if (TryGetReference(id, out var entity))
            {
                return entity;
            }

            var model = await _store.GetAsync(id, CacheRunMode.Async).ConfigureAwait(false);

            if (model != null)
            {
                entity = _entityBuilder(model);
                _references.TryAdd(id, new CacheReference<TEntity>(entity));
                return entity;
            }    

            if(mode == CacheMode.AllowDownload)
            {
                return await _restLookup(id, options).ConfigureAwait(false);
            }

            return null;
        }

        public IEnumerable<TEntity> GetAll()
        {
            var models = RunOrThrowValueTask(_store.GetAllAsync(CacheRunMode.Sync).ToArrayAsync());
            return models.Select(x =>
            {
                var entity = _entityBuilder(x);
                _references.TryAdd(x.Id, new CacheReference<TEntity>(entity));
                return entity;
            });
        }

        public async IAsyncEnumerable<TEntity> GetAllAsync()
        {
            await foreach(var model in _store.GetAllAsync(CacheRunMode.Async))
            {
                var entity = _entityBuilder(model);
                _references.TryAdd(model.Id, new CacheReference<TEntity>(entity));
                yield return entity;
            }
        }

        public TEntity GetOrAdd(TId id, Func<TId, TModel> valueFactory)
        {
            var entity = Get(id);
            if (entity != null)
                return entity;

            var model = valueFactory(id);
            AddOrUpdate(model);
            return _entityBuilder(model);
        }

        public async ValueTask<TEntity> GetOrAddAsync(TId id, Func<TId, TModel> valueFactory)
        {
            var entity = await GetAsync(id, CacheMode.CacheOnly).ConfigureAwait(false);
            if (entity != null)
                return (TEntity)entity;

            var model = valueFactory(id);
            await AddOrUpdateAsync(model);
            return _entityBuilder(model);
        }

        public void AddOrUpdate(TModel model)
        {
            RunOrThrowValueTask(_store.AddOrUpdateAsync(model, CacheRunMode.Sync));
            if (TryGetReference(model.Id, out var reference))
                reference.Update(model);
        }

        public ValueTask AddOrUpdateAsync(TModel model)
        {
            if (TryGetReference(model.Id, out var reference))
                reference.Update(model);
            return _store.AddOrUpdateAsync(model, CacheRunMode.Async);
        }

        public void Remove(TId id)
        {
            RunOrThrowValueTask(_store.RemoveAsync(id, CacheRunMode.Sync));
            _references.TryRemove(id, out _);
        }

        public ValueTask RemoveAsync(TId id)
        {
            _references.TryRemove(id, out _);
            return _store.RemoveAsync(id, CacheRunMode.Async);
        }

        public void Purge()
        {
            RunOrThrowValueTask(_store.PurgeAllAsync(CacheRunMode.Sync));
            _references.Clear();
        }

        public ValueTask PurgeAsync()
        {
            _references.Clear();
            return _store.PurgeAllAsync(CacheRunMode.Async);
        }
    }

    internal partial class ClientStateManager
    {
        public ReferenceStore<SocketGlobalUser, IUserModel, ulong, IUser> UserStore;
        public ReferenceStore<SocketPresence, IPresenceModel, ulong, IPresence> PresenceStore;
        private ConcurrentDictionary<ulong, ReferenceStore<SocketGuildUser, IMemberModel, ulong, IGuildUser>> _memberStores;
        private ConcurrentDictionary<ulong, ReferenceStore<SocketThreadUser, IThreadMemberModel, ulong, IThreadUser>> _threadMemberStores;

        private SemaphoreSlim _memberStoreLock;
        private SemaphoreSlim _threadMemberLock;

        private void CreateStores()
        {
            UserStore = new ReferenceStore<SocketGlobalUser, IUserModel, ulong, IUser>(
                _cacheProvider,
                m => SocketGlobalUser.Create(_client, m),
                async (id, options) => await _client.Rest.GetUserAsync(id, options).ConfigureAwait(false),
                AllowSyncWaits);

            PresenceStore = new ReferenceStore<SocketPresence, IPresenceModel, ulong, IPresence>(
                _cacheProvider,
                m => SocketPresence.Create(m),
                (id, options) => Task.FromResult<IPresence>(null),
                AllowSyncWaits);

            _memberStores = new();
            _threadMemberStores = new();

            _threadMemberLock = new(1, 1);
            _memberStoreLock = new(1,1);
        }

        public void ClearDeadReferences()
        {
            UserStore.ClearDeadReferences();
            PresenceStore.ClearDeadReferences();
        }

        public async ValueTask InitializeAsync()
        {
            await UserStore.InitializeAsync();
            await PresenceStore.InitializeAsync();
        }

        public bool TryGetMemberStore(ulong guildId, out ReferenceStore<SocketGuildUser, IMemberModel, ulong, IGuildUser> store)
            => _memberStores.TryGetValue(guildId, out store);

        public async ValueTask<ReferenceStore<SocketGuildUser, IMemberModel, ulong, IGuildUser>> GetMemberStoreAsync(ulong guildId)
        {
            if (_memberStores.TryGetValue(guildId, out var store))
                return store;

            await _memberStoreLock.WaitAsync().ConfigureAwait(false);

            try
            {
                store = new ReferenceStore<SocketGuildUser, IMemberModel, ulong, IGuildUser>(
                    _cacheProvider,
                    m => SocketGuildUser.Create(guildId, _client, m),
                    async (id, options) => await _client.Rest.GetGuildUserAsync(guildId, id, options).ConfigureAwait(false),
                    AllowSyncWaits);

                await store.InitializeAsync(guildId).ConfigureAwait(false);

                _memberStores.TryAdd(guildId, store);
                return store;
            }
            finally
            {
                _memberStoreLock.Release();
            }
        }

        public async Task<ReferenceStore<SocketThreadUser, IThreadMemberModel, ulong, IThreadUser>> GetThreadMemberStoreAsync(ulong threadId, ulong guildId)
        {
            if (_threadMemberStores.TryGetValue(threadId, out var store))
                return store;

            await _threadMemberLock.WaitAsync().ConfigureAwait(false);

            try
            {
                store = new ReferenceStore<SocketThreadUser, IThreadMemberModel, ulong, IThreadUser>(
                    _cacheProvider,
                    m => SocketThreadUser.Create(_client, guildId, threadId, m),
                    async (id, options) => await ThreadHelper.GetUserAsync(id, _client.GetChannel(threadId) as SocketThreadChannel, _client, options).ConfigureAwait(false),
                    AllowSyncWaits);

                await store.InitializeAsync().ConfigureAwait(false);

                _threadMemberStores.TryAdd(threadId, store);
                return store;
            }
            finally
            {
                _threadMemberLock.Release();
            }
        }
    }
}
