namespace Discord.Models.Rest.Actors;

public abstract class RestActor<TId, TEntity> : IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
{
    public TId Id { get; }
    public DiscordRestClient Client { get; }

    protected RestActor(TId id, DiscordRestClient client)
    {
        Id = id;
        Client = client;
    }
    
    IDiscordClient IClientProvider.Client => Client;
}