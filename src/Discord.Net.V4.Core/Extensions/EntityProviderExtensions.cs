using Discord.Models;

namespace Discord;

internal static class EntityProviderExtensions
{
    public static async Task<TResult?> ExecuteAndConstructAsync<TResult, TCoreEntity, TModel, TApi, TBody>(
        this IEntityProvider<TResult, TCoreEntity, TModel> provider,
        ApiBodyRoute<TBody, TApi> route, RequestOptions? options, CancellationToken token)
        where TResult : class, TCoreEntity, IConstructable<TModel>
        where TCoreEntity : IEntity
        where TModel : IEntityModel
        where TApi : class, TModel
        where TBody : class
    {
        var model = await provider.Client.RestApiClient.ExecuteAsync(route,
            options ?? provider.Client.DefaultRequestOptions, token);

        return model is not null
            ? TResult.Construct<TResult>(model)
            : null;
    }

    public static async Task<TResult?> ExecuteAndConstructAsync<TResult, TCoreEntity, TModel, TApi>(
        this IEntityProvider<TResult, TCoreEntity, TModel> provider,
        ApiRoute<TApi> route, RequestOptions? options, CancellationToken token = default)
        where TResult : class, TCoreEntity, IConstructable<TModel>
        where TCoreEntity : IEntity
        where TModel : IEntityModel
        where TApi : class, TModel
    {
        var model = await provider.Client.RestApiClient.ExecuteAsync(route,
            options ?? provider.Client.DefaultRequestOptions, token);

        return model is not null
            ? TResult.Construct<TResult>(model)
            : null;
    }
}
