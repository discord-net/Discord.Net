using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Discord.Gateway;

public sealed class GatewayEnumerableIndexableLink<TActor, TId, TEntity, TRestEntity, TCore, TModel> :
    GatewayEnumerableIndexableLink<TActor, TId, TEntity, TRestEntity, TCore, TModel, IEnumerable<TModel>>
    where TActor :
    class,
    IGatewayCachedActor<TId, TEntity, TModel>,
    IActor<TId, TCore>
    where TId : IEquatable<TId>
    where TEntity :
    GatewayEntity<TId>,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TRestEntity : RestEntity<TId>
    where TCore : class, IEntity<TId, TModel>
    where TModel : class, IEntityModel<TId>
{
    public GatewayEnumerableIndexableLink(
        DiscordGatewayClient client,
        Func<TId, TActor> factory,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<IEnumerable<TModel>> apiRoute,
        Func<IEnumerable<TModel>, IEnumerable<TModel>>? transformer = null
    ) : base(
        client,
        factory,
        restFactory,
        path,
        apiRoute,
        transformer ?? (api => api)
    )
    {
    }

    public GatewayEnumerableIndexableLink(
        DiscordGatewayClient client,
        GatewayIndexableLink<TActor, TId, TEntity> indexableLink,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<IEnumerable<TModel>> apiRoute,
        Func<IEnumerable<TModel>, IEnumerable<TModel>>? transformer = null
    ) : base(
        client,
        indexableLink,
        restFactory,
        path,
        apiRoute,
        transformer ?? (api => api)
    )
    {
    }
}

public class GatewayEnumerableIndexableLink<TActor, TId, TEntity, TRestEntity, TCore, TModel, TApi> :
    IEnumerableIndexableLink<TActor, TId, TCore>
    where TActor :
    class,
    IGatewayCachedActor<TId, TEntity, TModel>,
    IActor<TId, TCore>
    where TId : IEquatable<TId>
    where TEntity :
    GatewayEntity<TId>,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>
    where TRestEntity : RestEntity<TId>
    where TCore : class, IEntity<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TApi : class
{
    public TActor this[TId id] => _indexableLink[id];

    public TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity] => _indexableLink[identity];

    private IEntityBroker<TId, TEntity, TModel>? _broker;
    private IStoreInfo<TId, TModel>? _storeInfo;

    private readonly DiscordGatewayClient _client;
    private readonly Func<TModel, TRestEntity> _restFactory;
    private readonly CachePathable _path;
    private readonly IApiOutRoute<TApi> _apiRoute;
    private readonly Func<TApi, IEnumerable<TModel>> _transformer;

    private readonly GatewayIndexableLink<TActor, TId, TEntity> _indexableLink;

    internal GatewayEnumerableIndexableLink(
        DiscordGatewayClient client,
        GatewayIndexableLink<TActor, TId, TEntity> indexableLink,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<TApi> apiRoute,
        Func<TApi, IEnumerable<TModel>> transformer)
    {
        _client = client;
        _restFactory = restFactory;
        _path = path;
        _apiRoute = apiRoute;
        _transformer = transformer;
        _indexableLink = indexableLink;
    }

    internal GatewayEnumerableIndexableLink(
        DiscordGatewayClient client,
        Func<TId, TActor> factory,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<TApi> apiRoute,
        Func<TApi, IEnumerable<TModel>> transformer
    ) : this(client, new GatewayIndexableLink<TActor, TId, TEntity>(factory), restFactory, path, apiRoute, transformer)
    {
    }

    private IEntityBroker<TId, TEntity, TModel> GetBroker()
        => _broker ??= TEntity.GetBroker(_client);

    private async ValueTask<IStoreInfo<TId, TModel>> GetStoreInfoAsync(CancellationToken token = default)
        => _storeInfo ??= await TEntity.GetStoreInfoAsync(_client, _path, token);

    public async IAsyncEnumerable<IEntityHandle<TId, TEntity>> GetAllHandlesAsync(
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var broker = GetBroker();
        var storeInfo = await GetStoreInfoAsync(token);

        await foreach (var handle in broker.GetAllAsync(_path, storeInfo, token))
            yield return handle;
    }

    public IAsyncEnumerable<TEntity> GetAllAsync(
        CancellationToken token = default
    ) => GetAllHandlesAsync(token).ConsumeAsReferenceAsync();

    public async Task<IReadOnlyCollection<TRestEntity>> FetchAllAsync(
        GatewayRequestOptions? options = null,
        CancellationToken token = default)
    {
        var apiResult = await _client.RestApiClient.ExecuteAsync(
            _apiRoute,
            options ?? _client.DefaultRequestOptions,
            token
        );

        if (apiResult is null)
            return [];

        var models = _transformer(apiResult).ToArray();

        if (models.Length == 0)
            return [];

        if (options?.UpdateCache ?? false)
        {
            var broker = GetBroker();
            var storeInfo = await GetStoreInfoAsync(token);
            
            await broker.BatchUpdateAsync(
                models,
                storeInfo,
                token
            );
        }

        return models.Select(_restFactory).ToImmutableList();
    }

    public async ValueTask<IReadOnlyCollection<TCore>> AllAsync(
        GatewayRequestOptions? options = null,
        CancellationToken token = default)
    {
        options ??= _client.DefaultRequestOptions;

        return options.AllowCached
            ? (await GetAllAsync(token).OfType<TCore>().ToListAsync(cancellationToken: token)).AsReadOnly()
            : (await FetchAllAsync(options, token)).OfType<TCore>().ToList().AsReadOnly();
    }

    ValueTask<IReadOnlyCollection<TCore>> IEnumerableLink<TId, TCore>.AllAsync(RequestOptions? options,
        CancellationToken token)
        => AllAsync(GatewayRequestOptions.FromRestOptions(options), token);

    public TActor Specifically(TId id) => _indexableLink.Specifically(id);
}