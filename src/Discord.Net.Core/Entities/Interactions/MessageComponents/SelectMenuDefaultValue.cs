namespace Discord;

/// <summary>
///     Represents a default value of an auto-populated select menu.
/// </summary>
public struct SelectMenuDefaultValue
{
    public ulong Id { get; }

    public SelectDefaultValueType Type { get; }

    /// <summary>
    ///     Creates a new default value.
    /// </summary>
    /// <param name="id">Id of the target object.</param>
    /// <param name="type">Type of the target entity.</param>
    public SelectMenuDefaultValue(ulong id, SelectDefaultValueType type)
    {
        Id = id;
        Type = type;
    }

    /// <summary>
    ///     Creates a new default value from a <see cref="IChannel"/>.
    /// </summary>
    public SelectMenuDefaultValue FromChannel(IChannel channel)
        => new(channel.Id, SelectDefaultValueType.Channel);

    /// <summary>
    ///     Creates a new default value from a <see cref="IRole"/>.
    /// </summary>
    public SelectMenuDefaultValue FromRole(IRole role)
        => new(role.Id, SelectDefaultValueType.Role);

    /// <summary>
    ///     Creates a new default value from a <see cref="IUser"/>.
    /// </summary>
    public SelectMenuDefaultValue FromUser(IUser user)
        => new(user.Id, SelectDefaultValueType.User);
}
