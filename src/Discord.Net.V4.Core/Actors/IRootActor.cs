using Discord.Paging;

namespace Discord;

public interface IEnumerableActor<in TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    Task<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null);
}


public interface IEnumerableIndexableActor<out TActor, in TId, TEntity> :
    IIndexableActor<TActor, TId, TEntity>,
    IEnumerableActor<TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;

public interface IPagedIndexableActor<out TActor, in TId, out TEntity> :
    IPagedIndexableActor<TActor, TId, TEntity, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;


public interface IPagedIndexableActor<out TActor, in TId, out TEntity, out TPaged> :
    IIndexableActor<TActor, TId, TEntity>,
    IPagedActor<TId, TPaged>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TPaged : class, IEntity<TId>;

public interface IPagedActor<in TId, out TPaged>
    where TId : IEquatable<TId>
    where TPaged : class, IEntity<TId>
{
    IAsyncPaged<TPaged> PagedAsync(int? pageSize = null, RequestOptions? options = null);
}

public interface IIndexableActor<out TActor, in TId, out TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TActor this[TId id] => Specifically(id);
    TActor Specifically(TId id);
}
