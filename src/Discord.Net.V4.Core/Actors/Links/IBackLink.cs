using Discord.Models;

namespace Discord;

public interface IBackLink<out TSource, out TActor, TId, out TEntity, in TModel>
    where TSource : class, IPathable
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    internal TSource Source { get; }
}