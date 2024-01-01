using Discord.Models;

namespace Discord.Rest;

public class EntityUtils
{
    public static Task<T> ModifyAsync<T, U, V>(DiscordRestClient client, Func<U, ApiBodyRoute<U, T>> route,
        Action<V> func, RequestOptions options, CancellationToken token)
        where T : class, IEntityModel
        where U : class
        where V : IEntityProperties<U>, new()

    {
        var args = new V();
        func(args);
        return client.RestApiClient.ExecuteAsync(
            route(args.ToApiModel()),
            options,
            token
        )!;
    }
}
