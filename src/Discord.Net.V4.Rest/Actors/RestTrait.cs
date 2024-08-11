namespace Discord.Rest;

public abstract class RestTrait<TId, TEntity, TIdentity>(DiscordRestClient client, TIdentity identity) :
    RestActor<TId, TEntity, TIdentity>(client, identity),
    IRestTrait<TId, TEntity, TIdentity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TIdentity : IIdentifiable<TId>;

public interface IRestTrait<out TId, out TEntity, out TIdentity> :
    IRestTrait<TId, TEntity>,
    IRestActor<TId, TEntity, TIdentity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>;

public interface IRestTrait<out TId, out TEntity> :
    IRestActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>;
