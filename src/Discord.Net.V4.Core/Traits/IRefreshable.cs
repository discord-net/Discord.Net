using Discord.Models;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface)]
internal sealed class RefreshableAttribute(string route) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.

public interface IRefreshable<in TSelf, TId, TModel> :
    IFetchable<TId, TModel>,
    IUpdatable<TModel>,
    IPathable
    where TModel : class, IEntityModel<TId>
    where TSelf : IRefreshable<TSelf, TId, TModel>
    where TId : IEquatable<TId>
{
    Task RefreshAsync(RequestOptions? options = null, CancellationToken token = default)
        => RefreshInternalAsync((TSelf)this, TSelf.RefreshRoute(this, Id), options, token);

    internal static async Task RefreshInternalAsync(
        TSelf self,
        IApiOutRoute<TModel> route,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await self.Client.RestApiClient.ExecuteRequiredAsync(
            route,
            options ?? self.Client.DefaultRequestOptions,
            token
        );

        await self.UpdateAsync(model, token);
    }

    static abstract IApiOutRoute<TModel> RefreshRoute(IPathable path, TId id);

    static IApiOutRoute<TModel> IFetchable<TId, TModel>.FetchRoute(IPathable path, TId id)
        => TSelf.RefreshRoute(path, id);
}
