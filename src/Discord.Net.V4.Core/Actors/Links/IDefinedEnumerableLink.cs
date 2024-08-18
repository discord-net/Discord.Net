using Discord.Models;

namespace Discord;

public interface IDefinedEnumerableLink<out TActor, TId, TEntity, in TModel> :
    IEnumerableIndexableLink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    IReadOnlyCollection<TId> Ids { get; }

    IEnumerable<TActor> Specifically(params TId[] ids)
        => Specifically((IEnumerable<TId>)ids);

    IEnumerable<TActor> Specifically(IEnumerable<TId> ids);
}
