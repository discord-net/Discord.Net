namespace Discord.Rest;

public interface IRestApiClient
{
    Task ExecuteAsync(BasicApiRoute route, RequestOptions options, CancellationToken token);

    Task<T?> ExecuteAsync<T>(ApiRoute<T> route, RequestOptions options, CancellationToken token)
        where T : class;

    Task ExecuteAsync<T>(ApiBodyRoute<T> route, RequestOptions options, CancellationToken token)
        where T : class;

    Task<U?> ExecuteAsync<T, U>(ApiBodyRoute<T, U> route, RequestOptions options, CancellationToken token)
        where T : class
        where U : class;

    async Task<U> ExecuteRequiredAsync<T, U>(ApiBodyRoute<T, U> route, RequestOptions options, CancellationToken token)
        where T : class
        where U : class
    {
        return await ExecuteAsync(route, options, token) ?? throw new MissingApiResultException(route, options);
    }

    async Task<T> ExecuteRequiredAsync<T>(ApiRoute<T> route, RequestOptions options, CancellationToken token)
        where T : class
    {
        return await ExecuteAsync(route, options, token) ?? throw new MissingApiResultException(route, options);
    }
}
