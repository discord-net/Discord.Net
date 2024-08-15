using Discord.Models;

namespace Discord.Gateway.State;

file sealed class BrokerInfo<TId, TEntity, TActor, TModel>(
    IEntityBroker<TId, TEntity, TActor, TModel> broker,
    IReadOnlyDictionary<Type, IManageableEntityBroker<TId, TEntity, TModel>> hierarchy
) :
    IBrokerInfo<TId, TEntity, TActor, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    public IEntityBroker<TId, TEntity, TActor, TModel> Broker { get; } = broker;

    public IReadOnlyDictionary<Type, IManageableEntityBroker<TId, TEntity, TModel>> BrokerHierarchy { get; } =
        hierarchy;
}

internal interface IBrokerInfo<TId, TEntity, in TActor, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    bool HasHierarchicBrokers => BrokerHierarchy.Count > 0;

    IEntityBroker<TId, TEntity, TActor, TModel> Broker { get; }

    IReadOnlyDictionary<Type, IManageableEntityBroker<TId, TEntity, TModel>> BrokerHierarchy { get; }

    static IBrokerInfo<TId, TEntity, TActor, TModel> Create(
        IEntityBroker<TId, TEntity, TActor, TModel> broker,
        IReadOnlyDictionary<Type, IManageableEntityBroker<TId, TEntity, TModel>> hierarchy
    ) => new BrokerInfo<TId, TEntity, TActor, TModel>(broker, hierarchy);
}