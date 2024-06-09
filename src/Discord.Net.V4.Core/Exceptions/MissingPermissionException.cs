namespace Discord;

/// <summary>
///     Represents an exception thrown when trying to preform an action with insufficient permissions.
/// </summary>
public sealed class MissingPermissionException : DiscordException
{
    /// <summary>
    ///     Constructs a new <see cref="MissingPermissionException" />.
    /// </summary>
    /// <param name="message">A detailed message explaining why the exception was thrown.</param>
    public MissingPermissionException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Constructs a new <see cref="MissingPermissionException" />.
    /// </summary>
    /// <param name="missing">The permissions that were missing.</param>
    public MissingPermissionException(GuildPermission missing)
        : base($"Missing the required permission(s) {missing}")
    {
        Permissions = missing;
    }

    /// <summary>
    ///     Gets the permissions that was required but missing.
    /// </summary>
    public GuildPermission? Permissions { get; }
}
