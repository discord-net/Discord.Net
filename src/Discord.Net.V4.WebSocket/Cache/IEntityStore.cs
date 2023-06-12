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
        new ValueTask<TModel?> GetAsync(TId id);

        ValueTask AddOrUpdateAsync(TModel model);

        ValueTask AddOrUpdateBatchAsync(IEnumerable<TModel> model);


        async ValueTask<IEntityModel<TId>?> IEntityStore<TId>.GetAsync(TId id)
            => await GetAsync(id);
    }

    public interface IEntityStore<TId> 
        where TId : IEquatable<TId>
    {
        ValueTask<IEntityModel<TId>?> GetAsync(TId id);

        IAsyncEnumerable<IEntityModel<TId>> GetAllAsync();

        ValueTask AddOrUpdateAsync(IEntityModel<TId> model);

        ValueTask AddOrUpdateBatchAsync(IEnumerable<IEntityModel<TId>> model);

        ValueTask RemoveAsync(TId id);

        ValueTask PurgeAllAsync();
    }
}
