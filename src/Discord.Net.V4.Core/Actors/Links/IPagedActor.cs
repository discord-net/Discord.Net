using Discord.Models;
using Discord.Paging;

namespace Discord;

public interface IPagedLink<out TActor, TId, out TEntity, in TModel, in TPageParams> :
    ILink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
    where TPageParams : IPagingParams
{
    IAsyncPaged<TEntity> PagedAsync(TPageParams? args = default, RequestOptions? options = null);
}
