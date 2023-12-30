using Discord.Models;

namespace Discord;

internal static class EntityProviderExtensions
{
    public static async Task<TResult?> ExecuteAndConstructAsync<TResult, TCoreEntity, TModel, TAPI, TBody>(
        this IEntityProvider<TResult, TCoreEntity, TModel> provider,
        ApiBodyRoute<TBody, TAPI> route, RequestOptions? options, CancellationToken token)
        where TResult : class, TCoreEntity, IConstructable<TModel>
        where TCoreEntity : IEntity
        where TModel : IEntityModel
        where TAPI : TModel
    {
        var model = await provider.Client.RestApiClient.ExecuteAsync(route, options ?? default, token);

        return model is not null
            ? TResult.Construct<TResult>(model)
            : null;
    }

    public static async Task<TResult?> ExecuteAndConstructAsync<TResult, TCoreEntity, TModel, TAPI>(
        this IEntityProvider<TResult, TCoreEntity, TModel> provider,
        ApiRoute<TAPI> route, RequestOptions? options, CancellationToken token = default)
        where TResult : class, TCoreEntity, IConstructable<TModel>
        where TCoreEntity : IEntity
        where TModel : IEntityModel
        where TAPI : TModel
    {
        var model = await provider.Client.RestApiClient.ExecuteAsync(route, options ?? default, token);

        return model is not null
            ? TResult.Construct<TResult>(model)
            : null;
    }
}
