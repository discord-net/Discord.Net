using Discord.Models;

namespace Discord.Rest;

public abstract class RestActor<TId, TEntity, TIdentity, TModel>(DiscordRestClient client, TIdentity identity) :
    IRestActor<TId, TEntity, TIdentity, TModel>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TIdentity : IIdentifiable<TId>
    where TModel : IEntityModel<TId>
{
    public DiscordRestClient Client { get; } = client;

    public TId Id { get; } = identity.Id;

    internal abstract TIdentity Identity { get; }
    
    internal abstract TEntity CreateEntity(TModel model);

    TIdentity IRestActor<TId, TEntity, TIdentity, TModel>.Identity => Identity;
    TEntity IEntityProvider<TEntity, TModel>.CreateEntity(TModel model) => CreateEntity(model);
}

public interface IRestActor<out TId, out TEntity, out TIdentity, in TModel> :
    IRestActor<TId, TEntity>,
    IEntityProvider<TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>
    where TModel : IEntityModel<TId>
{
    internal TIdentity Identity { get; }
}

public interface IRestActor<out TId, out TEntity> :
    IActor<TId, TEntity>,
    IRestClientProvider
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>;
