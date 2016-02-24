using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IPublicChannel : IChannel
    {
        Server Server { get; }

        string Name { get; set; }
        int Position { get; set; }
        
        IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }

        PermissionOverwrite? GetPermissionsRule(User user);
        PermissionOverwrite? GetPermissionsRule(Role role);
        Task<IEnumerable<Invite>> DownloadInvites();

        Task Delete();

        Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false);

        Task AddPermissionsRule(User user, ChannelPermissions allow, ChannelPermissions deny);
        Task AddPermissionsRule(User user, TriStateChannelPermissions permissions);
        Task AddPermissionsRule(Role role, ChannelPermissions allow, ChannelPermissions deny);
        Task AddPermissionsRule(Role role, TriStateChannelPermissions permissions);
        Task RemovePermissionsRule(User user);
        Task RemovePermissionsRule(Role role);
    }
}
