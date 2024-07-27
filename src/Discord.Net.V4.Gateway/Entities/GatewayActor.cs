using Discord.Gateway.Cache;

namespace Discord.Gateway;

public abstract class GatewayCachedActor<TId, TEntity, TIdentity, TModel>(
    DiscordGatewayClient client,
    TIdentity identity
) :
    GatewayActor<TId, TEntity, TIdentity>(client, identity)
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>
    where TIdentity : IIdentifiable<TId>
    where TModel : class, IEntityModel<TId>
{
    protected abstract ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync(CancellationToken token = default);
}

public abstract class GatewayActor<TId, TEntity, TIdentity>(
    DiscordGatewayClient client,
    TIdentity identity
):
    IGatewayActor<TId, TEntity, TIdentity>
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>
    where TIdentity : IIdentifiable<TId>
{
    public DiscordGatewayClient Client { get; } = client;

    public TId Id { get; } = identity.Id;

    public virtual TIdentity Identity { get; } = identity;

    public static implicit operator TIdentity(GatewayActor<TId, TEntity, TIdentity> actor) => actor.Identity;
}

public interface IGatewayActor<out TId, out TEntity, out TIdentity> : IGatewayActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
{
    TIdentity Identity { get; }
}

public interface IGatewayActor<out TId, out TEntity> : IActor<TId, TEntity>, IGatewayClientProvider
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;
