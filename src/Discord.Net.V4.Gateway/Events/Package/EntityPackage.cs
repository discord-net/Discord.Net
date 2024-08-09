using Discord.Models;

namespace Discord.Gateway;

public class EntityPackage<TId, TEntity, TActor, TModel>(TId id)
    where TId : IEquatable<TId>
    where TActor : class, IGatewayActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>>
    where TEntity : GatewayEntity<TId>, IEntityOf<TModel>, IProxied<TActor>
    where TModel : class, IEntityModel<TId>
{
    public TId Id { get; } = id;
    public TActor? Actor { get; init; }
    public TEntity? Entity { get; init; }
    public TModel? Model { get; init; }

}
