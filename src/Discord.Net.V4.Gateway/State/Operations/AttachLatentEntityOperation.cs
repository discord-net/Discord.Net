using Discord.Gateway;
using Discord.Models;

namespace Discord.Gateway.State.Operations;

internal sealed record AttachLatentEntityOperation<TId, TEntity, TModel>(
    TEntity Entity,
    TModel Model,
    IDisposable Handle,
    IEntityBroker<TId, TEntity, TModel>? Broker = null
):
    IAttachLatentEntityOperation
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public async ValueTask AttachAsync(StateController controller, CancellationToken token)
    {
        var broker = Broker ?? await Entity.GetBrokerAsync(token);

        var store = await Entity.GetStoreInfoAsync(token);
        await broker.AttachLatentEntityAsync(Model.Id, Entity, store, token);
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
