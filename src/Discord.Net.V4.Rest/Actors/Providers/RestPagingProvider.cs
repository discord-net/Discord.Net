using Discord.Models;
using MorseCode.ITask;

namespace Discord.Rest;

internal sealed class RestPagingProvider<TEntity, TModel, TParams>(
    DiscordRestClient client,
    Func<TModel, IEnumerable<TEntity>> entityFactory,
    IPathable? path = null
):
    IPagedLinkProvider<TEntity, TParams>
    where TParams : class, IPagingParams<TParams, TModel>
    where TModel : class, IModel
    where TEntity : class, IEntity
{
    public IAsyncPaged<TEntity> PagedAsync(TParams? args = default, RequestOptions? options = null)
    {
        return new RestPager<TEntity, TModel, TParams>(
            client,
            entityFactory,
            args,
            path,
            options
        );
    }
}