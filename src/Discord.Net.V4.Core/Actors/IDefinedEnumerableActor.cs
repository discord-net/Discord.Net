using Discord.Models;

namespace Discord;

public interface IDefinedIndexableActor<out TActor, TId, out TEntity> :
    IIndexableActor<TActor, TId, TEntity>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, IEntityModel<TId>>
    where TId : IEquatable<TId>
{
    IReadOnlyCollection<TId> Ids { get; }

    IEnumerable<TActor> Specifically(params TId[] ids)
        => Specifically((IEnumerable<TId>)ids);

    IEnumerable<TActor> Specifically(IEnumerable<TId> ids);
}

public interface IDefinedEnumerableActor<out TActor, TId, TEntity> :
    IEnumerableIndexableActor<TActor, TId, TEntity>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, IEntityModel<TId>>
    where TId : IEquatable<TId>
{
    IReadOnlyCollection<TId> Ids { get; }

    IEnumerable<TActor> Specifically(params TId[] ids)
        => Specifically((IEnumerable<TId>)ids);

    IEnumerable<TActor> Specifically(IEnumerable<TId> ids);
}
