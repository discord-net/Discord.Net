namespace Discord;

/// <summary>
///     Represents a Discord Team member role.
/// </summary>
public enum TeamRole
{
    /// <summary>
    ///     The user is the owner of the team.
    /// </summary>
    Owner,

    /// <summary>
    ///     The user is an admin in the team.
    /// </summary>
    Admin,

    /// <summary>
    ///     The user is a developer in the team.
    /// </summary>
    Developer,

    /// <summary>
    ///     The user is a read-only member of the team.
    /// </summary>
    ReadOnly,
}
