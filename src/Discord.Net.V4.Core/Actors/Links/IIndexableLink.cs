using Discord.Models;

namespace Discord;

public interface IIndexableLink<out TActor, TId, out TEntity, in TModel> :
    ILink<TActor, TId, TEntity, TModel> 
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    TActor this[TId id] => Specifically(id);

    TActor Specifically(TId id);

    TEntity IEntityProvider<TEntity, TModel>.CreateEntity(TModel model)
        => Specifically(model.Id).CreateEntity(model);
}
