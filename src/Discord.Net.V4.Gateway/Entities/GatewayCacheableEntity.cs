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
    IStoreProvider<TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TSelf, TModel>,
    IContextConstructable<TSelf, TModel, ICacheConstructionContext, DiscordGatewayClient>
{
    protected async ValueTask UpdateCacheAsync(TSelf self, TModel model, CancellationToken token)
    {
        var store = await self.GetStoreInfoAsync(token);
        var broker = await self.GetBrokerAsync(token);

        await broker.UpdateAsync(model, store, token);
    }

    public abstract TModel GetModel();

    public abstract ValueTask UpdateAsync(TModel model, bool updateCache = true, CancellationToken token = default);
}
