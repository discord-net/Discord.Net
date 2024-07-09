namespace Discord.Rest;

public abstract class RestActor<TId, TEntity, TIdentity>(DiscordRestClient client, TIdentity identity) :
    IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TIdentity : IIdentifiable<TId>
{
    public DiscordRestClient Client { get; } = client;

    public TId Id => Identity.Id;

    public TIdentity Identity { get; } = identity;

    public static implicit operator TIdentity(RestActor<TId, TEntity, TIdentity> actor) => actor.Identity;

    IDiscordClient IClientProvider.Client => Client;
}
