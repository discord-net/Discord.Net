namespace Discord;

public interface IRestApiProvider : IAsyncDisposable, IDisposable
{
    Task LoginAsync(TokenType tokenType, string token, bool validateToken, CancellationToken? cancellationToken = null);

    Task LogoutAsync(CancellationToken? cancellationToken = null);
}
