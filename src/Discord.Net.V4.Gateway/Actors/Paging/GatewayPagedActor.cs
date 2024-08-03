using Discord.Gateway.State;
using Discord.Models;
using Discord.Paging;
using Discord.Rest;

namespace Discord.Gateway.Paging;

public sealed class GatewayPagedActor<TId, TGatewayEntity, TRestEntity, TCore, TModel, TParams> :
    IPagedActor<TId, TCore, TParams>
    where TId : IEquatable<TId>
    where TGatewayEntity :
    class,
    TCore,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TGatewayEntity, TModel>,
    ICacheableEntity<TGatewayEntity, TId, TModel>,
    IContextConstructable<TGatewayEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TRestEntity :
    RestEntity<TId>,
    TCore
    where TCore : class, IEntity<TId>, IEntityOf<TModel>
    where TModel : class, IEntityModel<TId>
    where TParams : IPagingParams
{
    private readonly DiscordGatewayClient _client;

    private readonly CachedPagedActor<TId, TGatewayEntity, TModel, TParams> _cachePager;

    public GatewayPagedActor(
        DiscordGatewayClient client,
        CachePathable path,
        IApiOutRoute<IEnumerable<TModel>> apiRoute,
        Func<TModel, TRestEntity> restFactory)
    {
        _client = client;
        _cachePager = new(client, path);
    }

    public IAsyncPaged<TRestEntity> PageRestAsync(TParams? args = default, GatewayRequestOptions? options = null)
    {

    }

    public IAsyncPaged<TGatewayEntity> PageCacheAsync(TParams? args = default, GatewayRequestOptions? options = null)
        => _cachePager.PagedAsync(args, options);

    public IAsyncPaged<TCore> PagedAsync(TParams? args = default, GatewayRequestOptions? options = null)
    {
        if (options?.AllowCached ?? false)
            return PageCacheAsync(args, options);

        return PageRestAsync(args, options);
    }

    IAsyncPaged<TCore> IPagedActor<TId, TCore, TParams>.PagedAsync(TParams? args, RequestOptions? options)
        => PagedAsync(args, GatewayRequestOptions.FromRestOptions(options));
}
