using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord.Gateway;

public sealed class GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCore, TModel>(
    DiscordGatewayClient client,
    Func<TId, TActor> factory,
    Func<TModel, TRestEntity> restFactory,
    CachePathable path,
    IApiOutRoute<IEnumerable<TModel>> apiRoute
):
    GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCore, TModel, TModel>(
        client,
        factory,
        restFactory,
        path,
        apiRoute
    )
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
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>,
    TCore
    where TRestEntity : RestEntity<TId>, TCore
    where TCore : class, IEntity<TId>
    where TModel : class, IEntityModel<TId>;

public class GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCore, TModel, TApi> :
    GatewayIndexableActor<TActor, TId, TEntity>,
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
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>,
    TCore
    where TRestEntity : RestEntity<TId>, TCore
    where TCore : class, IEntity<TId>
    where TModel : class, TApi
    where TApi : class, IEntityModel<TId>
{
    private IEntityBroker<TId, TEntity, TModel>? _broker;
    private IStoreInfo<TId, TModel>? _storeInfo;

    private readonly DiscordGatewayClient _client;
    private readonly Func<TModel, TRestEntity> _restFactory;
    private readonly CachePathable _path;
    private readonly IApiOutRoute<IEnumerable<TApi>> _apiRoute;

    internal GatewayEnumerableIndexableActor(
        DiscordGatewayClient client,
        Func<TId, TActor> factory,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<IEnumerable<TApi>> apiRoute
    ) : base(factory)
    {
        _client = client;
        _restFactory = restFactory;
        _path = path;
        _apiRoute = apiRoute;
    }

    private async ValueTask<IEntityBroker<TId, TEntity, TModel>> GetBrokerAsync(CancellationToken token)
        => _broker ??= await TEntity.GetBrokerAsync(_client, token);

    private async ValueTask<IStoreInfo<TId, TModel>> GetStoreInfoAsync(CancellationToken token)
        => _storeInfo ??= await TEntity.GetStoreInfoAsync(_client, _path, token);

    public async ValueTask<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllHandlesAsync(
        CancellationToken token = default)
    {
        var broker = await GetBrokerAsync(token);
        return await broker.GetAllHandlesAsync(
            _path,
            await GetStoreInfoAsync(token),
            token
        );
    }

    public async ValueTask<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken token = default)
    {
        var broker = await GetBrokerAsync(token);
        return await broker.GetAllImplicitAsync(
            _path,
            await GetStoreInfoAsync(token),
            token
        );
    }

    public async Task<IReadOnlyCollection<TRestEntity>> FetchAllAsync(
        GatewayRequestOptions? options = null,
        CancellationToken token = default)
    {
        var models = await _client.RestApiClient.ExecuteAsync(
            _apiRoute,
            options ?? _client.DefaultRequestOptions,
            token
        );

        if (models is null)
            return [];

        var modelsEnumerated = models.OfType<TModel>().ToArray();

        if (modelsEnumerated.Length == 0)
            return [];

        if (options?.UpdateCache ?? false)
        {
            var broker = await GetBrokerAsync(token);

            await broker.BatchUpdateAsync(
                modelsEnumerated,
                await GetStoreInfoAsync(token),
                token
            );
        }

        return modelsEnumerated.Select(_restFactory).ToImmutableList();
    }

    public async ValueTask<IReadOnlyCollection<TCore>> AllAsync(
        GatewayRequestOptions? options = null,
        CancellationToken token = default)
    {
        options ??= _client.DefaultRequestOptions;

        if (options.AllowCached)
            return await GetAllAsync(token);

        return await FetchAllAsync(options, token);
    }

    ValueTask<IReadOnlyCollection<TCore>> IEnumerableActor<TId, TCore>.AllAsync(RequestOptions? options,
        CancellationToken token)
        => AllAsync(GatewayRequestOptions.FromRestOptions(options), token);
}
