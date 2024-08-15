using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

public abstract class GatewayCacheableEntity<TSelf, TId, TModel>(
    DiscordGatewayClient client,
    TId id
) :
    GatewayEntity<TId>(client, id),
    ICacheableEntity<TSelf, TId, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TSelf :
    GatewayCacheableEntity<TSelf, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TSelf, TModel>,
    IContextConstructable<TSelf, TModel, IGatewayConstructionContext, DiscordGatewayClient>
{
    protected async ValueTask UpdateCacheAsync(TSelf self, TModel model, CancellationToken token)
    {
        await TSelf.GetBroker(Client)
            .UpdateAsync(
                model,
                await TSelf.GetStoreInfoAsync(Client, CachePath, token),
                token
            );
    }

    public abstract TModel GetModel();

    public abstract ValueTask UpdateAsync(TModel model, bool updateCache = true, CancellationToken token = default);

    internal abstract CachePathable CachePath { get; }

    CachePathable ICacheableEntity<TSelf, TId, TModel>.CachePath => CachePath;
}