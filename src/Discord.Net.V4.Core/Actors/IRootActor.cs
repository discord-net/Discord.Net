using Discord.Paging;

namespace Discord;

public interface ILoadableRootActor<out TSource, in TId, TEntity> : IRootActor<TSource, TId, TEntity>
    where TSource : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    Task<IReadOnlyCollection<TEntity>> AllAsync();
}

public interface IPagedLoadableRootActor<out TSource, in TId, out TEntity> :
    IPagedLoadableRootActor<TSource, TId, TEntity, TEntity>
    where TSource : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;

public interface IPagedLoadableRootActor<out TSource, in TId, out TEntity, out TPagedEntityType> :
    IRootActor<TSource, TId, TEntity>
    where TSource : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TPagedEntityType : class, IEntity<TId>
{
    IAsyncPaged<TPagedEntityType> PagedAsync();
}

public interface IRootActor<out TSource, in TId, out TEntity>
    where TSource : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TSource this[TId id] { get => Specifically(id); }
    TSource Specifically(TId id);
}
