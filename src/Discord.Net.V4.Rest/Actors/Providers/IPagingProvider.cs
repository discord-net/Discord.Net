using Discord.Models;
using MorseCode.ITask;

namespace Discord.Rest;

public interface IPagingProvider<in TParams, out TEntity, out TModel>
    where TModel : IModel
    where TEntity : IEntityOf<TModel>
    where TParams : class, IPagingParams
{
    IAsyncPaged<TEntity> CreatePagedAsync(
        TParams? pageParams = null,
        RequestOptions? options = null
    );

    // ITask<IEnumerable<TModel>?> NextAsync();
    //
    // static IPagingProvider<TModel> Create<TApi, TParams>(
    //     DiscordRestClient client,
    //     TParams pageParams,
    //     Func<TApi, IEnumerable<TModel>?> mapper,
    //     IPathable? path = null,
    //     RequestOptions? options = null,
    //     CancellationToken token = default
    // )
    //     where TParams : class, IPagingParams<TParams, TApi>
    //     where TApi : class
    //     => new RestPagingProvider<TModel, TApi, TParams>(client, pageParams, mapper, path, options, token);
}