using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Discord.Rest;

public sealed class DiscordRestClient : IDiscordClient
{
    public RestLoadableEntity<,,,> CurrentUser { get; }
    public ApiClient RestApiClient { get; }
    internal RateLimiter RateLimiter { get; }

    internal RequestOptions DefaultRequestOptions { get; }

    internal readonly DiscordConfig Config;

    internal readonly ILogger Logger;

    public DiscordRestClient(DiscordConfig config, ILogger logger)
    {
        Config = config;
        Logger = logger;
        RestApiClient = new ApiClient(this, config.Token);
        RateLimiter = new RateLimiter();
        DefaultRequestOptions = new RequestOptions(
            timeout: DiscordConfig.DefaultRequestTimeout,
            retryMode: Config.DefaultRetryMode,
            useSystemClock: false
        );

        // load the ID from the token

        CurrentUser = RestLoadableEntity<,,,>.Create(
            this,
            TokenUtils.GetUserIdFromToken(config.Token.Value),
            Routes.GetCurrentUser,
            RestSelfUser.Create
        );
    }

    public void Dispose() => throw new NotImplementedException();

    public ValueTask DisposeAsync() => throw new NotImplementedException();

    IRestApiClient IDiscordClient.RestApiClient => RestApiClient;
    ILoadableEntity<ulong, ISelfUser> IDiscordClient.SelfUser => CurrentUser;
}
