namespace Discord.Rest;

public interface IRestApiClient
{
    Task ExecuteAsync(IApiRoute route, RequestOptions options, CancellationToken token);

    Task<T?> ExecuteAsync<T>(IApiOutRoute<T> outRoute, RequestOptions options, CancellationToken token)
        where T : class;

    Task ExecuteAsync<T>(IApiInRoute<T> route, RequestOptions options, CancellationToken token)
        where T : class;

    Task<U?> ExecuteAsync<T, U>(IApiInOutRoute<T, U> route, RequestOptions options, CancellationToken token)
        where T : class
        where U : class;

    async Task<U> ExecuteRequiredAsync<T, U>(IApiInOutRoute<T, U> route, RequestOptions options,
        CancellationToken token)
        where T : class
        where U : class =>
        await ExecuteAsync(route, options, token) ?? throw new MissingApiResultException(route, options);

    async Task<T> ExecuteRequiredAsync<T>(IApiOutRoute<T> outRoute, RequestOptions options, CancellationToken token)
        where T : class =>
        await ExecuteAsync(outRoute, options, token) ?? throw new MissingApiResultException(outRoute, options);
}
