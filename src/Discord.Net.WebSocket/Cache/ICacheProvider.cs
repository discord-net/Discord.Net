using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ICacheProvider
    {
        ValueTask<IEntityStore<TModel, TId>> GetStoreAsync<TModel, TId>()
            where TModel : IEntityModel<TId>
            where TId : IEquatable<TId>;

        ValueTask<IEntityStore<TModel, TId>> GetSubStoreAsync<TModel, TId>(TId parentId)
            where TModel : IEntityModel<TId>
            where TId : IEquatable<TId>;
    }

    public interface IEntityStore<TModel, TId>
        where TModel : IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        ValueTask<TModel> GetAsync(TId id, CacheRunMode runmode);
        IAsyncEnumerable<TModel> GetAllAsync(CacheRunMode runmode);
        ValueTask AddOrUpdateAsync(TModel model, CacheRunMode runmode);
        ValueTask AddOrUpdateBatchAsync(IEnumerable<TModel> models, CacheRunMode runmode);
        ValueTask RemoveAsync(TId id, CacheRunMode runmode);
        ValueTask PurgeAllAsync(CacheRunMode runmode);
    }
}
