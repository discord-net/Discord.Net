using System.Collections;

namespace Discord.Rest;

internal static class RouteExtensions
{
    public static ApiModelProviderDelegate<TModel> AsRequiredProvider<TModel>(this IApiOutRoute<TModel> route)
        where TModel : class
        => async (client, options, token) =>
        {
            var result = await client.RestApiClient.ExecuteAsync(route, options, token);

            if (result is null)
                throw new MissingApiResultException(route, options);

            return result;
        };

    public static ApiModelProviderDelegate<TModel?> AsProvider<TModel>(this IApiOutRoute<TModel> route)
        where TModel : class
        => async (client, options, token) => await client.RestApiClient.ExecuteAsync(route, options, token);

    public static ApiModelProviderDelegate<IEnumerable<TModel>> OfType<TModel>(
        this ApiModelProviderDelegate<IEnumerable> provider
    ) => async (client, options, token) => (await provider(client, options, token)).OfType<TModel>();

    public static ApiModelProviderDelegate<IEnumerable<TResult>> Select<TSource, TResult>(
        this ApiModelProviderDelegate<IEnumerable<TSource>> provider,
        Func<TSource, TResult> mapper
    ) => async (client, options, token) => (await provider(client, options, token)).Select(mapper);

    public static ApiModelProviderDelegate<TResult> Map<TSource, TResult>(
        this ApiModelProviderDelegate<TSource> provider,
        Func<TSource, TResult> mapper
    ) => async (client, options, token) => mapper(await provider(client, options, token));

    public static ApiModelProviderDelegate<T> Intercept<T>(
        this ApiModelProviderDelegate<T> provider,
        Func<T, ValueTask> func
    ) => async (client, options, token) =>
    {
        var result = await provider(client, options, token);
        await func(result);
        return result;
    };
}