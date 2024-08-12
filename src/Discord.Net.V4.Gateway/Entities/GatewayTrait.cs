namespace Discord.Gateway;

public abstract class GatewayTrait<TId, TEntity, TIdentity>(
    DiscordGatewayClient client,
    TIdentity identity
) :
    GatewayActor<TId, TEntity, TIdentity>(client, identity),
    IGatewayTrait<TId, TEntity, TIdentity>
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>
    where TIdentity : IIdentifiable<TId>;

public interface IGatewayTrait<out TId, out TEntity, out TIdentity> :
    IGatewayActor<TId, TEntity, TIdentity>,
    IGatewayTrait<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IGatewayEntity<TId>;

public interface IGatewayTrait<out TId, out TEntity> :
    IActorTrait<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IGatewayEntity<TId>;
