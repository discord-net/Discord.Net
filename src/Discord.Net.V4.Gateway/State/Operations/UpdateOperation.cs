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
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public async ValueTask UpdateAsync(CancellationToken token)
    {
        await Broker.UpdateAsync(
            Model,
            await TEntity.GetStoreInfoAsync(Entity.Client, Entity.CachePath, token),
            token
        );
    }
}

internal interface IUpdateOperation : IStateOperation
{
    ValueTask UpdateAsync(CancellationToken token);
}