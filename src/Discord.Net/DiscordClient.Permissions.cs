using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		public Task SetChannelUserPermissions(Channel channel, User member, ChannelPermissions allow = null, ChannelPermissions deny = null)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (member == null) throw new ArgumentNullException(nameof(member));
			CheckReady();

			return SetChannelPermissions(channel, member?.Id, PermissionTarget.User, allow, deny);
		}
		public Task SetChannelUserPermissions(Channel channel, User member, DualChannelPermissions permissions = null)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (member == null) throw new ArgumentNullException(nameof(member));
			CheckReady();

			return SetChannelPermissions(channel, member?.Id, PermissionTarget.User, permissions?.Allow, permissions?.Deny);
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
		private async Task SetChannelPermissions(Channel channel, string targetId, PermissionTarget targetType, ChannelPermissions allow = null, ChannelPermissions deny = null)
		{
			uint allowValue = allow?.RawValue ?? 0;
			uint denyValue = deny?.RawValue ?? 0;
			bool changed = false;

			var perms = channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != targetId).FirstOrDefault();
			if (allowValue != 0 || denyValue != 0)
			{
				await _api.SetChannelPermissions(channel.Id, targetId, targetType.Value, allowValue, denyValue);
				if (perms != null)
				{
					perms.Allow.SetRawValueInternal(allowValue);
					perms.Deny.SetRawValueInternal(denyValue);
				}
				else
				{
					var oldPerms = channel.PermissionOverwrites.ToArray();
					var newPerms = new Channel.PermissionOverwrite[oldPerms.Length + 1];
					Array.Copy(oldPerms, newPerms, oldPerms.Length);
					newPerms[oldPerms.Length] = new Channel.PermissionOverwrite(targetType, targetId, allowValue, denyValue);
					channel.PermissionOverwrites = newPerms;
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
						channel.PermissionOverwrites = channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != targetId).ToArray();
						changed = true;
					}
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			}

			if (changed)
			{
				if (targetType == PermissionTarget.Role)
				{
					var role = _roles[targetId];
					if (role != null)
						channel.InvalidatePermissionsCache(role);
				}
				else if (targetType == PermissionTarget.User)
				{
					var user = _users[targetId, channel.Server?.Id];
					if (user != null)
						channel.InvalidatePermissionsCache(user);
				}
			}
		}

		public Task RemoveChannelUserPermissions(Channel channel, User member)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (member == null) throw new ArgumentNullException(nameof(member));
			CheckReady();

			return RemoveChannelPermissions(channel, member?.Id, PermissionTarget.User);
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
				if (perms != null)
				{
					channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != userOrRoleId).ToArray();

					if (targetType == PermissionTarget.Role)
					{
						var role = _roles[userOrRoleId];
						channel.InvalidatePermissionsCache(role);
					}
					else if (targetType == PermissionTarget.User)
					{
						var user = _users[userOrRoleId, channel.Server?.Id];
						if (user != null)
							channel.InvalidatePermissionsCache(user);
					}
				}
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
	}
}