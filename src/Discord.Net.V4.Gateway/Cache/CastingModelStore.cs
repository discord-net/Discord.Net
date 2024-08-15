using Discord.Models;

namespace Discord.Gateway;

internal sealed class CastUpModelStore<TId, TRootModel, TExpectedModel>(
    IEntityModelStore<TId, TRootModel> store
):
    IEntityModelStore<TId, TExpectedModel>
    where TRootModel : class, TExpectedModel, IEntityModel<TId>
    where TExpectedModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    public IEntityModelStore<TId, TRootModel> Store { get; } = store;
    
    public ValueTask<IEntityModelStore<TSubStoreId, TSubStoreModel>>
        GetSubStoreAsync<TSubStoreId, TSubStoreModel>(TId id, CancellationToken token = default)
        where TSubStoreId : IEquatable<TSubStoreId> where TSubStoreModel : class, IEntityModel<TSubStoreId>
        => Store.GetSubStoreAsync<TSubStoreId, TSubStoreModel>(id, token);

    public ValueTask<TExpectedModel?> GetAsync(TId id, CancellationToken token = default)
        => Store.GetAsync(id, token).CastUpAsync(Template.T<TExpectedModel>());

    public IAsyncEnumerable<TExpectedModel> GetManyAsync(IEnumerable<TId> ids, CancellationToken token = default)
        => Store.GetManyAsync(ids, token).Cast<TExpectedModel>();

    public IAsyncEnumerable<TExpectedModel> GetAllAsync(CancellationToken token = default)
        => Store.GetAllAsync(token).Cast<TExpectedModel>();

    public IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default)
        => Store.GetAllIdsAsync(token);

    public ValueTask AddOrUpdateAsync(TExpectedModel model, CancellationToken token = default)
    {
        if (model is not TRootModel rootModel)
            throw new InvalidCastException($"Expected {typeof(TRootModel)}, got {typeof(TExpectedModel)}");

        return Store.AddOrUpdateAsync(rootModel, token);
    }

    public ValueTask AddOrUpdateBatchAsync(IEnumerable<TExpectedModel> models, CancellationToken token = default)
        => Store.AddOrUpdateBatchAsync(models.Cast<TRootModel>(), token);

    public ValueTask RemoveAsync(TId id, CancellationToken token = default)
        => Store.RemoveAsync(id, token);

    public ValueTask PurgeAllAsync(CancellationToken token = default)
        => Store.PurgeAllAsync(token);

    public IAsyncEnumerable<TExpectedModel> QueryAsync(
        TId from,
        Optional<TId> to,
        Direction direction,
        int? limit = null,
        CancellationToken token = default
    ) => Store.QueryAsync(from, to, direction, limit, token).Cast<TExpectedModel>();

    Type IEntityModelStore.ModelType => Store.ModelType;
}

internal sealed class CastDownModelStore<TId, TRootModel, TExpectedModel>(
    IEntityModelStore<TId, TRootModel> store
) :
    IEntityModelStore<TId, TExpectedModel>
    where TRootModel : class, IEntityModel<TId>
    where TExpectedModel : class, TRootModel
    where TId : IEquatable<TId>
{
    public IEntityModelStore<TId, TRootModel> Store { get; } = store;
    
    public ValueTask<IEntityModelStore<TSubStoreId, TSubStoreModel>>
        GetSubStoreAsync<TSubStoreId, TSubStoreModel>(TId id, CancellationToken token = default)
        where TSubStoreId : IEquatable<TSubStoreId> where TSubStoreModel : class, IEntityModel<TSubStoreId>
        => Store.GetSubStoreAsync<TSubStoreId, TSubStoreModel>(id, token);

    public ValueTask<TExpectedModel?> GetAsync(TId id, CancellationToken token = default)
        => Store.GetAsync(id, token).CastDownAsync(Template.T<TExpectedModel>());

    public IAsyncEnumerable<TExpectedModel> GetManyAsync(IEnumerable<TId> ids, CancellationToken token = default)
        => Store.GetManyAsync(ids, token).Cast<TExpectedModel>();

    public IAsyncEnumerable<TExpectedModel> GetAllAsync(CancellationToken token = default)
        => Store.GetAllAsync(token).Cast<TExpectedModel>();

    public IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default)
        => Store.GetAllIdsAsync(token);

    public ValueTask AddOrUpdateAsync(TExpectedModel model, CancellationToken token = default)
        => Store.AddOrUpdateAsync(model, token);

    public ValueTask AddOrUpdateBatchAsync(IEnumerable<TExpectedModel> models, CancellationToken token = default)
        => Store.AddOrUpdateBatchAsync(models, token);

    public ValueTask RemoveAsync(TId id, CancellationToken token = default)
        => Store.RemoveAsync(id, token);

    public ValueTask PurgeAllAsync(CancellationToken token = default)
        => Store.PurgeAllAsync(token);

    public IAsyncEnumerable<TExpectedModel> QueryAsync(
        TId from,
        Optional<TId> to,
        Direction direction,
        int? limit = null,
        CancellationToken token = default
    ) => Store.QueryAsync(from, to, direction, limit, token).Cast<TExpectedModel>();

    Type IEntityModelStore.ModelType => Store.ModelType;
}
