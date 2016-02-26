using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IPublicChannel : IChannel
    {
        /// <summary> Gets the server this channel is a member of. </summary>
        Server Server { get; }
        /// <summary> Gets a collection of permission overwrites for this channel. </summary>
        IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }
        /// <summary> Gets the name of this public channel. </summary>
        string Name { get; }
        /// <summary> Gets the position of this public channel relative to others of the same type. </summary>
        int Position { get; }   

        /// <summary> Gets the permission overwrite for a specific user, or null if one does not exist. </summary>
        PermissionOverwrite? GetPermissionOverwrite(User user);
        /// <summary> Gets the permission overwrite for a specific role, or null if one does not exist. </summary>
        PermissionOverwrite? GetPermissionOverwrite(Role role);
        /// <summary> Downloads a collection of all invites to this server. </summary>
        Task<IEnumerable<Invite>> GetInvites();
        
        /// <summary> Adds or updates the permission overwrite for the given user. </summary>
        Task UpdatePermissionOverwrite(User user, ChannelPermissions allow, ChannelPermissions deny);
        /// <summary> Adds or updates the permission overwrite for the given user. </summary>
        Task UpdatePermissionOverwrite(User user, TriStateChannelPermissions permissions);
        /// <summary> Adds or updates the permission overwrite for the given role. </summary>
        Task UpdatePermissionOverwrite(Role role, ChannelPermissions allow, ChannelPermissions deny);
        /// <summary> Adds or updates the permission overwrite for the given role. </summary>
        Task UpdatePermissionOverwrite(Role role, TriStateChannelPermissions permissions);
        /// <summary> Removes the permission overwrite for the given user, if one exists. </summary>
        Task RemovePermissionOverwrite(User user);
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
