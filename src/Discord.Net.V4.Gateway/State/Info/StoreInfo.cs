using Discord.Models;
using System.Collections.Immutable;
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
}

internal interface IStoreInfo<TId, TModel> : IStoreInfo
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    bool HasHierarchicStores => HierarchyStoreMap.Count > 0;

    IEntityModelStore<TId, TModel> Store { get; }

    IReadOnlyDictionary<Type, IEntityModelStore<TId, TModel>> HierarchyStoreMap { get; }
    
    IReadOnlyCollection<IEntityModelStore<TId, TModel>> AllStores { get; }

    IEntityModelStore<TId, TModel> GetStoreForModelType(Type type)
    {
        if (HierarchyStoreMap.TryGetValue(type, out var store))
            return store;

        if (!type.IsAssignableTo(typeof(TModel)))
            throw new ArgumentException(
                $"Expected a model type that is assignable to {typeof(TModel)}, but got {type}"
            );

        return Store;
    }

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