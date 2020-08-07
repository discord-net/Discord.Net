using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a Discord Team.
    /// </summary>
    public interface ITeam
    {
        /// <summary>
        ///     Gets the team icon url.
        /// </summary>
        string IconUrl { get; }
        /// <summary>
        ///     Gets the team unique identifier.
        /// </summary>
        ulong Id { get; }
        /// <summary>
        ///     Gets the members of this team.
        /// </summary>
        IReadOnlyList<ITeamMember> TeamMembers { get; }
        /// <summary>
        ///     Gets the user identifier that owns this team.
        /// </summary>
        ulong OwnerUserId { get; }
    }
}
