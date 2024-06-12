namespace Discord;

public interface ILoadableRootEntitySource<TSource, TId, TEntity>
    : IRootEntitySource<TSource, TId, TEntity>
    where TSource : ILoadableEntity<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{

}

public interface IRootEntitySource<out TSource, in TId, out TEntity>
    where TSource : ILoadableEntity<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TSource this[TId id] { get => Specifically(id); }
    TSource Specifically(TId id);
}
