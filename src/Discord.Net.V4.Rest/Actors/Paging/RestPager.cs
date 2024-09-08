using Discord;
using System.Collections.Immutable;
using Discord.Models;

namespace Discord.Rest;

internal sealed class RestPager<TId, TEntity, TModel, TPagingModel, TParams>(
    DiscordRestClient client,
    IPathable path,
    RequestOptions options,
    Func<TPagingModel, IEnumerable<TModel>> modelsMapper,
    Func<TModel, TPagingModel, TEntity> entityFactory,
    TParams? pagingParams,
    Func<IEnumerable<TModel>, CancellationToken, ValueTask>? onBatch = null,
    Func<TPagingModel, RequestOptions, CancellationToken, ValueTask>? onPage = null
):
    IAsyncPaged<TEntity>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId, TModel>
    where TModel : IEntityModel<TId>
    where TParams : class, IPagingParams<TParams, TPagingModel>
    where TPagingModel : class
{
    public int? PageSize => pagingParams?.PageSize;
    public int? Total => pagingParams?.Total;

    public async IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken token = default)
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

            if (onBatch is not null) await onBatch(batchModels, token);

            amountFetched += batchModels.Length;

            foreach (var model in batchModels)
            {
                yield return entityFactory(model, request);
            }
        }
    }
}
