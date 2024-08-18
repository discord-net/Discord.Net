using Discord.Models;

namespace Discord;

public interface IEnumerableIndexableLink<out TActor, TId, TEntity, in TModel> :
    IIndexableLink<TActor, TId, TEntity, TModel>,
    IEnumerableLink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>;
