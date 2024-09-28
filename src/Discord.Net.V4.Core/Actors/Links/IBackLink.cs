using Discord.Models;

namespace Discord;

public interface IBackLink<out TSource, out TActor, in TId, out TEntity, in TModel> :
    ILink<TActor, TId, TEntity, TModel>
    where TSource : class, IPathable
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IModel
{
    internal TSource Source { get; }
}