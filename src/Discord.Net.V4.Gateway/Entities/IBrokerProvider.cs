using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[NoExposure]
public interface IBrokerInfoProvider<TId, TEntity, in TActor, TModel> : 
    IBrokerProvider<TId, TEntity, TActor, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
{
    internal static abstract IBrokerInfo<TId, TEntity, TActor, TModel> GetBrokerInfo(
        DiscordGatewayClient client
    );
}

[NoExposure]
public interface IBrokerProvider<TId, TEntity, in TActor, TModel> : 
    IBrokerProvider<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TActor : class, IGatewayCachedActor<TId, TEntity, TModel>
{
    internal new static abstract IEntityBroker<TId, TEntity, TActor, TModel> GetBroker(DiscordGatewayClient client);
}

[NoExposure]
public interface IBrokerProvider<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    internal static abstract IEntityBroker<TId, TEntity, TModel> GetBroker(
        DiscordGatewayClient client
    );
    
    internal static abstract IReadOnlyDictionary<
        Type,
        IManageableEntityBroker<TId, TEntity, TModel>
    > GetBrokerHierarchy(DiscordGatewayClient client);
}
