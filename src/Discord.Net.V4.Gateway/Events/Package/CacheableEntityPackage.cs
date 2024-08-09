using Discord.Models;

namespace Discord.Gateway;

public class CacheableEntityPackage<TId, TEntity, TActor, TModel>(TId id) :
    EntityPackage<TId, TEntity, TActor, TModel>(id)
    where TId : IEquatable<TId>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TEntity :
    GatewayEntity<TId>,
    ICacheableEntity<TEntity, TId, TModel>,
    IEntityOf<TModel>, IProxied<TActor>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
{
    public IEntityHandle<TId, TEntity>? Handle { get; init;}
}
