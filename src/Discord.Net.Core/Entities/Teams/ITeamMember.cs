namespace Discord;

/// <summary>
///     Represents a Discord Team member.
/// </summary>
public interface ITeamMember
{
    /// <summary>
    ///     Gets the membership state of this team member.
    /// </summary>
    MembershipState MembershipState { get; }

    /// <summary>
    ///     Gets the permissions of this team member.
    /// </summary>
    string[] Permissions { get; }

    /// <summary>
    ///     Gets the team unique identifier for this team member.
    /// </summary>
    ulong TeamId { get; }

    /// <summary>
    ///     Gets the Discord user of this team member.
    /// </summary>
    IUser User { get; }

    /// <summary>
    ///     Gets the role of this team member.
    /// </summary>
    public TeamRole Role { get; }
}
