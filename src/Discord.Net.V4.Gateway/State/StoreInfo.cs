using Discord.Models;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Discord.Gateway.State;

file sealed class SubStoreInfo<TSubProvider, TParentId, TParentModel, TId, TModel>(
    DiscordGatewayClient client,
    IEntityModelStore<TId, TModel> store,
    IEntityModelStore<TParentId, TParentModel> parentStore,
    TParentId parentId
) :
    BaseStoreInfo<TId, TModel>(client, store)
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
    where TParentModel : class, IEntityModel<TParentId>
    where TParentId : IEquatable<TParentId>
    where TSubProvider : ISubStoreProvider<TParentId, TParentModel, TId, TModel>
{
    protected override IReadOnlyDictionary<Type, StoreProviderInfo<TId, TModel>> GetStoreMap()
        => TSubProvider.GetStoreHierarchy(Client, parentStore, parentId);
}

file sealed class RootStoreInfo<TRootProvider, TId, TModel>(
    DiscordGatewayClient client,
    IEntityModelStore<TId, TModel> store
) :
    BaseStoreInfo<TId, TModel>(client, store)
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
    where TRootProvider : IRootStoreProvider<TId, TModel>
{
    protected override IReadOnlyDictionary<Type, StoreProviderInfo<TId, TModel>> GetStoreMap()
        => TRootProvider.GetStoreHierarchy(Client);
}

file abstract class BaseStoreInfo<TId, TModel>(
    DiscordGatewayClient client,
    IEntityModelStore<TId, TModel> store
) :
    IStoreInfo<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    public IEntityModelStore<TId, TModel> Store { get; } = store;

    public IReadOnlyDictionary<Type, StoreProviderInfo<TId, TModel>> HierarchyStoreMap
        => _hierarchyStoreMap ??= GetStoreMap();

    public IReadOnlyList<StoreProviderInfo<TId, TModel>> EnabledStoresForHierarchy
        => _enabledHierarchyStores ??= HierarchyStoreMap.Values
            .DistinctBy(x => x.StoreType)
            .ToImmutableList();

    protected DiscordGatewayClient Client { get; } = client;

    private IReadOnlyDictionary<Type, StoreProviderInfo<TId, TModel>>? _hierarchyStoreMap;
    private IReadOnlyList<StoreProviderInfo<TId, TModel>>? _enabledHierarchyStores;
    private readonly Dictionary<Type, IEntityModelStore<TId, TModel>> _computedStores = [];
    private readonly KeyedSemaphoreSlim<Type> _keyedSemaphore = new(1, 1);

    public async ValueTask<IEntityModelStore<TId, TModel>> GetOrComputeStoreAsync(
        StoreProviderInfo<TId, TModel> store,
        CancellationToken token = default
    )
    {
        using var scope = _keyedSemaphore.Get(store.StoreType, out var semaphoreSlim);

        await semaphoreSlim.WaitAsync(token);

        try
        {
            if (_computedStores.TryGetValue(store.StoreType, out var computedStore))
                return computedStore;

            return _computedStores[store.StoreType] = await store.StoreDelegate(Client, token);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    protected abstract IReadOnlyDictionary<Type, StoreProviderInfo<TId, TModel>> GetStoreMap();
}

internal interface IStoreInfo<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    IEntityModelStore<TId, TModel> Store { get; }

    IReadOnlyDictionary<Type, StoreProviderInfo<TId, TModel>> HierarchyStoreMap { get; }
    IReadOnlyList<StoreProviderInfo<TId, TModel>> EnabledStoresForHierarchy { get; }

    ValueTask<IEntityModelStore<TId, TModel>> GetOrComputeStoreAsync(
        StoreProviderInfo<TId, TModel> store,
        CancellationToken token = default
    );

    async IAsyncEnumerable<IEntityModelStore<TId, TModel>> EnumerateAllAsync([EnumeratorCancellation] CancellationToken token = default)
    {
        yield return Store;

        foreach (var storeInfo in EnabledStoresForHierarchy)
            yield return await GetOrComputeStoreAsync(storeInfo, token);
    }

    async ValueTask<IEntityModelStore<TId, TModel>> GetStoreForModelType(Type type, CancellationToken token = default)
    {
        if (HierarchyStoreMap.TryGetValue(type, out var storeProviderInfo))
            return await GetOrComputeStoreAsync(storeProviderInfo, token);

        if (!type.IsAssignableTo(typeof(TModel)))
            throw new ArgumentException(
                $"Expected a model type that is assignable to {typeof(TModel)}, but got {type}"
            );

        return Store;
    }
}

internal static class StoreInfo
{
    public static IStoreInfo<TId, TModel> ToInfo<TId, TModel, TRootProvider>(
        this IEntityModelStore<TId, TModel> store,
        DiscordGatewayClient client,
        Template<TRootProvider> template)
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
        where TRootProvider : IRootStoreProvider<TId, TModel>
        => new RootStoreInfo<TRootProvider, TId, TModel>(client, store);

    public static IStoreInfo<TId, TModel> ToInfo<TParentId, TParentModel, TId, TModel, TSubProvider>(
        this IEntityModelStore<TId, TModel> store,
        DiscordGatewayClient client,
        IEntityModelStore<TParentId, TParentModel> parentStore,
        TParentId parentId,
        Template<TSubProvider> template)
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
        where TParentModel : class, IEntityModel<TParentId>
        where TParentId : IEquatable<TParentId>
        where TSubProvider : ISubStoreProvider<TParentId, TParentModel, TId, TModel>
        => new SubStoreInfo<TSubProvider, TParentId, TParentModel, TId, TModel>(client, store, parentStore, parentId);
}
