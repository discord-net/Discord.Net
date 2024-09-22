namespace Discord.Rest;

public abstract class RestEntity<TId>(DiscordRestClient client, TId id) : 
    IRestEntity<TId>
    where TId : IEquatable<TId>
{
    public DiscordRestClient Client { get; } = client;
    public TId Id { get; } = id;
}

public interface IRestEntity<out TId> : 
    IEntity<TId>,
    IRestClientProvider
    where TId : IEquatable<TId>;