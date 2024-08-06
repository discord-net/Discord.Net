using Discord.Paging;
using System.Collections.Immutable;

namespace Discord.Rest;

internal sealed class RestPager<TId, TEntity, TModel, TPagingModel, TParams>(
    DiscordRestClient client,
    IPathable path,
    RequestOptions options,
    Func<TPagingModel, IEnumerable<TModel>> modelsMapper,
    Func<TModel, TPagingModel, TEntity> entityFactory,
    TParams? pagingParams,
    Func<TModel[], CancellationToken, ValueTask>? onBatch = null,
    Func<TPagingModel, RequestOptions, CancellationToken, ValueTask>? onPage = null
):
    IAsyncPaged<TEntity>
    where TId : IEquatable<TId>
    where TEntity : RestEntity<TId>
    where TModel : class
    where TParams : class, IPagingParams<TParams, TPagingModel>
    where TPagingModel : class
{
    public int? PageSize => pagingParams?.PageSize;
    public int? Total => pagingParams?.Total;

    public async IAsyncEnumerator<IReadOnlyCollection<TEntity>> GetAsyncEnumerator(CancellationToken token = default)
    {
        var amountFetched = 0;
        TPagingModel? lastRequest = null;

        while (amountFetched < pagingParams?.Total)
        {
            var route = TParams.GetRoute(pagingParams, path, lastRequest);

            if (route is null)
                yield break;

            var request = lastRequest = await client.RestApiClient.ExecuteAsync(
                route,
                options,
                token
            );

            if (request is null)
                yield break;

            if (onPage is not null)
                await onPage(request, options, token);

            var batchModels = modelsMapper(request).ToArray();

            if (onBatch is not null)
                await onBatch(batchModels, token);

            // TODO: 'AsParallel' may not be performant in this case.
            var batch = batchModels
                .AsParallel()
                .Select(model => entityFactory(model, request))
                .ToImmutableList();

            amountFetched += batch.Count;

            yield return batch;
        }
    }
}
