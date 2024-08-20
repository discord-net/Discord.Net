using Discord.Models;
using Discord.Paging;

namespace Discord;

[BackLinkable]
public partial interface IPagedIndexableLink<out TActor, TId, TEntity, in TModel, out TPaged, in TPageParams> :
    ILinkType<TActor, TId, TEntity, TModel>.Indexable
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
    where TPaged : class, IEntity<TId>
    where TPageParams : IPagingParams
{
    IAsyncPaged<TPaged> PagedAsync(TPageParams? args = default, RequestOptions? options = null);
}

[BackLinkable]
public partial interface IPagedIndexableLink<out TActor, TId, TEntity, in TModel, in TPageParams> :
    IPagedIndexableLink<TActor, TId, TEntity, TModel, TEntity, TPageParams>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
    where TPageParams : IPagingParams;