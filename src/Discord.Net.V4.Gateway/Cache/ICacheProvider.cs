using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.Cache;

public interface ICacheProvider
{
    ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync<TId, TModel>(CancellationToken token = default)
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>;
}
