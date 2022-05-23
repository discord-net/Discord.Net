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
            Interlocked.Decrement(ref _referenceCount);
        }
    }

    internal interface ILookupReferenceStore<TEntity, TId>
    {
        TEntity Get(TId id);
        ValueTask<TEntity> GetAsync(TId id); 
    }

    internal class ReferenceStore<TEntity, TModel, TId, TSharedEntity> : ILookupReferenceStore<TEntity, TId>
        where TEntity : class, ICached<TModel>, TSharedEntity
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
        where TSharedEntity : class
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ConcurrentDictionary<TId, CacheReference<TEntity>> _references = new();
        private IEntityStore<TModel, TId> _store;
        private Func<TModel, TEntity> _entityBuilder;
        private Func<TModel> _modelFactory;
        private Func<TId, RequestOptions, Task<TSharedEntity>> _restLookup;
        private readonly object _lock = new();

        public ReferenceStore(ICacheProvider cacheProvider,
            Func<TModel, TEntity> entityBuilder,
            Func<TId, RequestOptions, Task<TSharedEntity>> restLookup,
            Func<TModel> userDefinedModelFactory)
        {
            _cacheProvider = cacheProvider;
            _entityBuilder = entityBuilder;
            _restLookup = restLookup;
            _modelFactory = userDefinedModelFactory;
        }

        private TModel GetUserDefinedModel(TModel t)
            => t.ToSpecifiedModel(_modelFactory());

        internal bool RemoveReference(TId id)
        {
            if(_references.TryGetValue(id, out var rf))
            {
                rf.ReleaseReference();

                if (rf.CanRelease)
                    return _references.TryRemove(id, out _);
            }

            return false;
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

            var model = _store.Get(id);

            if (model != null)
            {
                entity = _entityBuilder(model);
                _references.TryAdd(id, new CacheReference<TEntity>(entity));
                return entity;
            }

            return null;
        }

        public async ValueTask<TSharedEntity> GetAsync(TId id, CacheMode mode, RequestOptions options = null)
        {
            if (TryGetReference(id, out var entity))
            {
                return entity;
            }

            var model = await _store.GetAsync(id).ConfigureAwait(false);

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
            var models = _store.GetAll();
            return models.Select(x =>
            {
                var entity = _entityBuilder(x);
                _references.TryAdd(x.Id, new CacheReference<TEntity>(entity));
                return entity;
            });
        }

        public async IAsyncEnumerable<TEntity> GetAllAsync()
        {
            await foreach(var model in _store.GetAllAsync())
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
            await AddOrUpdateAsync(model).ConfigureAwait(false);
            return _entityBuilder(model);
        }

        public void AddOrUpdate(TModel model)
        {
            var userDefinedModel = GetUserDefinedModel(model);
            _store.AddOrUpdate(userDefinedModel);
            if (TryGetReference(model.Id, out var reference))
                reference.Update(userDefinedModel);
        }

        public ValueTask AddOrUpdateAsync(TModel model)
        {
            var userDefinedModel = GetUserDefinedModel(model);
            if (TryGetReference(userDefinedModel.Id, out var reference))
                reference.Update(userDefinedModel);
            return _store.AddOrUpdateAsync(userDefinedModel);
        }

        public void BulkAddOrUpdate(IEnumerable<TModel> models)
        {
            models = models.Select(x => GetUserDefinedModel(x));
            _store.AddOrUpdateBatch(models);
            foreach (var model in models)
            {
                if (_references.TryGetValue(model.Id, out var rf) && rf.Reference.TryGetTarget(out var entity))
                    entity.Update(model);
            }
        }

        public async ValueTask BulkAddOrUpdateAsync(IEnumerable<TModel> models)
        {
            models = models.Select(x => GetUserDefinedModel(x));
            await _store.AddOrUpdateBatchAsync(models).ConfigureAwait(false);

            foreach (var model in models)
            {
                if (_references.TryGetValue(model.Id, out var rf) && rf.Reference.TryGetTarget(out var entity))
                    entity.Update(model);
            }
        }

        public void Remove(TId id)
        {
            _store.Remove(id);
            _references.TryRemove(id, out _);
        }

        public ValueTask RemoveAsync(TId id)
        {
            _references.TryRemove(id, out _);
            return _store.RemoveAsync(id);
        }

        public void Purge()
        {
            _store.PurgeAll();
            _references.Clear();
        }

        public ValueTask PurgeAsync()
        {
            _references.Clear();
            return _store.PurgeAllAsync();
        }

        public IEnumerable<TEntity> GetEnumerable(IEnumerable<TId> ids)
        {
            foreach (var id in ids)
            {
                yield return Get(id);
            }
        }

        public async IAsyncEnumerable<TEntity> GetEnumerableAsync(IEnumerable<TId> ids)
        {
            foreach (var id in ids)
            {
                yield return (TEntity)await GetAsync(id, CacheMode.CacheOnly);
            }
        }

        TEntity ILookupReferenceStore<TEntity, TId>.Get(TId id) => Get(id);
        async ValueTask<TEntity> ILookupReferenceStore<TEntity, TId>.GetAsync(TId id) => (TEntity)await GetAsync(id, CacheMode.CacheOnly).ConfigureAwait(false);
    }

    internal partial class ClientStateManager
    {
        public ReferenceStore<SocketUser, IUserModel, ulong, IUser> UserStore;
        public ReferenceStore<SocketPresence, IPresenceModel, ulong, IPresence> PresenceStore;

        private ConcurrentDictionary<ulong, ReferenceStore<SocketGuildUser, IMemberModel, ulong, IGuildUser>> _memberStores;
        private ConcurrentDictionary<ulong, ReferenceStore<SocketThreadUser, IThreadMemberModel, ulong, IThreadUser>> _threadMemberStores;
        private ConcurrentDictionary<ulong, ReferenceStore<SocketMessage, IMessageModel, ulong, IMessage>> _messageStores;

        private SemaphoreSlim _memberStoreLock;
        private SemaphoreSlim _messageStoreLock;
        private SemaphoreSlim _threadMemberLock;

        #region Models
        private readonly Dictionary<Type, Func<object>> _defaultModelFactory = new()
        {
            { typeof(IUserModel), () => new SocketUser.CacheModel() },
            { typeof(IMemberModel), () => new SocketGuildUser.CacheModel() },
            { typeof(ICurrentUserModel), () => new SocketSelfUser.CacheModel() },
            { typeof(IThreadMemberModel), () => new SocketThreadUser.CacheModel() },
            { typeof(IPresenceModel), () => new SocketPresence.CacheModel() },
            { typeof(IActivityModel), () => new SocketPresence.ActivityCacheModel() },
            { typeof(IMessageModel), () => new SocketMessage.CacheModel() },
            { typeof(IMessageActivityModel), () => new SocketMessage.CacheModel.MessageActivityModel() },
            { typeof(IMessageComponentModel), () => new SocketMessage.CacheModel.MessageComponentModel() },
            { typeof(IMessageComponentOptionModel), () => new SocketMessage.CacheModel.MessageComponentModel.MessageComponentOptionModel() },
            { typeof(IPartialApplicationModel), () => new SocketMessage.CacheModel.PartialApplicationModel() },
            { typeof(IStickerItemModel), () => new SocketMessage.CacheModel.StickerItemModel() },
            { typeof(IReactionMetadataModel), () => new SocketMessage.CacheModel.ReactionModel() },
            { typeof(IEmbedModel), () => new SocketMessage.CacheModel.EmbedModel() },
            { typeof(IEmbedFieldModel), () => new SocketMessage.CacheModel.EmbedModel.EmbedFieldModel() },
            { typeof(IEmbedMediaModel), () => new SocketMessage.CacheModel.EmbedModel.EmbedMediaModel()}

        };

        public TModel GetModel<TModel, TFallback>()
            where TFallback : class, TModel, new()
            where TModel : class
        {
            return GetModel<TModel>() ?? new TFallback();
        }

        public TModel GetModel<TModel>()
            where TModel : class
        {
            var type = _cacheProvider.GetModel<TModel>();

            if (type != null)
            {
                if (!type.GetInterfaces().Contains(typeof(TModel)))
                    throw new InvalidOperationException($"Cannot use {type.Name} as a model for {typeof(TModel).Name}");

                return (TModel)Activator.CreateInstance(type);
            }
            else
                return _defaultModelFactory.TryGetValue(typeof(TModel), out var m) ? (TModel)m() : null;
        }
        #endregion

        #region References & Initialization
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

        private void CreateStores()
        {
            UserStore = new ReferenceStore<SocketUser, IUserModel, ulong, IUser>(
                _cacheProvider,
                m => SocketGlobalUser.Create(_client, m),
                async (id, options) => await _client.Rest.GetUserAsync(id, options).ConfigureAwait(false),
                GetModel<IUserModel>);

            PresenceStore = new ReferenceStore<SocketPresence, IPresenceModel, ulong, IPresence>(
                _cacheProvider,
                m => SocketPresence.Create(_client, m),
                (id, options) => Task.FromResult<IPresence>(null),
                GetModel<IPresenceModel>);

            _memberStores = new();
            _threadMemberStores = new();

            _threadMemberLock = new(1, 1);
            _memberStoreLock = new(1, 1);
        }
        #endregion

        #region Members
        public ReferenceStore<SocketGuildUser, IMemberModel, ulong, IGuildUser> GetMemberStore(ulong guildId)
            => TryGetMemberStore(guildId, out var store) ? store : null;

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
                    GetModel<IMemberModel>);

                await store.InitializeAsync(guildId).ConfigureAwait(false);

                _memberStores.TryAdd(guildId, store);
                return store;
            }
            finally
            {
                _memberStoreLock.Release();
            }
        }
        #endregion

        #region Thread Members
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
                    GetModel<IThreadMemberModel>);

                await store.InitializeAsync().ConfigureAwait(false);

                _threadMemberStores.TryAdd(threadId, store);
                return store;
            }
            finally
            {
                _threadMemberLock.Release();
            }
        }

        public ReferenceStore<SocketThreadUser, IThreadMemberModel, ulong, IThreadUser> GetThreadMemberStore(ulong threadId)
            => _threadMemberStores.TryGetValue(threadId, out var store) ? store : null;
        #endregion

        #region Messages
        public ReferenceStore<SocketMessage, IMessageModel, ulong, IMessage> GetMessageStore(ulong channelId)
           => TryGetMessageStore(channelId, out var store) ? store : null;

        public bool TryGetMessageStore(ulong channelId, out ReferenceStore<SocketMessage, IMessageModel, ulong, IMessage> store)
            => _messageStores.TryGetValue(channelId, out store);

        public async ValueTask<ReferenceStore<SocketMessage, IMessageModel, ulong, IMessage>> GetMessageStoreAsync(ulong channelId)
        {
            if (_messageStores.TryGetValue(channelId, out var store))
                return store;

            await _messageStoreLock.WaitAsync().ConfigureAwait(false);

            try
            {
                store = new ReferenceStore<SocketMessage, IMessageModel, ulong, IMessage>(
                    _cacheProvider,
                    m => SocketMessage.Create(_client, m, channelId),
                    async (id, options) => await _client.Rest.GetMessageAsync(channelId, id).ConfigureAwait(false),
                    GetModel<IMessageModel>);

                await store.InitializeAsync(channelId).ConfigureAwait(false);

                _messageStores.TryAdd(channelId, store);
                return store;
            }
            finally
            {
                _memberStoreLock.Release();
            }
        }
        #endregion
    }
}
