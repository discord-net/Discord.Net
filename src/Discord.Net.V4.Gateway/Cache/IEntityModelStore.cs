using Discord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

public interface IEntityModelStore<TId, TModel> : IEntityModelStore
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    ValueTask<IEntityModelStore<TSubStoreId, TSubStoreModel>> GetSubStoreAsync<TSubStoreId, TSubStoreModel>(
        TId id,
        CancellationToken token = default)
        where TSubStoreId : IEquatable<TSubStoreId>
        where TSubStoreModel : class, IEntityModel<TSubStoreId>;

    ValueTask<TModel?> GetAsync(TId id, CancellationToken token = default);
    IAsyncEnumerable<TModel> GetAllAsync(CancellationToken token = default);
    IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default);

    ValueTask AddOrUpdateAsync(TModel model, CancellationToken token = default);
    ValueTask AddOrUpdateBatchAsync(IEnumerable<TModel> models, CancellationToken token = default);

    ValueTask RemoveAsync(TId id, CancellationToken token = default);
    ValueTask PurgeAllAsync(CancellationToken token = default);

    IAsyncEnumerable<TModel> QueryAsync(TId from, Direction direction, int limit);

    Type IEntityModelStore.IdType => typeof(TId);
    Type IEntityModelStore.ModelType => typeof(TModel);
}

public interface IEntityModelStore
{
    Type IdType { get; }
    Type ModelType { get; }
}
