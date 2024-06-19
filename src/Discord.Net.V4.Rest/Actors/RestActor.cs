namespace Discord.Rest;

public abstract class RestActor<TId, TEntity>(DiscordRestClient client, TId id) : IActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
{
    public DiscordRestClient Client { get; } = client;

    public TId Id { get; } = id;

    IDiscordClient IClientProvider.Client => Client;
}
