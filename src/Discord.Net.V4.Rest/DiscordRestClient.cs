using Discord.Rest;

namespace Discord.Rest;

public class DiscordRestClient : IAsyncDisposable, IDisposable
{
    internal IRestApiProvider ApiClient { get; }
    internal DiscordRestConfig Config { get; }

    public DiscordRestClient(DiscordRestConfig? config = null)
    {
        Config = config ?? new();
        ApiClient = config?.ApiClient ?? new DiscordRestApiClient();
    }

    public Task LoginAsync(TokenType tokenType, string token, bool validateToken, CancellationToken? cancellationToken = null)
        => ApiClient.LoginAsync(tokenType, token, validateToken, cancellationToken);

    public Task LogoutAsync(CancellationToken? cancellationToken = null)
        => ApiClient.LogoutAsync(cancellationToken);

    public ValueTask DisposeAsync() => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();
}
