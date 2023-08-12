using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IEntityStore<TId, TModel> : IEntityStore<TId>
        where TModel : IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        new ValueTask<TModel?> GetAsync(TId id, CancellationToken token = default);

        ValueTask AddOrUpdateAsync(TModel model, CancellationToken token = default);

        ValueTask AddOrUpdateBatchAsync(IEnumerable<TModel> model, CancellationToken token = default);


        async ValueTask<IEntityModel<TId>?> IEntityStore<TId>.GetAsync(TId id, CancellationToken token)
            => await GetAsync(id, token);
    }

    public interface IEntityStore<TId> 
        where TId : IEquatable<TId>
    {
        ValueTask<IEntityModel<TId>?> GetAsync(TId id, CancellationToken token = default);

        IAsyncEnumerable<IEntityModel<TId>> GetAllAsync(CancellationToken token = default);

        IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default);

        ValueTask AddOrUpdateAsync(IEntityModel<TId> model, CancellationToken token = default);

        ValueTask AddOrUpdateBatchAsync(IEnumerable<IEntityModel<TId>> model, CancellationToken token = default);

        ValueTask RemoveAsync(TId id, CancellationToken token = default);

        ValueTask PurgeAllAsync(CancellationToken token = default);
    }
}
