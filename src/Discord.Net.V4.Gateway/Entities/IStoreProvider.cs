using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[NoExposure]
public interface IStoreInfoProvider<TId, TModel> : IStoreProvider<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    internal static abstract ValueTask<IStoreInfo<TId, TModel>> GetStoreInfoAsync(
        DiscordGatewayClient client,
        IPathable path,
        CancellationToken token = default
    );

    internal ValueTask<IStoreInfo<TId, TModel>> GetStoreInfoAsync(CancellationToken token = default);
}

[NoExposure]
public interface IStoreProvider<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    internal static abstract ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync(
        DiscordGatewayClient client,
        IPathable path,
        CancellationToken token = default
    );

    internal ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync(CancellationToken token = default);

    internal ValueTask<IEntityModelStore<TId, TSubModel>?> GetStoreForModelAsync<TSubModel>(
        CancellationToken token = default)
        where TSubModel : class, TModel, IEntityModel<TId>;

    internal ValueTask<IEntityModelStore<TId, TModel>?> GetStoreForModelAsync(
        Type modelType,
        CancellationToken token = default
    );
}

[NoExposure]
public interface IRootStoreProvider<TId, TModel> : IStoreProvider<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    internal static abstract IReadOnlyDictionary<Type, StoreProviderInfo<TId, TModel>> GetStoreHierarchy(
        DiscordGatewayClient client
    );
}

[NoExposure]
public interface ISubStoreProvider<TParentId, TParentModel, TId, TModel> : IStoreProvider<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
    where TParentModel : class, IEntityModel<TParentId>
    where TParentId : IEquatable<TParentId>
{
    internal static abstract IReadOnlyDictionary<Type, StoreProviderInfo<TId, TModel>> GetStoreHierarchy(
        DiscordGatewayClient client,
        IEntityModelStore<TParentId, TParentModel> parentStore,
        TParentId parentId
    );
}

internal readonly struct StoreProviderInfo<TId, TModel>(Type storeType, GetStoreDelegate<TId, TModel> storeDelegate)
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    public readonly Type StoreType = storeType;
    public readonly GetStoreDelegate<TId, TModel> StoreDelegate = storeDelegate;
}

internal delegate ValueTask<IEntityModelStore<TId, TModel>> GetStoreDelegate<TId, TModel>(
    DiscordGatewayClient client,
    CancellationToken token = default
)
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>;
