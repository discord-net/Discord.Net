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

    public IAsyncEnumerable<IEnumerable<TExpectedModel>> GetAllAsync(CancellationToken token = default)
        => store.GetAllAsync(token).Cast<IEnumerable<TExpectedModel>>();

    public IAsyncEnumerable<IEnumerable<TId>> GetAllIdsAsync(CancellationToken token = default)
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

    public IAsyncEnumerable<IEnumerable<TExpectedModel>> QueryAsync(
        TId from,
        Optional<TId> to,
        Direction direction,
        int? limit = null,
        CancellationToken token = default
    ) => store.QueryAsync(from, to, direction, limit, token).Cast<IEnumerable<TExpectedModel>>();

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

    public IAsyncEnumerable<IEnumerable<TExpectedModel>> GetAllAsync(CancellationToken token = default)
        => store.GetAllAsync(token).Cast<IEnumerable<TExpectedModel>>();

    public IAsyncEnumerable<IEnumerable<TId>> GetAllIdsAsync(CancellationToken token = default)
        => store.GetAllIdsAsync(token);

    public ValueTask AddOrUpdateAsync(TExpectedModel model, CancellationToken token = default)
        => store.AddOrUpdateAsync(model, token);

    public ValueTask AddOrUpdateBatchAsync(IEnumerable<TExpectedModel> models, CancellationToken token = default)
        => store.AddOrUpdateBatchAsync(models, token);

    public ValueTask RemoveAsync(TId id, CancellationToken token = default)
        => store.RemoveAsync(id, token);

    public ValueTask PurgeAllAsync(CancellationToken token = default)
        => store.PurgeAllAsync(token);

    public IAsyncEnumerable<IEnumerable<TExpectedModel>> QueryAsync(
        TId from,
        Optional<TId> to,
        Direction direction,
        int? limit = null,
        CancellationToken token = default
    ) => store.QueryAsync(from, to, direction, limit, token).Cast<IEnumerable<TExpectedModel>>();

    Type IEntityModelStore.ModelType => store.ModelType;
}
