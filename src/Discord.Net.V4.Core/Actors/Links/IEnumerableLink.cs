using Discord.Models;

namespace Discord;

[BackLinkable]
public partial interface IEnumerableLink<out TActor, TId, TEntity, in TModel> :
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    ValueTask<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default);
}
