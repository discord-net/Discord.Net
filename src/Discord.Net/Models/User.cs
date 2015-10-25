using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public class User : CachedObject
	{
		private static readonly string[] _initialRoleIds = new string[0];

		internal static string GetId(string userId, string serverId) => (serverId ?? "Private") + '_' + userId;

		private ConcurrentDictionary<string, Channel> _channels;
		private ConcurrentDictionary<string, ChannelPermissions> _permissions;
		private ServerPermissions _serverPermissions;
		private string[] _roleIds;

		/// <summary> Returns a unique identifier combining this user's id with its server's. </summary>
		internal string UniqueId => GetId(Id, ServerId);
		/// <summary> Returns the name of this user on this server. </summary>
		public string Name { get; private set; }
		/// <summary> Returns a by-name unique identifier separating this user from others with the same name. </summary>
		public string Discriminator { get; private set; }
		/// <summary> Returns the unique identifier for this user's current avatar. </summary>
		public string AvatarId { get; private set; }
		/// <summary> Returns the URL to this user's current avatar. </summary>
		public string AvatarUrl => API.Endpoints.UserAvatar(Id, AvatarId);
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
		public UserStatus Status { get; private set; }
		/// <summary> Returns the time this user last sent/edited a message, started typing or sent voice data in this server. </summary>
		public DateTime? LastActivityAt { get; private set; }
		/// <summary> Returns the time this user was last seen online in this server. </summary>
		public DateTime LastOnlineAt => Status != UserStatus.Offline ? DateTime.UtcNow : _lastOnline;
		private DateTime _lastOnline;

		[JsonIgnore]
		internal GlobalUser GlobalUser => _client.GlobalUsers[Id];

		public string ServerId { get; }
		[JsonIgnore]
		public Server Server => _client.Servers[ServerId];

		public string VoiceChannelId { get; private set; }
		[JsonIgnore]
		public Channel VoiceChannel => _client.Channels[VoiceChannelId];

		[JsonIgnore]
		public IEnumerable<Role> Roles => _roleIds.Select(x => _client.Roles[x]);
		/// <summary> Returns a collection of all messages this user has sent on this server that are still in cache. </summary>
		[JsonIgnore]
		public IEnumerable<Message> Messages => _client.Messages.Where(x => x.UserId == Id && x.Server.Id == ServerId);
		/// <summary> Returns a collection of all channels this user is a member of. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> Channels => _client.Channels.Where(x => x.Server.Id == ServerId && x.Members == this);

		internal User(DiscordClient client, string id, string serverId)
			: base(client, id)
		{
			ServerId = serverId;
			Status = UserStatus.Offline;
			_roleIds = _initialRoleIds;
			_channels = new ConcurrentDictionary<string, Channel>();
			_permissions = new ConcurrentDictionary<string, ChannelPermissions>();
			_serverPermissions = new ServerPermissions();
        }
		internal override void OnCached()
		{
			var server = Server;
			server.AddMember(this);
			if (Id == _client.CurrentUserId)
				server.CurrentMember = this;

			var user = GlobalUser;
			if (server == null || !server.IsVirtual)
				user.AddUser(this);
		}
		internal override void OnUncached()
		{
			var server = Server;
			if (server != null)
			{
				server.RemoveMember(this);
				if (Id == _client.CurrentUserId)
					server.CurrentMember = null;
			}
			var globalUser = GlobalUser;
			if (globalUser != null)
				globalUser.RemoveUser(this);
		}

		public override string ToString() => Id;

		internal void Update(UserReference model)
		{
			if (model.Avatar != null)
				AvatarId = model.Avatar;
			if (model.Discriminator != null)
				Discriminator = model.Discriminator;
			if (model.Username != null)
				Name = model.Username;
		}
		internal void Update(MemberInfo model)
		{
			if (model.User != null)
				Update(model.User);
			if (model.JoinedAt.HasValue)
				JoinedAt = model.JoinedAt.Value;
			if (model.Roles != null)
				UpdateRoles(model.Roles);

			UpdateServerPermissions();
        }
		internal void Update(ExtendedMemberInfo model)
		{
			Update(model as API.MemberInfo);
			if (model.IsServerDeafened != null)
				IsServerDeafened = model.IsServerDeafened.Value;
			if (model.IsServerMuted != null)
				IsServerMuted = model.IsServerMuted.Value;
		}
		internal void Update(PresenceInfo model)
		{
			if (model.User != null)
				Update(model.User as UserReference);

			if (model.Roles != null)
				UpdateRoles(model.Roles);
			if (model.Status != null && Status != model.Status)
			{
				Status = UserStatus.FromString(model.Status);
				if (Status == UserStatus.Offline)
					_lastOnline = DateTime.UtcNow;
			}

			//Allows null
			GameId = model.GameId;
		}
		internal void Update(VoiceMemberInfo model)
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
		private void UpdateRoles(string[] roleIds)
		{
			//Set roles, with the everyone role added too
			string[] newRoles = new string[roleIds.Length + 1];
			newRoles[0] = ServerId; //Everyone
			for (int i = 0; i < roleIds.Length; i++)
				newRoles[i + 1] = roleIds[i];
			_roleIds = newRoles;
		}

		internal void UpdateActivity(DateTime? activity = null)
		{
			if (LastActivityAt == null || activity > LastActivityAt.Value)
				LastActivityAt = activity ?? DateTime.UtcNow;
		}
		
		internal void UpdateChannelPermissions(Channel channel)
		{
			if (_roleIds == null) return; // We don't have all our data processed yet, this will be called again soon

			var server = Server;
			if (server == null || channel.Server != server) return;

			ChannelPermissions permissions;
			if (!_permissions.TryGetValue(channel.Id, out permissions)) return;
			uint newPermissions = _serverPermissions.RawValue;
			uint oldPermissions = permissions.RawValue;
			
			if (server.Owner == this)
				newPermissions = ChannelPermissions.All(channel).RawValue;
			else
			{
				var channelOverwrites = channel.PermissionOverwrites;

				//var roles = Roles.OrderBy(x => x.Id);
				var roles = Roles;
				foreach (var denyRole in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Role && x.Deny.RawValue != 0 && roles.Any(y => y.Id == x.TargetId)))
					newPermissions &= ~denyRole.Deny.RawValue;
				foreach (var allowRole in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Role && x.Allow.RawValue != 0 && roles.Any(y => y.Id == x.TargetId)))
					newPermissions |= allowRole.Allow.RawValue;
				foreach (var denyMembers in channelOverwrites.Where(x => x.TargetType == PermissionTarget.User && x.TargetId == Id && x.Deny.RawValue != 0))
					newPermissions &= ~denyMembers.Deny.RawValue;
				foreach (var allowMembers in channelOverwrites.Where(x => x.TargetType == PermissionTarget.User && x.TargetId == Id && x.Allow.RawValue != 0))
					newPermissions |= allowMembers.Allow.RawValue;
			}

            if (BitHelper.GetBit(newPermissions, (int)PermissionsBits.ManageRolesOrPermissions))
				newPermissions = ChannelPermissions.All(channel).RawValue;

			if (newPermissions != oldPermissions)
			{
				permissions.SetRawValueInternal(newPermissions);
				channel.InvalidMembersCache();
			}

			permissions.SetRawValueInternal(newPermissions);
		}
		internal void UpdateServerPermissions()
		{
			if (_roleIds == null) return; // We don't have all our data processed yet, this will be called again soon

			var server = Server;
			if (server == null) return;
			
			uint newPermissions = 0x0;
			uint oldPermissions = _serverPermissions.RawValue;

			if (server.Owner == this)
				newPermissions = ServerPermissions.All.RawValue;
			else
			{
				//var roles = Roles.OrderBy(x => x.Id);
				var roles = Roles;
				foreach (var serverRole in roles)
					newPermissions |= serverRole.Permissions.RawValue;
			}

			if (BitHelper.GetBit(newPermissions, (int)PermissionsBits.ManageRolesOrPermissions))
				newPermissions = ServerPermissions.All.RawValue;

			if (newPermissions != oldPermissions)
			{
				_serverPermissions.SetRawValueInternal(newPermissions);
				foreach (var channel in _channels)
					UpdateChannelPermissions(channel.Value);
			}
		}
		public ServerPermissions GetPermissions() => _serverPermissions;
		public ChannelPermissions GetPermissions(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));

			ChannelPermissions perms;
			if (_permissions.TryGetValue(channel.Id, out perms))
				return perms;
			return null;
		}

		internal void AddChannel(Channel channel)
		{
			var perms = new ChannelPermissions();
			perms.Lock();
			_channels.TryAdd(channel.Id, channel);
			_permissions.TryAdd(channel.Id, perms);
			UpdateChannelPermissions(channel);
		}
		internal void RemoveChannel(Channel channel)
		{
			ChannelPermissions ignored;
			_channels.TryRemove(channel.Id, out channel);
			_permissions.TryRemove(channel.Id, out ignored);
		}

		public bool HasRole(Role role)
		{
			if (role == null) throw new ArgumentNullException(nameof(role));

			return _roleIds.Contains(role.Id);
		}
	}
}