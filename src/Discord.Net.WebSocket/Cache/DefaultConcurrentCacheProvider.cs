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

        private readonly Dictionary<Type, Type> _models = new()
        {
            { typeof(IUserModel), typeof(API.User) },
            { typeof(ICurrentUserModel), typeof(API.CurrentUser) },
            { typeof(IMemberModel), typeof(API.GuildMember) },
            { typeof(IThreadMemberModel), typeof(API.ThreadMember)},
            { typeof(IPresenceModel), typeof(API.Presence)},
            { typeof(IActivityModel), typeof(API.Game)}
        };

        private class DefaultEntityStore<TModel, TId> : IEntityStore<TModel, TId>
            where TModel : IEntityModel<TId>
            where TId : IEquatable<TId>
        {
            private ConcurrentDictionary<TId, TModel> _cache;

            public DefaultEntityStore(ConcurrentDictionary<TId, TModel> cache)
            {
                _cache = cache;
            }

            public TModel Get(TId id)
            {
                if (_cache.TryGetValue(id, out var model))
                    return model;
                return default;
            }
            public IEnumerable<TModel> GetAll()
            {
                return _cache.Select(x => x.Value);
            }
            public void AddOrUpdate(TModel model)
            {
                _cache.AddOrUpdate(model.Id, model, (_, __) => model);
            }
            public void AddOrUpdateBatch(IEnumerable<TModel> models)
            {
                foreach (var model in models)
                    _cache.AddOrUpdate(model.Id, model, (_, __) => model);
            }
            public void Remove(TId id)
            {
                _cache.TryRemove(id, out _);
            }
            public void PurgeAll()
            {
                _cache.Clear();
            }

            ValueTask<TModel> IEntityStore<TModel, TId>.GetAsync(TId id) => new ValueTask<TModel>(Get(id));
            IAsyncEnumerable<TModel> IEntityStore<TModel, TId>.GetAllAsync()
            {
                var enumerator = GetAll().GetEnumerator();
                return AsyncEnumerable.Create((cancellationToken)
                 => AsyncEnumerator.Create(
                     () => new ValueTask<bool>(enumerator.MoveNext()),
                     () => enumerator.Current,
                     () => new ValueTask())
                );
            }
            ValueTask IEntityStore<TModel, TId>.AddOrUpdateAsync(TModel model)
            {
                AddOrUpdate(model);
                return default;
            }
            ValueTask IEntityStore<TModel, TId>.AddOrUpdateBatchAsync(IEnumerable<TModel> models)
            {
                AddOrUpdateBatch(models);
                return default;
            }
            ValueTask IEntityStore<TModel, TId>.RemoveAsync(TId id)
            {
                Remove(id);
                return default;
            }
            ValueTask IEntityStore<TModel, TId>.PurgeAllAsync()
            {
                PurgeAll();
                return default;
            }
        }

        public Type GetModel<TInterface>()
        {
            if (_models.TryGetValue(typeof(TInterface), out var t))
                return t;
            return null;
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
