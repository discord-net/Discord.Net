using Discord.Models;

namespace Discord;

[BackLinkable]
public partial interface IDefinedLink<out TActor, TId, out TEntity, in TModel> :
    ILink<TActor, TId, TEntity, TModel> 
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    IReadOnlyCollection<TId> Ids { get; }
}