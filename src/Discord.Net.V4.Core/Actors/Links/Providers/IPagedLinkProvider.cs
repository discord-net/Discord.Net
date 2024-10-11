namespace Discord;

public interface IPagedLinkProvider<out TEntity, in TParams>
    where TParams : IPagingParams
{
    IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null);
}