using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public class DefaultConcurrentCacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<Type, object> _storeCache = new();
        private readonly ConcurrentDictionary<object, object> _subStoreCache = new();

        private class DefaultEntityStore<TModel, TId> : IEntityStore<TModel, TId>
            where TModel : IEntityModel<TId>
            where TId : IEquatable<TId>
        {
            private ConcurrentDictionary<TId, TModel> _cache;

            public DefaultEntityStore(ConcurrentDictionary<TId, TModel> cache)
            {
                _cache = cache;
            }

            public ValueTask AddOrUpdateAsync(TModel model, CacheRunMode runmode)
            {
                _cache.AddOrUpdate(model.Id, model, (_, __) => model);
                return default;
            }

            public ValueTask AddOrUpdateBatchAsync(IEnumerable<TModel> models, CacheRunMode runmode)
            {
                foreach (var model in models)
                    _cache.AddOrUpdate(model.Id, model, (_, __) => model);
                return default;
            }

            public IAsyncEnumerable<TModel> GetAllAsync(CacheRunMode runmode)
            {
                var coll = _cache.Select(x => x.Value).GetEnumerator();
                return AsyncEnumerable.Create((_) => AsyncEnumerator.Create(
                    () => new ValueTask<bool>(coll.MoveNext()),
                    () => coll.Current,
                    () => new ValueTask()));
            }
            public ValueTask<TModel> GetAsync(TId id, CacheRunMode runmode)
            {
                if (_cache.TryGetValue(id, out var model))
                    return new ValueTask<TModel>(model);
                return default;
            }
            public ValueTask RemoveAsync(TId id, CacheRunMode runmode)
            {
                _cache.TryRemove(id, out _);
                return default;
            }

            public ValueTask PurgeAllAsync(CacheRunMode runmode)
            {
                _cache.Clear();
                return default;
            }
        }

        public virtual ValueTask<IEntityStore<TModel, TId>> GetStoreAsync<TModel, TId>()
            where TModel : IEntityModel<TId>
            where TId : IEquatable<TId>
        {
            var store = _storeCache.GetOrAdd(typeof(TModel), (_) => new DefaultEntityStore<TModel, TId>(new ConcurrentDictionary<TId, TModel>()));
            return new ValueTask<IEntityStore<TModel, TId>>((IEntityStore<TModel, TId>)store);
        }

        public virtual ValueTask<IEntityStore<TModel, TId>> GetSubStoreAsync<TModel, TId>(TId parentId)
            where TModel : IEntityModel<TId>
            where TId : IEquatable<TId>
        {
            var store = _subStoreCache.GetOrAdd(parentId, (_) => new DefaultEntityStore<TModel, TId>(new ConcurrentDictionary<TId, TModel>()));
            return new ValueTask<IEntityStore<TModel, TId>>((IEntityStore<TModel, TId>)store);
        }
    }
}
