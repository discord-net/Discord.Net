using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

public sealed class GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCore, TModel> :
    GatewayIndexableActor<TActor, TId, TEntity>,
    IEnumerableIndexableActor<TActor, TId, TCore>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
    where TId : IEquatable<TId>
    where TEntity :
    GatewayEntity<TId>,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext<TId, TEntity>, DiscordGatewayClient>,
    TCore
    where TRestEntity : RestEntity<TId>, TCore
    where TCore : class, IEntity<TId>
    where TModel : class, IEntityModel<TId>
{
    private readonly DiscordGatewayClient _client;
    private readonly IEntityModelStore<TId, TModel> _store;
    private readonly IEntityBroker<TId, TEntity, TActor, TModel> _broker;
    private readonly Func<TModel, TRestEntity> _restFactory;
    private readonly CachePathable _path;
    private readonly IApiOutRoute<IEnumerable<TModel>> _apiRoute;

    internal GatewayEnumerableIndexableActor(
        DiscordGatewayClient client,
        Func<TId, TActor> factory,
        IEntityModelStore<TId, TModel> store,
        IEntityBroker<TId, TEntity, TActor, TModel> broker,
        Func<TModel, TRestEntity> restFactory,
        CachePathable path,
        IApiOutRoute<IEnumerable<TModel>> apiRoute
    ) : base(factory)
    {
        _client = client;
        _store = store;
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

    public async Task<IReadOnlyCollection<TRestEntity>> FetchAllAsync(GatewayRequestOptions? options = null,
        CancellationToken token = default)
    {
        var models = await _client.RestApiClient.ExecuteAsync(
            _apiRoute,
            options ?? _client.DefaultRequestOptions,
            token
        );

        if (options?.UpdateCache ?? false)
        {
            _broker.UpdateAsync()
        }
    }

    public ValueTask<IReadOnlyCollection<TCore>> AllAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
    }
}
