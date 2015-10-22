using Discord.Net;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		public Task SetChannelUserPermissions(Channel channel, Member member, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, member?.UserId, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(string channelId, Member member, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], member?.UserId, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(Channel channel, User user, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, user?.Id, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(string channelId, User user, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], user?.Id, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(Channel channel, string userId, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, userId, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(string channelId, string userId, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], userId, PermissionTarget.Member, allow, deny);

		public Task SetChannelRolePermissions(Channel channel, Role role, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, role?.Id, PermissionTarget.Role, allow, deny);
		public Task SetChannelRolePermissions(string channelId, Role role, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], role?.Id, PermissionTarget.Role, allow, deny);
		public Task SetChannelRolePermissions(Channel channel, string userId, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, userId, PermissionTarget.Role, allow, deny);
		public Task SetChannelRolePermissions(string channelId, string userId, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], userId, PermissionTarget.Role, allow, deny);

		private async Task SetChannelPermissions(Channel channel, string targetId, string targetType, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
		{
			CheckReady();
			if (channel == null) throw new NullReferenceException(nameof(channel));
			if (targetId == null) throw new NullReferenceException(nameof(targetId));
			if (targetType == null) throw new NullReferenceException(nameof(targetType));

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
			=> RemoveChannelPermissions(channel, member?.UserId, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(string channelId, Member member)
			=> RemoveChannelPermissions(_channels[channelId], member?.UserId, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(Channel channel, User user)
			=> RemoveChannelPermissions(channel, user?.Id, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(string channelId, User user)
			=> RemoveChannelPermissions(_channels[channelId], user?.Id, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(Channel channel, string userId)
			=> RemoveChannelPermissions(channel, userId, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(string channelId, string userId)
			=> RemoveChannelPermissions(_channels[channelId], userId, PermissionTarget.Member);

		public Task RemoveChannelRolePermissions(Channel channel, Role role)
			=> RemoveChannelPermissions(channel, role?.Id, PermissionTarget.Role);
		public Task RemoveChannelRolePermissions(string channelId, Role role)
			=> RemoveChannelPermissions(_channels[channelId], role?.Id, PermissionTarget.Role);
		public Task RemoveChannelRolePermissions(Channel channel, string roleId)
			=> RemoveChannelPermissions(channel, roleId, PermissionTarget.Role);
		public Task RemoveChannelRolePermissions(string channelId, string roleId)
			=> RemoveChannelPermissions(_channels[channelId], roleId, PermissionTarget.Role);

		private async Task RemoveChannelPermissions(Channel channel, string userOrRoleId, string idType)
		{
			CheckReady();
			if (channel == null) throw new NullReferenceException(nameof(channel));
			if (userOrRoleId == null) throw new NullReferenceException(nameof(userOrRoleId));
			if (idType == null) throw new NullReferenceException(nameof(idType));

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