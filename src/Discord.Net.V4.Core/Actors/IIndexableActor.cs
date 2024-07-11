namespace Discord;

public interface IIndexableActor<out TActor, in TId, out TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TActor this[TId id] => Specifically(id);

    TActor Specifically(TId id);
}
