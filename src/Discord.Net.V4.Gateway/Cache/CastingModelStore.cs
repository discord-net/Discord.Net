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
    public ValueTask<IEntityModelStore<TSubStoreId, TSubStoreModel>>
        GetSubStoreAsync<TSubStoreId, TSubStoreModel>(TId id, CancellationToken token = default)
        where TSubStoreId : IEquatable<TSubStoreId> where TSubStoreModel : class, IEntityModel<TSubStoreId>
        => store.GetSubStoreAsync<TSubStoreId, TSubStoreModel>(id, token);

    public ValueTask<TExpectedModel?> GetAsync(TId id, CancellationToken token = default)
        => store.GetAsync(id, token).CastUpAsync(Template.T<TExpectedModel>());

    public IAsyncEnumerable<TExpectedModel> GetAllAsync(CancellationToken token = default)
        => store.GetAllAsync(token).Cast<TExpectedModel>();

    public IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default)
        => store.GetAllIdsAsync(token);

    public ValueTask AddOrUpdateAsync(TExpectedModel model, CancellationToken token = default)
    {
        if (model is not TRootModel rootModel)
            throw new InvalidCastException($"Expected {typeof(TRootModel)}, got {typeof(TExpectedModel)}");

        return store.AddOrUpdateAsync(rootModel, token);
    }

    public ValueTask AddOrUpdateBatchAsync(IEnumerable<TExpectedModel> models, CancellationToken token = default)
        => store.AddOrUpdateBatchAsync(models.Cast<TRootModel>(), token);

    public ValueTask RemoveAsync(TId id, CancellationToken token = default)
        => store.RemoveAsync(id, token);

    public ValueTask PurgeAllAsync(CancellationToken token = default)
        => store.PurgeAllAsync(token);

    public IAsyncEnumerable<TExpectedModel> QueryAsync(TId from, Direction direction, int limit)
        => store.QueryAsync(from, direction, limit).Cast<TExpectedModel>();

    Type IEntityModelStore.ModelType => store.ModelType;
}

internal sealed class CastDownModelStore<TId, TRootModel, TExpectedModel>(
    IEntityModelStore<TId, TRootModel> store
) :
    IEntityModelStore<TId, TExpectedModel>
    where TRootModel : class, IEntityModel<TId>
    where TExpectedModel : class, TRootModel
    where TId : IEquatable<TId>
{
    public ValueTask<IEntityModelStore<TSubStoreId, TSubStoreModel>>
        GetSubStoreAsync<TSubStoreId, TSubStoreModel>(TId id, CancellationToken token = default)
        where TSubStoreId : IEquatable<TSubStoreId> where TSubStoreModel : class, IEntityModel<TSubStoreId>
        => store.GetSubStoreAsync<TSubStoreId, TSubStoreModel>(id, token);

    public ValueTask<TExpectedModel?> GetAsync(TId id, CancellationToken token = default)
        => store.GetAsync(id, token).CastDownAsync(Template.T<TExpectedModel>());

    public IAsyncEnumerable<TExpectedModel> GetAllAsync(CancellationToken token = default)
        => store.GetAllAsync(token).Cast<TExpectedModel>();

    public IAsyncEnumerable<TId> GetAllIdsAsync(CancellationToken token = default)
        => store.GetAllIdsAsync(token);

    public ValueTask AddOrUpdateAsync(TExpectedModel model, CancellationToken token = default)
        => store.AddOrUpdateAsync(model, token);

    public ValueTask AddOrUpdateBatchAsync(IEnumerable<TExpectedModel> models, CancellationToken token = default)
        => store.AddOrUpdateBatchAsync(models, token);

    public ValueTask RemoveAsync(TId id, CancellationToken token = default)
        => store.RemoveAsync(id, token);

    public ValueTask PurgeAllAsync(CancellationToken token = default)
        => store.PurgeAllAsync(token);

    public IAsyncEnumerable<TExpectedModel> QueryAsync(TId from, Direction direction, int limit)
        => store.QueryAsync(from, direction, limit).Cast<TExpectedModel>();

    Type IEntityModelStore.ModelType => store.ModelType;
}
