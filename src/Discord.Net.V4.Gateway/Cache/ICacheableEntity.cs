using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.Cache;

public interface ICacheableEntity<out TSelf, out TId, TModel> :
    ICacheableEntity<TId>,
    IEntityOf<TModel>,
    IUpdatable<TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
    where TSelf : ICacheableEntity<TSelf, TId, TModel>,
    IContextConstructable<TSelf, TModel, IPathable, DiscordGatewayClient>
{
    ValueTask UpdateAsync(TModel model, bool updateCache = true, CancellationToken token = default);

    ValueTask IUpdatable<TModel>.UpdateAsync(TModel model, CancellationToken token) => UpdateAsync(model, token: token);
}

public interface ICacheableEntity<out TId> : IEntity<TId>, IDisposable, IAsyncDisposable
    where TId : IEquatable<TId>;
