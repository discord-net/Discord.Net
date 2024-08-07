using Discord.Models;

namespace Discord;

public interface IEnumerableIndexableActor<out TActor, in TId, TEntity> :
    IIndexableActor<TActor, TId, TEntity>,
    IEnumerableActor<TId, TEntity>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, IEntityModel<TId>>
    where TId : IEquatable<TId>;
