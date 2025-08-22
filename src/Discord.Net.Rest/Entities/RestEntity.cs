namespace Discord.Models.Rest;

public abstract class RestEntity<TId, TModel> :
    RestEntity<TId>,
    IModeledBy<TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    public TModel Model { get; protected set; }
    
    protected RestEntity(TModel model, DiscordRestClient client) : base(model.Id, client)
    {
        Model = model;
    }
}

public abstract class RestEntity<TId> :
    IEntity<TId>
    where TId : IEquatable<TId>
{
    public TId Id { get; }
    public DiscordRestClient Client { get; }

    public RestEntity(TId id, DiscordRestClient client)
    {
        Id = id;
        Client = client;
    }

    IDiscordClient IClientProvider.Client => Client;
}