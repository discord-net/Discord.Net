using Discord.Models;

namespace Discord;

public interface ILink<out TActor, TId, out TEntity, in TModel> :
    IEntityProvider<TEntity, TModel>,
    IPathable
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>;