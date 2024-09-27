namespace Discord;

public interface IPagedLinkProvider<out TEntity, in TParams>
    where TEntity : IEntity
    where TParams : class, IPagingParams
{
    IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null);
}