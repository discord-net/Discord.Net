namespace Discord.Rest.Actors;

public abstract class RestActor<TId, TEntity> :
    IActor<TId, TEntity>,
    IRestClientProvider
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
{
    public TId Id { get; }
    public DiscordRestClient Client { get; }

    protected RestActor(DiscordRestClient client, TId id)
    {
        Id = id;
        Client = client;
    }
}