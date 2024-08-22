using Discord.Models;

namespace Discord.Rest;

public abstract class RestTrait<TId, TEntity, TIdentity, TModel>(
    DiscordRestClient client,
    TIdentity identity
) :
    RestActor<TId, TEntity, TIdentity, TModel>(client, identity),
    IRestTrait<TId, TEntity, TIdentity, TModel>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>
    where TIdentity : IIdentifiable<TId>
    where TModel : IEntityModel<TId>
{
    internal abstract TEntity CreateEntity(TIdentity model);
}

public interface IRestTrait<out TId, out TEntity, out TIdentity, in TModel> :
    IRestTrait<TId, TEntity>,
    IRestActor<TId, TEntity, TIdentity, TModel>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>
    where TIdentity : IIdentifiable<TId>
    where TModel : IEntityModel<TId>;

public interface IRestTrait<out TId, out TEntity> :
    IRestActor<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>;