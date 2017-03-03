using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IGuildChannel : IChannel, IDeletable
    {
        /// <summary> Gets the position of this channel in the guild's channel list, relative to others of the same type. </summary>
        int Position { get; }

        /// <summary> Gets the guild this channel is a member of. </summary>
        IGuild Guild { get; }
        /// <summary> Gets the id of the guild this channel is a member of. </summary>
        ulong GuildId { get; }
        /// <summary> Gets a collection of permission overwrites for this channel. </summary>
        IReadOnlyCollection<Overwrite> PermissionOverwrites { get; }

        /// <summary> Creates a new invite to this channel. </summary>
        /// <param name="maxAge"> The time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the guild after closing their client. </param>
        Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 1800, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null);
        /// <summary> Returns a collection of all invites to this channel. </summary>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null);
        
        /// <summary> Modifies this guild channel. </summary>
        Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions options = null);

        /// <summary> Gets the permission overwrite for a specific role, or null if one does not exist. </summary>
        OverwritePermissions? GetPermissionOverwrite(IRole role);
        /// <summary> Gets the permission overwrite for a specific user, or null if one does not exist. </summary>
        OverwritePermissions? GetPermissionOverwrite(IUser user);
        /// <summary> Removes the permission overwrite for the given role, if one exists. </summary>
        Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null);
        /// <summary> Removes the permission overwrite for the given user, if one exists. </summary>
        Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null);
        /// <summary> Adds or updates the permission overwrite for the given role. </summary>
        Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null);
        /// <summary> Adds or updates the permission overwrite for the given user. </summary>
        Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null);

        /// <summary> Gets a collection of all users in this channel. </summary>
        new IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary> Gets a user in this channel with the provided id.</summary>
        new Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    }
}