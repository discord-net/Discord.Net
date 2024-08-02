using Discord.Gateway.State;
using Discord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

public interface ICacheableEntity<out TSelf, out TId, TModel> :
    ICacheableEntity<TId>,
    IEntityOf<TModel>,
    IUpdatable<TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TSelf : class,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TSelf, TModel>,
    ICacheableEntity<TSelf, TId, TModel>,
    IContextConstructable<TSelf, TModel, ICacheConstructionContext, DiscordGatewayClient>
{
    ValueTask UpdateAsync(TModel model, bool updateCache = true, CancellationToken token = default);

    ValueTask IUpdatable<TModel>.UpdateAsync(TModel model, CancellationToken token) => UpdateAsync(model, token: token);
}

public interface ICacheableEntity<out TId> : IEntity<TId>, ICacheableEntity
    where TId : IEquatable<TId>;

public interface ICacheableEntity;
