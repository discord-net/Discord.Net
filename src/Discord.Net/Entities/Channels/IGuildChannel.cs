using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IGuildChannel : IChannel, IDeletable
    {
        /// <summary> Gets the name of this channel. </summary>
        string Name { get; }
        /// <summary> Gets the position of this channel in the guild's channel list, relative to others of the same type. </summary>
        int Position { get; }

        /// <summary> Gets the guild this channel is a member of. </summary>
        IGuild Guild { get; }

        /// <summary> Creates a new invite to this channel. </summary>
        /// <param name="maxAge"> The time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the guild after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 1800, int? maxUses = default(int?), bool isTemporary = false, bool withXkcd = false);
        /// <summary> Returns a collection of all invites to this channel. </summary>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync();

        /// <summary> Gets a collection of permission overwrites for this channel. </summary>
        IReadOnlyCollection<Overwrite> PermissionOverwrites { get; }
        
        /// <summary> Modifies this guild channel. </summary>
        Task ModifyAsync(Action<ModifyGuildChannelParams> func);

        /// <summary> Gets the permission overwrite for a specific role, or null if one does not exist. </summary>
        OverwritePermissions? GetPermissionOverwrite(IRole role);
        /// <summary> Gets the permission overwrite for a specific user, or null if one does not exist. </summary>
        OverwritePermissions? GetPermissionOverwrite(IUser user);
        /// <summary> Removes the permission overwrite for the given role, if one exists. </summary>
        Task RemovePermissionOverwriteAsync(IRole role);
        /// <summary> Removes the permission overwrite for the given user, if one exists. </summary>
        Task RemovePermissionOverwriteAsync(IUser user);
        /// <summary> Adds or updates the permission overwrite for the given role. </summary>
        Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions);
        /// <summary> Adds or updates the permission overwrite for the given user. </summary>
        Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions);

        /// <summary> Gets a collection of all users in this channel. </summary>
        new Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync();
        /// <summary> Gets a user in this channel with the provided id.</summary>
        new Task<IGuildUser> GetUserAsync(ulong id);
    }
}