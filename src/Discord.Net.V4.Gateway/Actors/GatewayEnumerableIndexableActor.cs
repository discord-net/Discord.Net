using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Discord.Gateway;

public sealed class GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCore, TModel> :
    GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCore, TModel, IEnumerable<TModel>>
    where TActor :
    class,
    IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>,
    IStoreProvider<TId, TModel>
    where TId : IEquatable<TId>
    where TEntity :
    GatewayEntity<TId>,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>,
    TCore
    where TRestEntity : RestEntity<TId>, TCore
    where TCore : class, IEntity<TId, TModel>
    where TModel : class, IEntityModel<TId>
{
    public GatewayEnumerableIndexableActor(
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

    public GatewayEnumerableIndexableActor(
        DiscordGatewayClient client,
        GatewayIndexableActor<TActor, TId, TEntity> indexableActor,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<IEnumerable<TModel>> apiRoute,
        Func<IEnumerable<TModel>, IEnumerable<TModel>>? transformer = null
        ) : base(
        client,
        indexableActor,
        restFactory,
        path,
        apiRoute,
        transformer ?? (api => api)
    )
    {
    }
}

public class GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCore, TModel, TApi> :
    IEnumerableIndexableActor<TActor, TId, TCore>
    where TActor :
    class,
    IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>,
    IStoreProvider<TId, TModel>
    where TId : IEquatable<TId>
    where TEntity :
    GatewayEntity<TId>,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>,
    TCore
    where TRestEntity : RestEntity<TId>, TCore
    where TCore : class, IEntity<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TApi : class
{
    public TActor this[TId id] => _indexableActor[id];

    public TActor this[IIdentifiable<TId, TEntity, TActor, TModel> identity] => _indexableActor[identity];

    private IEntityBroker<TId, TEntity, TModel>? _broker;
    private IStoreInfo<TId, TModel>? _storeInfo;

    private readonly DiscordGatewayClient _client;
    private readonly Func<TModel, TRestEntity> _restFactory;
    private readonly CachePathable _path;
    private readonly IApiOutRoute<TApi> _apiRoute;
    private readonly Func<TApi, IEnumerable<TModel>> _transformer;

    private readonly GatewayIndexableActor<TActor, TId, TEntity> _indexableActor;

    internal GatewayEnumerableIndexableActor(
        DiscordGatewayClient client,
        GatewayIndexableActor<TActor, TId, TEntity> indexableActor,
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
        _indexableActor = indexableActor;
    }

    internal GatewayEnumerableIndexableActor(
        DiscordGatewayClient client,
        Func<TId, TActor> factory,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<TApi> apiRoute,
        Func<TApi, IEnumerable<TModel>> transformer
    ) : this(client, new GatewayIndexableActor<TActor, TId, TEntity>(factory), restFactory, path, apiRoute, transformer)
    {
    }

    private async ValueTask<IEntityBroker<TId, TEntity, TModel>> GetBrokerAsync(CancellationToken token)
        => _broker ??= await TEntity.GetBrokerAsync(_client, token);

    private async ValueTask<IStoreInfo<TId, TModel>> GetStoreInfoAsync(CancellationToken token)
        => _storeInfo ??= await TEntity.GetStoreInfoAsync(_client, _path, token);

    public async IAsyncEnumerable<IEntityHandle<TId, TEntity>> GetAllHandlesAsync(
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var broker = await GetBrokerAsync(token);
        var store = await GetStoreInfoAsync(token);

        await foreach (var handle in broker.GetAllAsync(_path, store, token))
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
            var broker = await GetBrokerAsync(token);

            await broker.BatchUpdateAsync(
                models,
                await GetStoreInfoAsync(token),
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

        if (options.AllowCached)
            return (await GetAllAsync(token).ToListAsync(cancellationToken: token)).AsReadOnly();

        return await FetchAllAsync(options, token);
    }

    ValueTask<IReadOnlyCollection<TCore>> IEnumerableActor<TId, TCore>.AllAsync(RequestOptions? options,
        CancellationToken token)
        => AllAsync(GatewayRequestOptions.FromRestOptions(options), token);

    public TActor Specifically(TId id) => _indexableActor.Specifically(id);
}
