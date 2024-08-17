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
    IUpdatable<TModel>,
    ICacheUpdatable<TId, TModel>,
    IGatewayClientProvider
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TSelf : class,
    ICacheUpdatable<TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TSelf, TModel>,
    ICacheableEntity<TSelf, TId, TModel>,
    IContextConstructable<TSelf, TModel, IGatewayConstructionContext, DiscordGatewayClient>
{
    internal CachePathable CachePath { get; }
    
    ValueTask IUpdatable<TModel>.UpdateAsync(TModel model, CancellationToken token) => UpdateAsync(model, true, token: token);
}

public interface ICacheUpdatable<out TId, in TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    ValueTask UpdateAsync(TModel model, bool updateCache = true, CancellationToken token = default);
}

public interface ICacheableEntity<out TId> : IEntity<TId>, ICacheableEntity
    where TId : IEquatable<TId>;

public interface ICacheableEntity;
