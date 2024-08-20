using Discord.Models;

namespace Discord;

[BackLinkable]
public partial interface IDefinedEnumerableIndexableLink<out TActor, TId, TEntity, in TModel> :
    IDefinedLink<TActor, TId, TEntity, TModel>,
    IEnumerableIndexableLink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    IEnumerable<TActor> Specifically(params TId[] ids)
        => Specifically((IEnumerable<TId>)ids);

    IEnumerable<TActor> Specifically(IEnumerable<TId> ids);
}
