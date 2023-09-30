namespace Discord;

/// <summary>
///     Type of a <see cref="SelectDefaultValueType" />.
/// </summary>
public enum SelectDefaultValueType
{
    /// <summary>
    ///     The select menu default value is a user.
    /// </summary>
    User = 0,

    /// <summary>
    ///     The select menu default value is a role.
    /// </summary>
    Role = 1,

    /// <summary>
    ///     The select menu default value is a channel.
    /// </summary>
    Channel = 2
}
