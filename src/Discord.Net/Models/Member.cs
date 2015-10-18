using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public class Member
	{
		private readonly DiscordClient _client;
		private ConcurrentDictionary<string, PackedChannelPermissions> _permissions;

		/// <summary> Returns the name of this user on this server. </summary>
		public string Name { get; private set; }
		/// <summary> Returns a by-name unique identifier separating this user from others with the same name. </summary>
		public string Discriminator { get; private set; }
		/// <summary> Returns the unique identifier for this user's current avatar. </summary>
		public string AvatarId { get; private set; }
		/// <summary> Returns the URL to this user's current avatar. </summary>
		public string AvatarUrl => API.Endpoints.UserAvatar(UserId, AvatarId);
		/// <summary> Returns the datetime that this user joined this server. </summary>
		public DateTime JoinedAt { get; private set; }

		public bool IsSelfMuted { get; private set; }
		public bool IsSelfDeafened { get; private set; }
		public bool IsServerMuted { get; private set; }
		public bool IsServerDeafened { get; private set; }
		public bool IsServerSuppressed { get; private set; }
		public bool IsSpeaking { get; internal set; }

		public string SessionId { get; private set; }
		public string Token { get; private set; }

		/// <summary> Returns the id for the game this user is currently playing. </summary>
		public string GameId { get; private set; }
		/// <summary> Returns the current status for this user. </summary>
		public string Status { get; private set; }
		/// <summary> Returns the time this user last sent/edited a message, started typing or sent voice data in this server. </summary>
		public DateTime? LastActivityAt { get; private set; }
		/// <summary> Returns the time this user was last seen online in this server. </summary>
		public DateTime LastOnlineAt => Status != UserStatus.Offline ? DateTime.UtcNow : _lastOnline;
		private DateTime _lastOnline;

		public string UserId { get; }
		[JsonIgnore]
		public User User => _client.Users[UserId];

		public string ServerId { get; }
		[JsonIgnore]
		public Server Server => _client.Servers[ServerId];

		public string VoiceChannelId { get; private set; }
		[JsonIgnore]
		public Channel VoiceChannel => _client.Channels[VoiceChannelId];

		private static readonly string[] _initialRoleIds = new string[0];
		public string[] RoleIds { get; private set; }
		[JsonIgnore]
		public IEnumerable<Role> Roles => RoleIds.Select(x => _client.Roles[x]);

		/// <summary> Returns a collection of all messages this user has sent on this server that are still in cache. </summary>
		public IEnumerable<Message> Messages => _client.Messages.Where(x => x.UserId == UserId && x.ServerId == ServerId);

		/// <summary> Returns a collection of all channels this user is a member of. </summary>
		public IEnumerable<Channel> Channels => _client.Channels.Where(x => x.ServerId == ServerId && x.UserIds.Contains(UserId));

		internal Member(DiscordClient client, string userId, string serverId)
		{
			_client = client;
			UserId = userId;
			ServerId = serverId;
			Status = UserStatus.Offline;
			RoleIds = _initialRoleIds;
			_permissions = new ConcurrentDictionary<string, PackedChannelPermissions>();
		}

		public override string ToString() => UserId;

		internal void Update(API.UserReference model)
		{
			if (model.Avatar != null)
				AvatarId = model.Avatar;
			if (model.Discriminator != null)
				Discriminator = model.Discriminator;
			if (model.Username != null)
				Name = model.Username;
		}
		internal void Update(API.MemberInfo model)
		{
			if (model.User != null)
				Update(model.User);
			if (model.JoinedAt.HasValue)
				JoinedAt = model.JoinedAt.Value;

			//Set roles, with the everyone role added too
			string[] newRoles = new string[model.Roles.Length + 1];
			newRoles[0] = Server.EveryoneRoleId;
			for (int i = 0; i < model.Roles.Length; i++)
				newRoles[i + 1] = model.Roles[i];
			RoleIds = newRoles;

			UpdatePermissions();
        }
		internal void Update(API.ExtendedMemberInfo model)
		{
			Update(model as API.MemberInfo);
			if (model.IsServerDeafened != null)
				IsServerDeafened = model.IsServerDeafened.Value;
			if (model.IsServerMuted != null)
				IsServerMuted = model.IsServerMuted.Value;
		}
		internal void Update(API.PresenceMemberInfo model)
		{
			//Allows null
			if (Status != model.Status)
			{
                Status = model.Status;
				if (Status == UserStatus.Offline)
					_lastOnline = DateTime.UtcNow;
            }
			GameId = model.GameId;
		}
		internal void Update(API.VoiceMemberInfo model)
		{
			if (model.IsServerDeafened != null)
				IsServerDeafened = model.IsServerDeafened.Value;
			if (model.IsServerMuted != null)
				IsServerMuted = model.IsServerMuted.Value;
			if (model.SessionId != null)
				SessionId = model.SessionId;
			if (model.Token != null)
				Token = model.Token;

			if (model.ChannelId != null)
				VoiceChannelId = model.ChannelId;
			if (model.IsSelfDeafened != null)
				IsSelfDeafened = model.IsSelfDeafened.Value;
			if (model.IsSelfMuted != null)
				IsSelfMuted = model.IsSelfMuted.Value;
			if (model.IsServerSuppressed != null)
				IsServerSuppressed = model.IsServerSuppressed.Value;
		}

		internal void UpdateActivity(DateTime? activity = null)
		{
			if (LastActivityAt == null || activity > LastActivityAt.Value)
				LastActivityAt = activity ?? DateTime.UtcNow;
		}

		internal void AddChannel(string channelId)
		{
			var perms = new PackedChannelPermissions();
			perms.Lock();
            _permissions.TryAdd(channelId, perms);
			UpdatePermissions(channelId);
        }
		internal bool RemoveChannel(string channelId)
		{
			PackedChannelPermissions ignored;
			return _permissions.TryRemove(channelId, out ignored);
		}
		internal void UpdatePermissions()
		{
			foreach (var channel in _permissions)
				UpdatePermissions(channel.Key);
		}
		internal void UpdatePermissions(string channelId)
		{
			if (RoleIds == null) return; // We don't have all our data processed yet, this will be called again soon

            var server = Server;
			if (server == null) return;
			var channel = _client.Channels[channelId];

			PackedChannelPermissions permissions;
			if (!_permissions.TryGetValue(channelId, out permissions)) return;
			uint newPermissions = 0x0;
			uint oldPermissions = permissions.RawValue;
			
			if (UserId == server.OwnerId)
				newPermissions = PackedChannelPermissions.All.RawValue;
			else
			{
				if (channel == null) return;
				var channelOverwrites = channel.PermissionOverwrites;

				//var roles = Roles.OrderBy(x => x.Id);
				var roles = Roles;
				foreach (var serverRole in roles)
					newPermissions |= serverRole.Permissions.RawValue;
				foreach (var denyRole in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Role && x.Deny.RawValue != 0 && roles.Any(y => y.Id == x.TargetId)))
					newPermissions &= ~denyRole.Deny.RawValue;
				foreach (var allowRole in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Role && x.Allow.RawValue != 0 && roles.Any(y => y.Id == x.TargetId)))
					newPermissions |= allowRole.Allow.RawValue;
				foreach (var denyMembers in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Member && x.TargetId == UserId && x.Deny.RawValue != 0))
					newPermissions &= ~denyMembers.Deny.RawValue;
				foreach (var allowMembers in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Member && x.TargetId == UserId && x.Allow.RawValue != 0))
					newPermissions |= allowMembers.Allow.RawValue;
			}

			permissions.SetRawValueInternal(newPermissions);

			if (permissions.General_ManagePermissions)
				permissions.SetRawValueInternal(PackedChannelPermissions.All.RawValue);
			else if (server.DefaultChannelId == channelId)
				permissions.SetBitInternal(PackedPermissions.Text_ReadMessagesBit, true);

			if (permissions.RawValue != oldPermissions)
				channel.InvalidMembersCache();
		}
		//TODO: Add GetServerPermissions
		public PackedChannelPermissions GetPermissions(Channel channel)
			=> GetPermissions(channel?.Id);
        public PackedChannelPermissions GetPermissions(string channelId)
		{
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			PackedChannelPermissions perms;
			if (_permissions.TryGetValue(channelId, out perms))
				return perms;
			return null;
		}
	}
}
