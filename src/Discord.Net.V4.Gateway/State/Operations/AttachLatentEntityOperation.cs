using Discord.Gateway.Cache;
using Discord.Models;

namespace Discord.Gateway.State.Operations;

internal sealed record AttachLatentEntityOperation<TId, TEntity, TModel>(
    TEntity Entity,
    TModel Model,
    IDisposable Handle,
    IRefCounted<IEntityBroker<TId, TEntity, TModel>>? Broker = null
):
    IAttachLatentEntityOperation
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public async ValueTask AttachAsync(StateController controller, CancellationToken token)
    {
        var broker = Broker ?? await controller.GetBrokerAsync<TId, TEntity, TModel>(token);

        var store = await Entity.GetStoreAsync(token);
        await broker.Value.AttachLatentEntityAsync(Model.Id, Entity, store, token);
    }

    public void Dispose()
    {
        Broker?.Dispose();
        Handle.Dispose();
    }
}

internal interface IAttachLatentEntityOperation : IStateOperation, IDisposable
{
    ValueTask AttachAsync(StateController controller, CancellationToken token);
}
