using Discord.Paging;

namespace Discord;

public interface IEnumerableActor<in TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    Task<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default);
}


public interface IEnumerableIndexableActor<out TActor, in TId, TEntity> :
    IIndexableActor<TActor, TId, TEntity>,
    IEnumerableActor<TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>;

public interface IPagedIndexableActor<out TActor, in TId, out TEntity, in TPageParams> :
    IPagedIndexableActor<TActor, TId, TEntity, TEntity, TPageParams>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TPageParams : IPagingParams;


public interface IPagedIndexableActor<out TActor, in TId, out TEntity, out TPaged, in TPageParams> :
    IIndexableActor<TActor, TId, TEntity>,
    IPagedActor<TId, TPaged, TPageParams>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TPaged : class, IEntity<TId>
    where TPageParams : IPagingParams;

public interface IPagedActor<in TId, out TPaged, in TPageParams>
    where TId : IEquatable<TId>
    where TPaged : class, IEntity<TId>
    where TPageParams : IPagingParams
{
    IAsyncPaged<TPaged> PagedAsync(TPageParams? args = default, RequestOptions? options = null);
}

public interface IIndexableActor<out TActor, in TId, out TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TActor this[TId id] => Specifically(id);
    TActor Specifically(TId id);
}
