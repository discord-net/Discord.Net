using System.Diagnostics.CodeAnalysis;
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

    public IManageableEntityBroker<TId, TEntity, TModel> GetBrokerForModel(TModel model)
    {
        if (BrokerHierarchy.Count == 0) return Broker;
        
        var type = model.GetType();
        
        if (model is IExtendedModel<TModel> extendedModel)
            type = extendedModel.ExtendedType;

        return GetBrokerForModel(type);
    }
    
    public IManageableEntityBroker<TId, TEntity, TModel> GetBrokerForModel(Type model)
    {
        if (BrokerHierarchy.Count == 0) return Broker;
        
        if (ModelMap.TryGet(model, out var brokerType))
        {
            if (brokerType == typeof(TModel))
                return Broker;

            if (BrokerHierarchy.TryGetValue(brokerType, out var broker))
                return broker;
        }
        
        foreach (var entry in BrokerHierarchy)
        {
            if (!model.IsAssignableTo(entry.Key)) continue;
            
            if(model != entry.Key) ModelMap.Set(model, entry.Key);
            return entry.Value;
        }

        if(model != typeof(TModel)) ModelMap.Set(model, typeof(TModel));
        return Broker;
    }
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

    IManageableEntityBroker<TId, TEntity, TModel> GetBrokerForModel(TModel model);
    IManageableEntityBroker<TId, TEntity, TModel> GetBrokerForModel(Type model);
    
    static IBrokerInfo<TId, TEntity, TActor, TModel> Create(
        IEntityBroker<TId, TEntity, TActor, TModel> broker,
        IReadOnlyDictionary<Type, IManageableEntityBroker<TId, TEntity, TModel>> hierarchy
    ) => new BrokerInfo<TId, TEntity, TActor, TModel>(broker, hierarchy);
}