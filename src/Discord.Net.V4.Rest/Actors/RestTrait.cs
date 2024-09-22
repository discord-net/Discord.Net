using Discord.Models;

namespace Discord.Rest;

public abstract class RestTrait<TId, TEntity, TModel>(
    DiscordRestClient client,
    TId id
) :
    IRestTrait<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>
    where TModel : IEntityModel<TId>
{
    public IDiscordClient Client { get; } = client;

    public TId Id { get; } = id;
}

public interface IRestTrait<out TId, out TEntity, in TModel> :
    IRestTrait<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>
    where TModel : IEntityModel<TId>;

public interface IRestTrait<out TId, out TEntity> :
    IActorTrait<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>;