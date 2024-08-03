using Discord.Gateway.State;
using Discord.Models;
using Discord.Paging;

namespace Discord.Gateway.Paging;

public class CachedPagedActor<TId, TEntity, TModel, TParams> : IPagedActor<TId, TEntity, TParams>
    where TId : IEquatable<TId>
    where TEntity :
    class,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    ICacheableEntity<TEntity, TId, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
    where TParams : IPagingParams
{
    private readonly DiscordGatewayClient _client;
    private readonly CachePathable _path;

    public CachedPagedActor(
        DiscordGatewayClient client,
        CachePathable path)
    {
        _client = client;
        _path = path;
    }

    public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
        => new CachePager<TId, TEntity, TModel, TParams>(_client, _path, args);
}
