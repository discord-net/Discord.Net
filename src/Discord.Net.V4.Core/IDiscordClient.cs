namespace Discord;

/// <summary>
///     Represents a generic Discord client.
/// </summary>
public interface IDiscordClient : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     Gets the current state of connection.
    /// </summary>
    ConnectionState ConnectionState { get; }

    /// <summary>
    ///     Gets the currently logged-in user.
    /// </summary>
    IEntitySource<ulong, ISelfUser> CurrentUser { get; }

    /// <summary>
    ///     Gets the token type of the logged-in user.
    /// </summary>
    TokenType TokenType { get; }
}
