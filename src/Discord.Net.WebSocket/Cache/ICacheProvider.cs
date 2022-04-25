using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ICacheProvider
    {
        Type GetModel<TModelInterface>();

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
        ValueTask<TModel> GetAsync(TId id);
        TModel Get(TId id);
        IAsyncEnumerable<TModel> GetAllAsync();
        IEnumerable<TModel> GetAll();
        ValueTask AddOrUpdateAsync(TModel model);
        void AddOrUpdate(TModel model);
        ValueTask AddOrUpdateBatchAsync(IEnumerable<TModel> models);
        void AddOrUpdateBatch(IEnumerable<TModel> models);
        ValueTask RemoveAsync(TId id);
        void Remove(TId id);
        ValueTask PurgeAllAsync();
        void PurgeAll();
    }
}
