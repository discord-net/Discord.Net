using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket.Cache;
using UserBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketUser, Discord.WebSocket.Cache.IUserModel>;
using MemberBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuildUser, Discord.WebSocket.Cache.IMemberModel, Discord.WebSocket.Cache.IUserModel>;
using PresenseBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketPresense, Discord.WebSocket.Cache.IPresenseModel>;
using GuildBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuild, Discord.WebSocket.Cache.IGuildModel, Discord.WebSocket.SocketGuild.FactoryArgs>;
using CleanupFunction = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>;


namespace Discord.WebSocket.State
{
    internal sealed class StateController
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

        private void AddCleanupTask(string name, CleanupFunction task)
            => _cleanQueue.Enqueue(new CleanupTask(name, task));
        private void AddCleanupTask(CleanupFunction task)
           => _cleanQueue.Enqueue(new CleanupTask(task));

        #region Brokers
        public UserBroker Users
            => _userBroker ??= new UserBroker(
                    this,
                    p => GetStoreAsync(StoreType.Users),
                    (_, __, model) => ValueTask.FromResult(new SocketUser(_client, model))
               );
        private UserBroker? _userBroker;

        public MemberBroker Members
            => _memberBroker ??= new MemberBroker(this, p => GetSubStoreAsync(p, StoreType.GuildUsers), ConstructGuildUserAsync);
        private MemberBroker? _memberBroker;
        private async ValueTask<SocketGuildUser> ConstructGuildUserAsync(IUserModel? userModel, ulong guildId, IMemberModel model)
        {
            // user information is required
            if(userModel is null)
            {
                var rawUserModel = await (await GetStoreAsync(StoreType.Users)).GetAsync(model.Id)
                    ?? throw new NullReferenceException($"No user information could be found for the member ({model.Id})");

                if (rawUserModel is not IUserModel user)
                {
                    throw new InvalidCastException($"Expected user model type, but got {rawUserModel.GetType()}");
                }

                userModel = user;
            }

            return new SocketGuildUser(_client, guildId, model, userModel);
        }

        public PresenseBroker Presense
            => _presenseBroker ??= new PresenseBroker(
                    this,
                    p => GetSubStoreAsync(p, StoreType.Presence),
                    (_, __, model) => ValueTask.FromResult(new SocketPresense(_client, model))
               );
        private PresenseBroker? _presenseBroker;

        public GuildBroker Guilds
            => _guilds ??= new GuildBroker(
                    this,
                    p => GetStoreAsync(StoreType.GuildStage),
                    ConstructGuildAsync
               );
        private GuildBroker? _guilds;
        private ValueTask<SocketGuild> ConstructGuildAsync(SocketGuild.FactoryArgs? args, ulong parent, IGuildModel model)
        {
            // TODO: load dependants based on configuration
            args ??= new SocketGuild.FactoryArgs();


            return ValueTask.FromResult(new SocketGuild(_client, model.Id, model, args));
        }

        #endregion

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

        private class EntityHandle<TId, TEntity> : IEntityHandle<TId, TEntity>
            where TEntity : class, ICacheableEntity<TId>
            where TId : IEquatable<TId>
        {
            public Guid HandleId { get; }
            public TId EntityId { get; }

            public TEntity? Entity
                => _entity;

            private TEntity? _entity;
            private readonly IEntityStore<TId> _store;

            private readonly SemaphoreSlim _operationSemaphore;
            private readonly StateController _state;
            private readonly EntityHandleFlags _flags;

            private List<Action<IEntityHandle>>? _notifyComplete;

            public EntityHandle(StateController state, TEntity entity, IEntityStore<TId> store, EntityHandleFlags flags)
            {
                EntityId = entity.Id;
                HandleId = Guid.NewGuid();

                _flags = flags;
                _state = state;
                _entity = entity;
                _store = store;
                _operationSemaphore = new(1, 1);
            }

            public async Task UpdateStoreAsync(CancellationToken token)
            {
                await _operationSemaphore.WaitAsync(token);

                try
                {
                    if (_entity is null)
                        return;

                    await _store.AddOrUpdateAsync(_entity.GetModel());
                }
                finally
                {
                    _operationSemaphore.Release();
                }
            }


            public async Task RefreshAsync(CancellationToken token)
            {
                await _operationSemaphore.WaitAsync(token);

                try
                {
                    if (_entity is null)
                        return;

                    var model = await _store.GetAsync(_entity.Id);

                    if(model is null)
                    {
                        _entity = null;
                        return;
                    }

                    _entity.Update(model);
                }
                finally
                {
                    _operationSemaphore.Release();
                }

            }

            public async Task DeleteAsync(CancellationToken token)
            {
                await _operationSemaphore.WaitAsync(token);

                try
                {
                    if (_entity is null)
                        return;

                    await _store.RemoveAsync(_entity.Id);
                    _entity = null;
                }
                finally
                {
                    _operationSemaphore.Release();
                }
            }

            ~EntityHandle()
            {
                Dispose();
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                _state.AddCleanupTask($"Clean of {(_entity?.GetType().Name ?? "null entity")} with flags {_flags}", RunCleanupAsync);
            }

            public async ValueTask DisposeAsync(CancellationToken token = default)
            {
                GC.SuppressFinalize(this);
                await RunCleanupAsync(token);
            }

            public ValueTask DisposeAsync()
                => DisposeAsync(default);

            private async Task RunCleanupAsync(CancellationToken token = default)
            {
                bool deleted = false;

                if(_flags.HasFlag(EntityHandleFlags.DeleteOnRelease))
                {
                    deleted = true;
                    await DeleteAsync(token);
                }

                if(_flags.HasFlag(EntityHandleFlags.UpdateOnRelease) && !deleted)
                {
                    await UpdateStoreAsync(token);
                }

                // remove for GC
                _entity = null;

                // notify the cleanup
                if(_notifyComplete is not null)
                {
                    foreach(var notifier in _notifyComplete)
                    {
                        notifier(this);
                    }
                }
            }

            public IDisposable? CreateScopedClone(out TEntity? entity)
            {
                if(Entity is null)
                {
                    entity = null;
                    return null;
                }

                entity = (TEntity)Entity.Clone();
                return new EntityScope(entity.DisposeClone);
            }

            void IEntityHandle.AddNotifier(Action<IEntityHandle> onDispose)
                => (_notifyComplete ??= new List<Action<IEntityHandle>>()).Add(onDispose);

            private class EntityScope : IDisposable
            {
                private readonly Action _close;
                private bool _isClosed;
                private readonly object _lock = new();

                public EntityScope(Action close)
                {
                    _close = close;
                }

                public void Dispose()
                {
                    lock (_lock)
                    {
                        if (_isClosed)
                            return;

                        _close();

                        _isClosed = true;
                    }
                }
            }
        }
    }
}
