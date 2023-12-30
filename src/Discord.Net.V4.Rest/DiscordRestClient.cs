using System.Text.Json;

namespace Discord.Rest;

public sealed class DiscordRestClient : IDiscordClient
{
    public IEntitySource<ulong, ISelfUser> CurrentUser { get; }
    public ApiClient RestApiClient { get; }
    internal RateLimiter RateLimiter { get; }

    internal readonly DiscordConfig Config;

    public DiscordRestClient(DiscordConfig config)
    {
        Config = config;

        RestApiClient = new ApiClient(this, config.Token);
        RateLimiter = new RateLimiter();
    }

    public void Dispose() => throw new NotImplementedException();

    public ValueTask DisposeAsync() => throw new NotImplementedException();

    IRestApiClient IDiscordClient.RestApiClient => RestApiClient;
}
