using Discord.Models;
using Discord.Paging;

namespace Discord;

public interface IPagedIndexableLink<out TActor, TId, out TEntity, in TModel, out TPaged, in TPageParams> :
    IIndexableLink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
    where TPaged : class, IEntity<TId>
    where TPageParams : IPagingParams
{
    IAsyncPaged<TPaged> PagedAsync(TPageParams? args = default, RequestOptions? options = null);
}

public interface IPagedIndexableLink<out TActor, TId, out TEntity, in TModel, in TPageParams> :
    IPagedIndexableLink<TActor, TId, TEntity, TModel, TEntity, TPageParams>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
    where TPageParams : IPagingParams;
