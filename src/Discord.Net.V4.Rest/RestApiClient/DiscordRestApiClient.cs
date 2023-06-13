namespace Discord.Rest;

public partial class DiscordRestApiClient : IRestApiProvider
{
    public ValueTask DisposeAsync() => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();

    public Task LoginAsync(TokenType tokenType, string token, bool validateToken, CancellationToken? cancellationToken = null) => throw new NotImplementedException();

    public Task LogoutAsync(CancellationToken? cancellationToken = null) => throw new NotImplementedException();
}
