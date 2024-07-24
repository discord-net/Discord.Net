using Discord.Models;

namespace Discord.Rest;

public abstract class RestActor<TId, TEntity, TIdentity>(DiscordRestClient client, TIdentity identity) :
    IRestActor<TId, TEntity, TIdentity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TIdentity : IIdentifiable<TId>
{
    public DiscordRestClient Client { get; } = client;

    public TId Id { get; } = identity.Id;

    public virtual TIdentity Identity { get; } = identity;

    public static implicit operator TIdentity(RestActor<TId, TEntity, TIdentity> actor) => actor.Identity;
}

public interface IRestActor<out TId, out TEntity, out TIdentity> :
    IRestActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
{
    TIdentity Identity { get; }
}

public interface IRestActor<out TId, out TEntity> :
    IActor<TId, TEntity>,
    IRestClientProvider
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;
