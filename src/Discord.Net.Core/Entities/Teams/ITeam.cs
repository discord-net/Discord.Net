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
        ///     Gets the name of this team.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Gets the user identifier that owns this team.
        /// </summary>
        ulong OwnerUserId { get; }
        /// <summary>
        ///     Get the icon URL for this Team.
        /// </summary>
        /// <remarks>
        ///     This property retrieves a URL for this Team's icon. In event that the team does not have a valid icon
        ///     (i.e. their icon identifier is not set), this property will return <c>null</c>.
        /// </remarks>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <returns>A string representing the team's icon URL; <c>null</c> if the team does not have an icon in place.</returns>
        string GetIconUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);
    }
}
