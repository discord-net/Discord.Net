using Discord.Gateway.Cache;
using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[NoExposure]
public interface IBrokerProvider<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    internal ValueTask<IEntityBroker<TId, TEntity, TModel>> GetBrokerAsync(CancellationToken token = default);
}
