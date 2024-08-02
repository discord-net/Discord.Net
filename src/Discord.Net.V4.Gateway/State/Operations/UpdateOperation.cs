using Discord.Gateway;
using Discord.Models;

namespace Discord.Gateway.State.Operations;

internal sealed record UpdateOperation<TId, TEntity, TModel>(
    TEntity Entity,
    TModel Model,
    IEntityBroker<TId, TEntity, TModel> Broker
) :
    IUpdateOperation
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public async ValueTask UpdateAsync(CancellationToken token)
    {
        var store = await Entity.GetStoreInfoAsync(token);

        await Broker.UpdateAsync(Model, store, token);
    }
}

internal interface IUpdateOperation : IStateOperation
{
    ValueTask UpdateAsync(CancellationToken token);
}
