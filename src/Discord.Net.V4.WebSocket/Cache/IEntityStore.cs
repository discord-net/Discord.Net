using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IEntityStore<TId, TModel> : IEntityStore<TId>
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        new ValueTask<TModel?> GetAsync(TId id, CancellationToken token = default);

        ValueTask AddOrUpdateAsync(TModel model, CancellationToken token = default);

        ValueTask AddOrUpdateBatchAsync(IEnumerable<TModel> models, CancellationToken token = default);

        new IAsyncEnumerable<TModel> QueryAsync(TId from, Direction direction, int limit);


        #region Implicit implementations
        async ValueTask<IEntityModel<TId>?> IEntityStore<TId>.GetAsync(TId id, CancellationToken token)
            => await GetAsync(id, token);

        IAsyncEnumerable<IEntityModel<TId>> IEntityStore<TId>.QueryAsync(TId from, Direction direction, int limit)
            => QueryAsync(from, direction, limit);

        ValueTask IEntityStore<TId>.AddOrUpdateAsync(IEntityModel<TId> model, CancellationToken token)
            => model is TModel tmodel
                ? AddOrUpdateAsync(tmodel, token)
                : throw new InvalidCastException($"Expected {typeof(TModel)} but got {model?.GetType().ToString() ?? "null"}");

        ValueTask IEntityStore<TId>.AddOrUpdateBatchAsync(IEnumerable<IEntityModel<TId>> models, CancellationToken token)
            => models is IEnumerable<TModel> e
                ? AddOrUpdateBatchAsync(e, token)
                : throw new InvalidCastException($"Expected {typeof(IEnumerable<TModel>)} but got {models?.GetType().ToString() ?? "null"}");

        #endregion
    }

    public interface IEntityStore<TId> 
        where TId : IEquatable<TId>
    {
        ValueTask<IEntityModel<TId>?> GetAsync(TId id, CancellationToken token = default);

        IAsyncEnumerable<IEntityModel<TId>> GetAllAsync(CancellationToken token = default);

        IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default);

        ValueTask AddOrUpdateAsync(IEntityModel<TId> model, CancellationToken token = default);

        ValueTask AddOrUpdateBatchAsync(IEnumerable<IEntityModel<TId>> models, CancellationToken token = default);

        ValueTask RemoveAsync(TId id, CancellationToken token = default);

        ValueTask PurgeAllAsync(CancellationToken token = default);

        IAsyncEnumerable<IEntityModel<TId>> QueryAsync(TId from, Direction direction, int limit);
    }
}
