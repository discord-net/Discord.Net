using Discord.Gateway.State;
using Discord.Models;
using Discord.Paging;

namespace Discord.Gateway;

public class CachedPagedActor<TId, TEntity, TModel, TParams>(
    DiscordGatewayClient client,
    CachePathable path
) :
    IPagedActor<TId, TEntity, TParams>
    where TId : IEquatable<TId>
    where TEntity :
    class,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    ICacheableEntity<TEntity, TId, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
    where TParams : class, IPagingParams
{
    public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
        => new CachePager<TId, TEntity, TModel, TParams>(client, path, args);
}
