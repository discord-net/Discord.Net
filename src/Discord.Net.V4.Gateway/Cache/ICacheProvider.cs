using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.Cache
{
    public interface ICacheProvider
    {
        ValueTask<IEntityStore<TId>> GetStoreAsync<TId>(StoreType type, CancellationToken token = default)
            where TId : IEquatable<TId>;

        ValueTask<IEntityStore<TId>> GetSubStoreAsync<TId>(StoreType type, TId parentId, CancellationToken token = default)
           where TId : IEquatable<TId>;
    }
}
