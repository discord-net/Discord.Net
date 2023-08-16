using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State
{
    internal sealed class EntityHandle<TId, TEntity> : IEntityHandle<TId, TEntity>
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

                if (model is null)
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

            if (_flags.HasFlag(EntityHandleFlags.DeleteOnRelease))
            {
                deleted = true;
                await DeleteAsync(token);
            }

            if (_flags.HasFlag(EntityHandleFlags.UpdateOnRelease) && !deleted)
            {
                await UpdateStoreAsync(token);
            }

            // remove for GC
            _entity = null;

            // notify the cleanup
            if (_notifyComplete is not null)
            {
                foreach (var notifier in _notifyComplete)
                {
                    notifier(this);
                }
            }
        }

        public IDisposable? CreateScopedClone(out TEntity? entity)
        {
            if (Entity is null)
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
