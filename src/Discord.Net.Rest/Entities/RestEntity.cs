using Discord.Models;

namespace Discord.Rest;

public abstract class RestEntity<TId, TModel> :
    RestEntity<TId>,
    IModeledBy<TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    public virtual TModel Model { get; }
    
    protected RestEntity(DiscordRestClient client, TModel model) : base(model.Id, client)
    {
        Model = model;
    }
}

public abstract class RestEntity<TId> :
    IRestEntity<TId>
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