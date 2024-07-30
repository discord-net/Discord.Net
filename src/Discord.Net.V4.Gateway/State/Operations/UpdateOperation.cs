using Discord.Gateway.Cache;
using Discord.Models;

namespace Discord.Gateway.State.Operations;

internal sealed record UpdateOperation<TId, TEntity, TModel>(
    TEntity Entity,
    TModel Model,
    IRefCounted<IEntityBroker<TId, TEntity, TModel>> Broker
) :
    IUpdateOperation
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public async ValueTask UpdateAsync(CancellationToken token)
    {
        var store = await Entity.GetStoreAsync(token);

        await Broker.Value.UpdateAsync(Model, store, token);
    }

    public void Dispose() => Broker.Dispose();
}

internal interface IUpdateOperation : IStateOperation, IDisposable
{
    ValueTask UpdateAsync(CancellationToken token);
}
