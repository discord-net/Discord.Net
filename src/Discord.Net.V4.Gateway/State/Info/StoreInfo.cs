using Discord.Models;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Discord.Gateway.State;

file sealed class StoreInfo<TId, TModel>(
    IEntityModelStore<TId, TModel> store,
    IReadOnlyDictionary<Type, IEntityModelStore<TId, TModel>> hierarchyStoreMap
) :
    IStoreInfo<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    public IEntityModelStore<TId, TModel> Store { get; } = store;

    public IReadOnlyDictionary<Type, IEntityModelStore<TId, TModel>> HierarchyStoreMap { get; } = hierarchyStoreMap;

    public IReadOnlyCollection<IEntityModelStore<TId, TModel>> AllStores { get; } = [store, ..hierarchyStoreMap.Values];

    // we cant store our results in 'ModelMap' since the 'HierarchyStoreMap' is based on user configuration.
    private readonly Dictionary<Type, IEntityModelStore<TId, TModel>> _fastLookup = new();

    public IEntityModelStore<TId, TModel> GetStoreForModel(TModel model)
    {
        if (HierarchyStoreMap.Count == 0) return Store;
        
        var type = model.GetType();
        
        if (model is IExtendedModel<TModel> extendedModel)
            type = extendedModel.ExtendedType;

        return GetStoreForModel(type);
    }
    
    public IEntityModelStore<TId, TModel> GetStoreForModel(Type model)
    {
        if (HierarchyStoreMap.Count == 0) return Store;

        IEntityModelStore<TId, TModel>? store;
        
        if (ModelMap.TryGet(model, out var mappingType))
        {
            if (mappingType == typeof(TModel) || !HierarchyStoreMap.TryGetValue(mappingType, out store))
                return Store;

            return store;
        }
        
        lock(this)
            if (_fastLookup.TryGetValue(model, out store))
                return store;
        
        foreach (var entry in HierarchyStoreMap)
        {
            if (!model.IsAssignableTo(entry.Key)) continue;
            
            lock(this) _fastLookup[model] = entry.Value;
            return entry.Value;
        }

        lock (this) _fastLookup[model] = Store;
        return Store;
    }
}

internal interface IStoreInfo<TId, TModel> : IStoreInfo
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    bool HasHierarchicStores => HierarchyStoreMap.Count > 0;

    IEntityModelStore<TId, TModel> Store { get; }

    IReadOnlyDictionary<Type, IEntityModelStore<TId, TModel>> HierarchyStoreMap { get; }
    
    IReadOnlyCollection<IEntityModelStore<TId, TModel>> AllStores { get; }

    IEntityModelStore<TId, TModel> GetStoreForModel(TModel model);
    IEntityModelStore<TId, TModel> GetStoreForModel(Type model);
    
    public static IStoreInfo<TId, TModel> Create(
        IEntityModelStore<TId, TModel> store,
        IReadOnlyDictionary<Type, IEntityModelStore<TId, TModel>> hierarchyStoreMap
    ) => new StoreInfo<TId, TModel>(store, hierarchyStoreMap);

    Type IStoreInfo.IdType => typeof(TId);
    Type IStoreInfo.ModelType => typeof(TModel);
}

internal interface IStoreInfo
{
    Type IdType { get; }
    Type ModelType { get; }
}