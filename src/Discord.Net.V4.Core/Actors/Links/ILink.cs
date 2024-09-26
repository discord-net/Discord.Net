using Discord.Models;

namespace Discord;

public interface ILink<out TActor, in TId, out TEntity, in TModel> :
    IEntityProvider<TEntity, TModel>,
    IActorProvider<TActor, TId>,
    IPathable
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IModel;