using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		public Task SetChannelUserPermissions(Channel channel, User user, ChannelPermissions allow = null, ChannelPermissions deny = null)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (user == null) throw new ArgumentNullException(nameof(user));
			CheckReady();

			return SetChannelPermissions(channel, user?.Id, PermissionTarget.User, allow, deny);
		}
		public Task SetChannelUserPermissions(Channel channel, User user, DualChannelPermissions permissions = null)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (user == null) throw new ArgumentNullException(nameof(user));
			CheckReady();

			return SetChannelPermissions(channel, user?.Id, PermissionTarget.User, permissions?.Allow, permissions?.Deny);
		}
		public Task SetChannelRolePermissions(Channel channel, Role role, ChannelPermissions allow = null, ChannelPermissions deny = null)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (role == null) throw new ArgumentNullException(nameof(role));
			CheckReady();

			return SetChannelPermissions(channel, role?.Id, PermissionTarget.Role, allow, deny);
		}
		public Task SetChannelRolePermissions(Channel channel, Role role, DualChannelPermissions permissions = null)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (role == null) throw new ArgumentNullException(nameof(role));
			CheckReady();

			return SetChannelPermissions(channel, role?.Id, PermissionTarget.Role, permissions?.Allow, permissions?.Deny);
		}
		private Task SetChannelPermissions(Channel channel, string targetId, PermissionTarget targetType, ChannelPermissions allow = null, ChannelPermissions deny = null)
			=> _api.SetChannelPermissions(channel.Id, targetId, targetType.Value, allow?.RawValue ?? 0, deny?.RawValue ?? 0);

		public Task RemoveChannelUserPermissions(Channel channel, User user)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (user == null) throw new ArgumentNullException(nameof(user));
			CheckReady();

			return RemoveChannelPermissions(channel, user?.Id, PermissionTarget.User);
		}
		public Task RemoveChannelRolePermissions(Channel channel, Role role)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (role == null) throw new ArgumentNullException(nameof(role));
			CheckReady();

			return RemoveChannelPermissions(channel, role?.Id, PermissionTarget.Role);
		}
		private async Task RemoveChannelPermissions(Channel channel, string userOrRoleId, PermissionTarget targetType)
		{
			try
			{
				var perms = channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != userOrRoleId).FirstOrDefault();
				await _api.DeleteChannelPermissions(channel.Id, userOrRoleId).ConfigureAwait(false);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
	}
}