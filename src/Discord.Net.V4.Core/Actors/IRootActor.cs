using Discord.Paging;

namespace Discord;

public interface ILoadableRootActor<out TActor, in TId, TEntity> : IRootActor<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    Task<IReadOnlyCollection<TEntity>> AllAsync();
}

public interface IPagedLoadableRootActor<out TActor, in TId, out TEntity> :
    IPagedLoadableRootActor<TActor, TId, TEntity, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;

public interface IPagedLoadableRootActor<out TActor, in TId, out TEntity, out TPagedEntityType> :
    IRootActor<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TPagedEntityType : class, IEntity<TId>
{
    IAsyncPaged<TPagedEntityType> PagedAsync();
}

public interface IRootActor<out TActor, in TId, out TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TActor this[TId id] { get => Specifically(id); }
    TActor Specifically(TId id);
}
