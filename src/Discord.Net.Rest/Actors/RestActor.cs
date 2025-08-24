namespace Discord.Rest;

public interface IRestActor<out TId> :
    IActor<TId>,
    IRestClientProvider
    where TId : IEquatable<TId>;

public interface IRestActor<out TId, out TEntity> : 
    IRestActor<TId>,
    IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;

public abstract class RestActor<TId> : IRestActor<TId>
    where TId : IEquatable<TId>
{
    public TId Id { get; }
    public DiscordRestClient Client { get; }

    protected RestActor(DiscordRestClient client, TId id)
    {
        Id = id;
        Client = client;
    }
}

public abstract class RestActor<TId, TEntity> :
    RestActor<TId>,
    IRestActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
{
    protected RestActor(DiscordRestClient client, TId id) : base(client, id)
    {
    }
}