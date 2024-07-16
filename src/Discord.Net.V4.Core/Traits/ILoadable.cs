using Discord.Models;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface)]
internal sealed class LoadableAttribute(string route, params Type[] generics) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.

public interface ILoadable<TSelf, TId, TEntity, TModel> :
    IFetchable<TId, TModel>,
    IEntityProvider<TEntity, TModel>,
    IIdentifiable<TId>,
    IPathable
    where TSelf : ILoadable<TSelf, TId, TEntity, TModel>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    ValueTask<TEntity?> FetchAsync(RequestOptions? options = null, CancellationToken token = default)
        => FetchInternalAsync(Client, this, TSelf.FetchRoute(this, Id), options, token);

    internal static async ValueTask<TEntity?> FetchInternalAsync(
        IDiscordClient client,
        IEntityProvider<TEntity, TModel> entityProvider,
        IApiOutRoute<TModel> route,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await client.RestApiClient.ExecuteAsync(
            route,
            options ?? client.DefaultRequestOptions,
            token
        );

        return entityProvider.CreateNullableEntity(model);
    }
}
