namespace Discord;

public interface IPagedIndexableActor<out TActor, in TId, out TEntity, out TPaged, in TPageParams> :
    IIndexableActor<TActor, TId, TEntity>,
    IPagedActor<TId, TPaged, TPageParams>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TPaged : class, IEntity<TId>
    where TPageParams : IPagingParams;

public interface IPagedIndexableActor<out TActor, in TId, out TEntity, in TPageParams> :
    IPagedIndexableActor<TActor, TId, TEntity, TEntity, TPageParams>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TPageParams : IPagingParams;
