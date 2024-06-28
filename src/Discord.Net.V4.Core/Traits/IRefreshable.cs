using Discord.Models;

namespace Discord;

public interface IRefreshable<in TSelf, TId, TModel> :
    IUpdatable<TModel>,
    IEntity<TId>,
    IPathable
    where TModel : class, IEntityModel<TId>
    where TSelf : IRefreshable<TSelf, TId, TModel>
    where TId : IEquatable<TId>
{
    async Task RefreshAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            TSelf.RefreshRoute((TSelf)this, Id),
            options ?? Client.DefaultRequestOptions,
            token
        );

        await UpdateAsync(model, token);
    }

    static abstract IApiOutRoute<TModel> RefreshRoute(TSelf self, TId id);
}
