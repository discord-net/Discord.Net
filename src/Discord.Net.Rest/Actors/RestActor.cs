namespace Discord.Rest;

public interface IRestActor : IRestClientProvider;

public interface IRestActor<out TId> :
    IActor<TId>,
    IRestActor
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
    public virtual TEntity? Entity => Client.Cache.GetCachedEntity<TId, TEntity>(this);
    
    protected RestActor(DiscordRestClient client, TId id) : base(client, id)
    {
    }
}