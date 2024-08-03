using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[NoExposure]
public interface IBrokerProvider<TId, TEntity, TActor, TModel> : IBrokerProvider<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TActor : class, IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>
{
    internal new ValueTask<IEntityBroker<TId, TEntity, TActor, TModel>> GetBrokerAsync(
        CancellationToken token = default);

    internal new static abstract ValueTask<IEntityBroker<TId, TEntity, TActor, TModel>> GetBrokerAsync(
        DiscordGatewayClient client,
        CancellationToken token = default
    );

    async ValueTask<IEntityBroker<TId, TEntity, TModel>> IBrokerProvider<TId, TEntity, TModel>.GetBrokerAsync(
        CancellationToken token
    ) => await GetBrokerAsync(token);
}

[NoExposure]
public interface IBrokerProvider<TId, TEntity, TModel>
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
{
    internal static abstract ValueTask<IEntityBroker<TId, TEntity, TModel>> GetBrokerAsync(
        DiscordGatewayClient client,
        CancellationToken token = default
    );

    internal ValueTask<IEntityBroker<TId, TEntity, TModel>> GetBrokerAsync(CancellationToken token = default);

    internal static abstract ValueTask<IManageableEntityBroker<TId, TEntity, TModel>> GetBrokerForModelAsync(
        DiscordGatewayClient client,
        Type modelType,
        CancellationToken token = default
    );

    internal static abstract IReadOnlyCollection<BrokerProviderDelegate<TId, TEntity, TModel>> GetBrokerHierarchy();
}

internal delegate ValueTask<IManageableEntityBroker<TId, TEntity, TModel>> BrokerProviderDelegate<TId, TEntity, TModel>(
    DiscordGatewayClient client,
    CancellationToken token = default
)
    where TEntity :
    class,
    ICacheableEntity<TEntity, TId, TModel>,
    IStoreProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>;
