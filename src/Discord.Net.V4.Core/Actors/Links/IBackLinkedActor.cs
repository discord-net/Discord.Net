using Discord.Models;

namespace Discord;

public interface IBackLinkedActor<out TSource, out TBackLink, out TActor, TId, out TEntity, in TModel> : 
    ILinkType<TBackLink, TId, TEntity, TModel>,
    IBackLink<TSource, TActor, TId, TEntity, TModel>
    where TBackLink : class, IBackLink<TSource, TActor, TId, TEntity, TModel>, TActor
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TSource : class, IPathable;