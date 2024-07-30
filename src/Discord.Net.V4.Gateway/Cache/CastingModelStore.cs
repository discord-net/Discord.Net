using Discord.Models;

namespace Discord.Gateway.Cache;

internal sealed class CastingModelStore<TId, TRootModel, TExpectedModel>(IEntityModelStore<TId, TRootModel> store)
    : IEntityModelStore<TId, TExpectedModel>
    where TRootModel : class, IEntityModel<TId>
    where TExpectedModel : class, TRootModel
    where TId : IEquatable<TId>
{
    public ValueTask<IEntityModelStore<TSubStoreId, TSubStoreModel>>
        GetSubStoreAsync<TSubStoreId, TSubStoreModel>(TId id, CancellationToken token = default)
        where TSubStoreId : IEquatable<TSubStoreId> where TSubStoreModel : class, IEntityModel<TSubStoreId>
        => store.GetSubStoreAsync<TSubStoreId, TSubStoreModel>(id, token);

    public ValueTask<TExpectedModel?> GetAsync(TId id, CancellationToken token = default)
        => store.GetAsync(id, token).CastDownAsync(Template.Of<TExpectedModel>());

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
}
