using System;
using System.Collections.Concurrent;

namespace Discord.WebSocket.Cache
{
    public sealed class ConcurrentCacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<StoreType, IStore> _stores;

        private interface IStore
        {
            Type IdType { get; }
        }

        private sealed class Store<TId> : IEntityStore<TId>, IStore
            where TId : IEquatable<TId>
        {
            private readonly ConcurrentDictionary<TId, IEntityModel<TId>> _cache;
            private readonly ConcurrentDictionary<TId, Store<TId>> _subStores;

            public Store()
            {
                _cache = new();
                _subStores = new();
            }

            Type IStore.IdType => typeof(TId);

            public ValueTask AddOrUpdateAsync(IEntityModel<TId> model, CancellationToken token = default(CancellationToken))
            {
                _cache.AddOrUpdate(model.Id, model, (_, __) => model);
                return ValueTask.CompletedTask;
            }

            public ValueTask AddOrUpdateBatchAsync(IEnumerable<IEntityModel<TId>> models, CancellationToken token = default(CancellationToken))
            {
                foreach(var model in models)
                {
                    _cache.AddOrUpdate(model.Id, model, (_, __) => model);
                }

                return ValueTask.CompletedTask;
            }


            public IAsyncEnumerable<IEntityModel<TId>> GetAllAsync(CancellationToken token = default(CancellationToken))
            {
                return _cache.Values.ToAsyncEnumerable();
            }

            public IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default(CancellationToken))
            {
                return _cache.Keys.ToAsyncEnumerable();
            }

            public ValueTask<IEntityModel<TId>?> GetAsync(TId id, CancellationToken token = default(CancellationToken))
            {
                return ValueTask.FromResult(_cache.TryGetValue(id, out var value) ? value : null);
            }

            public IStore GetSubStore(TId id)
                => _subStores.GetOrAdd(id, _ => new Store<TId>());

            public ValueTask PurgeAllAsync(CancellationToken token = default(CancellationToken))
            {
                _cache.Clear();
                return ValueTask.CompletedTask;
            }

            public IAsyncEnumerable<IEntityModel<TId>> QueryAsync(TId from, Direction direction, int limit)
            {
                if (from is not IComparable<TId> compareable)
                    throw new InvalidOperationException("Cannot compare keys");

                return direction switch
                {
                    Direction.Before => _cache.Where(x => compareable.CompareTo(x.Key) < 0).Take(limit).Select(x => x.Value).ToAsyncEnumerable(),
                    Direction.After => _cache.Where(x => compareable.CompareTo(x.Key) > 0).Take(limit).Select(x => x.Value).ToAsyncEnumerable(),
                    Direction.Around => _cache
                                            .Where(x => compareable.CompareTo(x.Key) < 0)
                                            .Take(limit / 2)
                                            .Concat(
                                                _cache
                                                .Where(x => compareable.CompareTo(x.Key) > 0)
                                                .Take(limit / 2)
                                            )
                                            .Select(x => x.Value)
                                            .ToAsyncEnumerable(),
                    _ => throw new ArgumentException($"Unknown/unsupported direction {direction}"),
                };
            }

            public ValueTask RemoveAsync(TId id, CancellationToken token = default(CancellationToken))
            {
                _cache.TryRemove(id, out _);
                return ValueTask.CompletedTask;
            }
        }

        public ConcurrentCacheProvider()
        {
            _stores = new();
        }

        public ValueTask<IEntityStore<TId>> GetStoreAsync<TId>(StoreType type) where TId : IEquatable<TId>
        {
            var polledStore = _stores.GetOrAdd(type, _ => new Store<TId>());

            if (polledStore is not IEntityStore<TId> store)
                throw new InvalidOperationException($"Expected {type} store to be of ID {typeof(TId)}, but got {polledStore}");

            return ValueTask.FromResult(store);
        }

        public ValueTask<IEntityStore<TId>> GetSubStoreAsync<TId>(StoreType type, TId parentId) where TId : IEquatable<TId>
        {
            var subStore = ((Store<TId>)_stores.GetOrAdd(type, _ => new Store<TId>())).GetSubStore(parentId);

            if (subStore is not IEntityStore<TId> store)
                throw new InvalidOperationException($"Expected {type} store to be of ID {typeof(TId)}, but got {subStore}");

            return ValueTask.FromResult(store);
        }
    }
}

