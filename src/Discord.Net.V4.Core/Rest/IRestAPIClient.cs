using Discord.API;

namespace Discord;

public interface IRestApiClient
{
    Task ExecuteAsync(APIRoute route, RequestOptions options, CancellationToken token);
    Task<T> ExecuteAsync<T>(APIRoute<T> route, RequestOptions options, CancellationToken token);
    Task ExecuteAsync<T>(APIBodyRoute<T> route, T body, RequestOptions options, CancellationToken token);
    Task<U> ExecuteAsync<T, U>(APIBodyRoute<T, U> route, T body, RequestOptions options, CancellationToken token);
}
