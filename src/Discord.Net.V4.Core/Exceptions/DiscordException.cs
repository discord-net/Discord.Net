namespace Discord;

/// <summary>
///     Represents an exception related to the Discord.Net library.
/// </summary>
public class DiscordException : Exception
{
    /// <summary>
    ///     Constructs a new <see cref="DiscordException" />.
    /// </summary>
    /// <param name="message">A detailed message explaining why the exception was thrown.</param>
    public DiscordException(string message)
        : base(message)
    {
        Routes = Array.Empty<string>();
    }

    /// <summary>
    ///     Constructs a new <see cref="DiscordException" />.
    /// </summary>
    /// <param name="message">A detailed message explaining why the exception was thrown.</param>
    /// <param name="options">The request options used for preforming the operation that caused the exception.</param>
    public DiscordException(string message, RequestOptions? options)
        : this(message)
    {
        RequestOptions = options;
    }

    /// <summary>
    ///     Gets the request options used for preforming the operation that caused the exception.
    /// </summary>
    public RequestOptions? RequestOptions { get; }

    /// <summary>
    ///     A read-only collection of api routes called during the execution that caused this exception to be thrown.
    /// </summary>
    public IReadOnlyCollection<string> Routes { get; }
}
