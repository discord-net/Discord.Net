namespace Discord.Rest;

public abstract class RestEntity<TId>(DiscordRestClient client, TId id) : IEntity<TId>
    where TId : IEquatable<TId>
{
    protected DiscordRestClient Client { get; } = client;
    public TId Id { get; } = id;

    IDiscordClient IEntity.Client => Client;
}
