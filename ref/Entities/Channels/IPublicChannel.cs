using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IPublicChannel : IChannel
    {
        /// <summary> Gets the server this channel is a member of. </summary>
        Server Server { get; }
        /// <summary> Gets a collection of permission overwrites for this channel. </summary>
        IEnumerable<PermissionOverwriteEntry> PermissionOverwrites { get; }
        /// <summary> Gets the position of this public channel relative to others of the same type. </summary>
        int Position { get; }

        /// <summary> Gets a user in this channel with the given id. </summary>
        new Task<ServerUser> GetUser(ulong id);
        /// <summary> Gets a collection of all users in this channel. </summary>
        new Task<IEnumerable<ServerUser>> GetUsers();

        /// <summary> Gets the permission overwrite for a specific user, or null if one does not exist. </summary>
        OverwritePermissions? GetPermissionOverwrite(ServerUser user);
        /// <summary> Gets the permission overwrite for a specific role, or null if one does not exist. </summary>
        OverwritePermissions? GetPermissionOverwrite(Role role);
        /// <summary> Downloads a collection of all invites to this server. </summary>
        Task<IEnumerable<Invite>> GetInvites();

        /// <summary> Adds or updates the permission overwrite for the given user. </summary>
        Task UpdatePermissionOverwrite(ServerUser user, OverwritePermissions permissions);
        /// <summary> Adds or updates the permission overwrite for the given role. </summary>
        Task UpdatePermissionOverwrite(Role role, OverwritePermissions permissions);
        /// <summary> Removes the permission overwrite for the given user, if one exists. </summary>
        Task RemovePermissionOverwrite(ServerUser user);
        /// <summary> Removes the permission overwrite for the given role, if one exists. </summary>
        Task RemovePermissionOverwrite(Role role);

        /// <summary> Creates a new invite to this channel. </summary>
        /// <param name="maxAge"> Time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false);
    }
}
