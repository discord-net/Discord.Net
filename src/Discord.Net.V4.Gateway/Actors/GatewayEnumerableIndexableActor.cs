using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord.Gateway;

public sealed class GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCore, TModel> :
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
    where TModel : class, IEntityModel<TId>
{
    private readonly DiscordGatewayClient _client;
    private readonly IEntityBroker<TId, TEntity, TActor, TModel> _broker;
    private readonly Func<TModel, TRestEntity> _restFactory;
    private readonly CachePathable _path;
    private readonly IApiOutRoute<IEnumerable<TModel>> _apiRoute;

    internal GatewayEnumerableIndexableActor(
        DiscordGatewayClient client,
        Func<TId, TActor> factory,
        IEntityBroker<TId, TEntity, TActor, TModel> broker,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<IEnumerable<TModel>> apiRoute
    ) : base(factory)
    {
        _client = client;
        _broker = broker;
        _restFactory = restFactory;
        _path = path;
        _apiRoute = apiRoute;
    }

    public ValueTask<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> GetAllHandlesAsync(
        CancellationToken token = default
    ) => _broker.GetAllHandlesAsync(_path, _store, token);

    public ValueTask<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken token = default
    ) => _broker.GetAllImplicitAsync(_path, _store, token);

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

        var modelsEnumerated = models.ToArray();

        if (modelsEnumerated.Length == 0)
            return [];

        if (options?.UpdateCache ?? false)
        {
            await _broker.BatchUpdateAsync(modelsEnumerated, TEntity.)
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
