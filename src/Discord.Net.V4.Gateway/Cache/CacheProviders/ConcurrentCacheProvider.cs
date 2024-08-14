using Discord.Models;
using System;
using System.Collections.Concurrent;

namespace Discord.Gateway;

public sealed partial class ConcurrentCacheProvider : ICacheProvider
{
    private readonly ConcurrentDictionary<Type, IStore> _cache = new();

    [TypeFactory]
    public ConcurrentCacheProvider(DiscordGatewayClient client, DiscordGatewayConfig config)
    {

    }

    public ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync<TId, TModel>(CancellationToken token = default)
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        if (
            _cache.GetOrAdd(typeof(TModel), _ => new Store<TId, TModel>())
            is not IEntityModelStore<TId, TModel> store
        ) throw new DiscordException("Corrupted cache data");

        return ValueTask.FromResult(store);
    }

    private interface IStore
    {
        Type ModelType { get; }
    }

    private sealed class Store<TId, TModel> :
        IStore,
        IEntityModelStore<TId, TModel>
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        public Type ModelType => typeof(TModel);

        private readonly ConcurrentDictionary<TId, TModel> _cache = new();
        private readonly ConcurrentDictionary<TId, IStore> _subStoreCache = new();

        public ValueTask<IEntityModelStore<TSubStoreId, TSubStoreModel>>
            GetSubStoreAsync<TSubStoreId, TSubStoreModel>(
                TId id,
                CancellationToken token = default
            )
            where TSubStoreId : IEquatable<TSubStoreId>
            where TSubStoreModel : class, IEntityModel<TSubStoreId>
        {
            if (
                _subStoreCache.GetOrAdd(id, _ => new Store<TId, TModel>())
                is not IEntityModelStore<TSubStoreId, TSubStoreModel> store
            ) throw new DiscordException("Corrupted cache data");

            return ValueTask.FromResult(store);
        }

        public ValueTask<TModel?> GetAsync(TId id, CancellationToken token = default)
            => ValueTask.FromResult(_cache.GetValueOrDefault(id));

        public IAsyncEnumerable<TModel> GetManyAsync(IEnumerable<TId> ids, CancellationToken token = default)
            => ids.Select(x => _cache.GetValueOrDefault(x)).OfType<TModel>().ToAsyncEnumerable();

        public IAsyncEnumerable<TModel> GetAllAsync(CancellationToken token = default)
            => _cache.Values.ToAsyncEnumerable();

        public IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default)
            => _cache.Keys.ToAsyncEnumerable();

        public ValueTask AddOrUpdateAsync(TModel model, CancellationToken token = default)
        {
            _cache[model.Id] = model;
            return ValueTask.CompletedTask;
        }

        public ValueTask AddOrUpdateBatchAsync(IEnumerable<TModel> models, CancellationToken token = default)
        {
            foreach (var model in models)
            {
                _cache[model.Id] = model;
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask RemoveAsync(TId id, CancellationToken token = default)
        {
            _cache.Remove(id, out _);
            return ValueTask.CompletedTask;
        }

        public ValueTask PurgeAllAsync(CancellationToken token = default)
        {
            _cache.Clear();
            return ValueTask.CompletedTask;
        }

        public IAsyncEnumerable<TModel> QueryAsync(
            TId from,
            Optional<TId> to,
            Direction direction,
            int? limit,
            CancellationToken token = default)
        {
            // TODO
            return null!;
        }
    }
}
