using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface ICacheProvider
    {
        ValueTask<IEntityStore<TId>> GetStoreAsync<TId>(StoreType type)
            where TId : IEquatable<TId>;

        ValueTask<IEntityStore<TId>> GetSubStoreAsync<TId>(StoreType type, TId parentId)
           where TId : IEquatable<TId>;
    }
}
