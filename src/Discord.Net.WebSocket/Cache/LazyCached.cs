using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a lazily-loaded cached value that can be loaded synchronously or asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The primary id type of the entity.</typeparam>
    public class LazyCached<TEntity, TId>
        where TEntity : class, ICached
        where TId : IEquatable<TId>
    {
        /// <summary>
        ///     Gets or loads the cached value synchronously.
        /// </summary>
        public TEntity Value
            => GetOrLoad();

        /// <summary>
        ///     Gets whether or not the <see cref="Value"/> has been loaded and is still alive.
        /// </summary>
        public bool IsValueCreated
            => _loadedValue != null && _loadedValue.IsFreed;

        private TEntity _loadedValue;
        private readonly ILookupReferenceStore<TEntity, TId> _store;
        private readonly TId _id;
        private readonly object _lock = new();

        internal LazyCached(TEntity value)
        {
            _loadedValue = value;
        }

        internal LazyCached(TId id, ILookupReferenceStore<TEntity, TId> store)
        {
            _store = store;
            _id = id;
        }

        private TEntity GetOrLoad()
        {
            lock (_lock)
            {
                if(!IsValueCreated)
                    _loadedValue = _store.Get(_id);
                return _loadedValue;
            }
        }

        /// <summary>
        ///     Gets or loads the value from the cache asynchronously.
        /// </summary>
        /// <returns>The loaded or fetched entity.</returns>
        public async ValueTask<TEntity> GetAsync()
        {
            if (!IsValueCreated)
                _loadedValue = await _store.GetAsync(_id).ConfigureAwait(false);
            return _loadedValue;
        }
    }

    public class LazyCached<TEntity> : LazyCached<TEntity, ulong>
        where TEntity : class, ICached
    {
        internal LazyCached(ulong id, ILookupReferenceStore<TEntity, ulong> store)
            : base(id, store) { }
        internal LazyCached(TEntity entity)
            : base(entity) { }
    }
}
