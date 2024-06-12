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
    /// <param name="inner">The inner exception.</param>
    public DiscordException(string message, Exception? inner = null)
        : base(message, inner)
    {
    }
}
