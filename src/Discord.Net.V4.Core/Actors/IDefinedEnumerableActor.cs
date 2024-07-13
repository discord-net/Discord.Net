namespace Discord;

public interface IDefinedEnumerableActor<out TActor, TId, TEntity> :
    IEnumerableIndexableActor<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    IReadOnlyCollection<TId> Ids { get; }

    IEnumerable<TActor> Specifically(params TId[] ids)
        => Specifically((IEnumerable<TId>)ids);
    IEnumerable<TActor> Specifically(IEnumerable<TId> ids);
}
