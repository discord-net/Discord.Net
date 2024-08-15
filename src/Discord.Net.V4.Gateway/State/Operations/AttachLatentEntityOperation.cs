using Discord.Gateway;
using Discord.Models;

namespace Discord.Gateway.State.Operations;

internal sealed record AttachLatentEntityOperation<TId, TEntity, TModel>(
    TEntity Entity,
    TModel Model,
    IDisposable Handle,
    IEntityBroker<TId, TEntity, TModel>? Broker = null
) :
    IAttachLatentEntityOperation
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public async ValueTask AttachAsync(StateController controller, CancellationToken token)
    {
        await (Broker ?? TEntity.GetBroker(Entity.Client))
            .AttachLatentEntityAsync(
                Model.Id,
                Entity,
                await TEntity.GetStoreInfoAsync(Entity.Client, Entity.CachePath, token),
                token
            );
    }

    public void Dispose()
    {
        Handle.Dispose();
    }
}

internal interface IAttachLatentEntityOperation : IStateOperation, IDisposable
{
    ValueTask AttachAsync(StateController controller, CancellationToken token);
}