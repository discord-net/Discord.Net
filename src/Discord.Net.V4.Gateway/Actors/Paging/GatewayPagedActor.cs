using Discord.Gateway.State;
using Discord.Models;
using Discord.Paging;
using Discord.Rest;

namespace Discord.Gateway;

public sealed class GatewayPartialPagedActor<
    TId,
    TGatewayEntity,
    TPartialRestEntity,
    TModel,
    TPartialModel,
    TApiModel,
    TParams
>:
    IPagedActor<TId, TGatewayEntity, TParams>
    where TId : IEquatable<TId>
    where TGatewayEntity :
    class,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TGatewayEntity, TModel>,
    ICacheableEntity<TGatewayEntity, TId, TModel>,
    IContextConstructable<TGatewayEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TPartialRestEntity : RestEntity<TId>, IEntityOf<TPartialModel>
    where TModel : class, TPartialModel
    where TPartialModel : class, IEntityModel<TId>
    where TApiModel : class
    where TParams : class, IPagingParams<TParams, TApiModel>
{
    private readonly DiscordGatewayClient _client;
    private readonly CachePathable _path;
    private readonly Func<TApiModel, IEnumerable<TPartialModel>> _modelMapper;
    private readonly Func<TPartialModel, TApiModel, TPartialRestEntity> _restEntityFactory;

    public GatewayPartialPagedActor(
        DiscordGatewayClient client,
        CachePathable path,
        Func<TApiModel, IEnumerable<TPartialModel>> modelMapper,
        Func<TPartialModel, TApiModel, TPartialRestEntity> restEntityFactory)
    {
        _client = client;
        _path = path;
        _modelMapper = modelMapper;
        _restEntityFactory = restEntityFactory;
    }

    public IAsyncPaged<TPartialRestEntity> PageRestAsync(TParams? args = default, GatewayRequestOptions? options = null)
        => new RestPager<TId, TPartialRestEntity, TPartialModel, TApiModel, TParams>(
            _client.Rest,
            _path,
            options ?? _client.DefaultRequestOptions,
            _modelMapper,
            _restEntityFactory,
            args
        );

    public IAsyncPaged<TGatewayEntity> PageCacheAsync(TParams? args = default)
        => new CachePager<TId, TGatewayEntity, TModel, TParams>(
            _client,
            _path,
            args
        );

    IAsyncPaged<TGatewayEntity> IPagedActor<TId, TGatewayEntity, TParams>.PagedAsync(TParams? args,
        RequestOptions? options)
        => PageCacheAsync(args);
}


public sealed class GatewayPagedActor<TId, TGatewayEntity, TRestEntity, TCore, TModel, TApiModel, TParams> :
    IPagedActor<TId, TCore, TParams>
    where TId : IEquatable<TId>
    where TGatewayEntity :
    class,
    TCore,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TGatewayEntity, TModel>,
    ICacheableEntity<TGatewayEntity, TId, TModel>,
    IContextConstructable<TGatewayEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TRestEntity :
    RestEntity<TId>,
    TCore
    where TCore : class, IEntity<TId>, IEntityOf<TModel>
    where TModel : class, IEntityModel<TId>
    where TApiModel : class
    where TParams : class, IPagingParams<TParams, TApiModel>
{
    private IEntityBroker<TId, TGatewayEntity, TModel>? _broker;
    private IStoreInfo<TId, TModel>? _storeInfo;

    private readonly DiscordGatewayClient _client;
    private readonly CachePathable _path;
    private readonly Func<TApiModel, IEnumerable<TModel>> _modelMapper;
    private readonly Func<TModel, TApiModel, TRestEntity> _restEntityFactory;
    private readonly Func<TApiModel, RequestOptions, CancellationToken, ValueTask>? _onRestPage;

    public GatewayPagedActor(
        DiscordGatewayClient client,
        CachePathable path,
        Func<TApiModel, IEnumerable<TModel>> modelMapper,
        Func<TModel, TApiModel, TRestEntity> restEntityFactory,
        Func<TApiModel, RequestOptions, CancellationToken, ValueTask>? onRestPage = null)
    {
        _client = client;
        _path = path;
        _modelMapper = modelMapper;
        _restEntityFactory = restEntityFactory;
        _onRestPage = onRestPage;
    }

    public IAsyncPaged<TRestEntity> PageRestAsync(TParams? args = default, GatewayRequestOptions? options = null)
        => new RestPager<TId, TRestEntity, TModel, TApiModel, TParams>(
            _client.Rest,
            _path,
            options ?? _client.DefaultRequestOptions,
            _modelMapper,
            _restEntityFactory,
            args,
            options?.UpdateCache ?? false
                ? AddModelsToCacheAsync
                : null,
            _onRestPage
        );

    private async ValueTask AddModelsToCacheAsync(IEnumerable<TModel> models, CancellationToken token)
    {
        _broker ??= await TGatewayEntity.GetBrokerAsync(_client, token);
        _storeInfo ??= await TGatewayEntity.GetStoreInfoAsync(_client, _path, token);

        await _broker.BatchUpdateAsync(models, _storeInfo, token);
    }

    public IAsyncPaged<TGatewayEntity> PageCacheAsync(TParams? args = default)
        => new CachePager<TId, TGatewayEntity, TModel, TParams>(
            _client,
            _path,
            args
        );

    public IAsyncPaged<TCore> PagedAsync(TParams? args = default, GatewayRequestOptions? options = null)
    {
        if (options?.AllowCached ?? false)
            return PageCacheAsync(args);

        return PageRestAsync(args, options);
    }

    IAsyncPaged<TCore> IPagedActor<TId, TCore, TParams>.PagedAsync(TParams? args, RequestOptions? options)
        => PagedAsync(args, GatewayRequestOptions.FromRestOptions(options));
}
