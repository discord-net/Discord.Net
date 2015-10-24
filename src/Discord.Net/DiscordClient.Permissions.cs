using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		public Task SetChannelUserPermissions(Channel channel, Member member, ChannelPermissions allow = null, ChannelPermissions deny = null)
			=> SetChannelPermissions(channel, member?.Id, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(Channel channel, Member member, DualChannelPermissions permissions = null)
			=> SetChannelPermissions(channel, member?.Id, PermissionTarget.Member, permissions?.Allow, permissions?.Deny);
		public Task SetChannelRolePermissions(Channel channel, Role role, ChannelPermissions allow = null, ChannelPermissions deny = null)
			=> SetChannelPermissions(channel, role?.Id, PermissionTarget.Role, allow, deny);
		public Task SetChannelRolePermissions(Channel channel, Role role, DualChannelPermissions permissions = null)
			=> SetChannelPermissions(channel, role?.Id, PermissionTarget.Role, permissions?.Allow, permissions?.Deny);
		private async Task SetChannelPermissions(Channel channel, string targetId, string targetType, ChannelPermissions allow = null, ChannelPermissions deny = null)
		{
			CheckReady();
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (targetId == null) throw new ArgumentNullException(nameof(targetId));
			if (targetType == null) throw new ArgumentNullException(nameof(targetType));

			uint allowValue = allow?.RawValue ?? 0;
			uint denyValue = deny?.RawValue ?? 0;
			bool changed = false;

			var perms = channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != targetId).FirstOrDefault();
			if (allowValue != 0 || denyValue != 0)
			{
				await _api.SetChannelPermissions(channel.Id, targetId, targetType, allowValue, denyValue);
				if (perms != null)
				{
					perms.Allow.SetRawValueInternal(allowValue);
					perms.Deny.SetRawValueInternal(denyValue);
				}
				else
				{
					var oldPerms = channel._permissionOverwrites;
					var newPerms = new Channel.PermissionOverwrite[oldPerms.Length + 1];
					Array.Copy(oldPerms, newPerms, oldPerms.Length);
					newPerms[oldPerms.Length] = new Channel.PermissionOverwrite(targetType, targetId, allowValue, denyValue);
					channel._permissionOverwrites = newPerms;
				}
				changed = true;
			}
			else
			{
				try
				{
					await _api.DeleteChannelPermissions(channel.Id, targetId);
					if (perms != null)
					{
						channel._permissionOverwrites = channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != targetId).ToArray();
						changed = true;
					}
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			}

			if (changed)
			{
				if (targetType == PermissionTarget.Role)
					channel.InvalidatePermissionsCache();
				else if (targetType == PermissionTarget.Member)
					channel.InvalidatePermissionsCache(targetId);
			}
		}

		public Task RemoveChannelUserPermissions(Channel channel, Member member)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (member == null) throw new ArgumentNullException(nameof(member));
			CheckReady();

			return RemoveChannelPermissions(channel, member?.Id, PermissionTarget.Member);
		}
		public Task RemoveChannelRolePermissions(Channel channel, Role role)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (role == null) throw new ArgumentNullException(nameof(role));
			CheckReady();

			return RemoveChannelPermissions(channel, role?.Id, PermissionTarget.Role);
		}
		private async Task RemoveChannelPermissions(Channel channel, string userOrRoleId, string idType)
		{
			try
			{
				var perms = channel.PermissionOverwrites.Where(x => x.TargetType != idType || x.TargetId != userOrRoleId).FirstOrDefault();
				await _api.DeleteChannelPermissions(channel.Id, userOrRoleId).ConfigureAwait(false);
				if (perms != null)
				{
					channel.PermissionOverwrites.Where(x => x.TargetType != idType || x.TargetId != userOrRoleId).ToArray();

					if (idType == PermissionTarget.Role)
						channel.InvalidatePermissionsCache();
					else if (idType == PermissionTarget.Member)
						channel.InvalidatePermissionsCache(userOrRoleId);
				}
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
	}
}