using Discord.Models;

namespace Discord.Gateway.State;

file sealed class BrokerInfo<TProvider, TId, TEntity, TActor, TModel>(
    IEntityBroker<TId, TEntity, TActor, TModel> broker,
    DiscordGatewayClient client
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
    where TProvider : IBrokerProvider<TId, TEntity, TActor, TModel>
{
    public IEntityBroker<TId, TEntity, TActor, TModel> Broker { get; } = broker;

    public IReadOnlyDictionary<Type, IManageableEntityBroker<TId, TEntity, TModel>> BrokerHierarchy
        => _brokerHierarchy ??= TProvider.GetBrokerHierarchy(client);

    private IReadOnlyDictionary<Type, IManageableEntityBroker<TId, TEntity, TModel>>? _brokerHierarchy;
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
}

internal static class BrokerInfo
{
    public static IBrokerInfo<TId, TEntity, TActor, TModel> ToInfo<TProvider, TId, TEntity, TActor, TModel>(
        this IEntityBroker<TId, TEntity, TActor, TModel> broker,
        DiscordGatewayClient client
    )
        where TEntity :
        class,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
        where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TProvider : IBrokerProvider<TId, TEntity, TActor, TModel>
    {
        return new BrokerInfo<TProvider, TId, TEntity, TActor, TModel>(broker, client);
    }
}