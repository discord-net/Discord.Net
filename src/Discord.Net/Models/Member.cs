using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public class Member
	{
		private readonly DiscordClient _client;

		/// <summary> Returns the name of this user on this server. </summary>
		public string Name { get; internal set; }
		/// <summary> Returns a by-name unique identifier separating this user from others with the same name. </summary>
		public string Discriminator { get; internal set; }
		/// <summary> Returns the unique identifier for this user's current avatar. </summary>
		public string AvatarId { get; internal set; }
		/// <summary> Returns the URL to this user's current avatar. </summary>
		public string AvatarUrl => API.Endpoints.UserAvatar(UserId, AvatarId);
		/// <summary> Returns the datetime that this user joined this server. </summary>
		public DateTime JoinedAt { get; internal set; }

		public bool IsMuted { get; internal set; }
		public bool IsDeafened { get; internal set; }
		public bool IsSelfMuted { get; internal set; }
		public bool IsSelfDeafened { get; internal set; }
		public bool IsSuppressed { get; internal set; }
		public bool IsSpeaking { get; internal set; }

		public string SessionId { get; internal set; }
		public string Token { get; internal set; }

		/// <summary> Returns the id for the game this user is currently playing. </summary>
		public string GameId { get; internal set; }
		/// <summary> Returns the current status for this user. </summary>
		public string Status { get; internal set; }
		/// <summary> Returns the time this user last sent/edited a message, started typing or sent voice data in this server. </summary>
		public DateTime? LastActivityAt { get; private set; }
		/// <summary> Returns the time this user was last seen online in this server. </summary>
		public DateTime? LastOnlineAt => Status != UserStatus.Offline ? DateTime.UtcNow : _lastOnline;
		private DateTime _lastOnline;

		public string UserId { get; }
		[JsonIgnore]
		public User User => _client.Users[UserId];

		public string ServerId { get; }
		[JsonIgnore]
		public Server Server => _client.Servers[ServerId];

		public string VoiceChannelId { get; internal set; }
		[JsonIgnore]
		public Channel VoiceChannel => _client.Channels[VoiceChannelId];

		public string[] RoleIds { get; internal set; }
		[JsonIgnore]
		public IEnumerable<Role> Roles => RoleIds.Select(x => _client.Roles[x]);

		/// <summary> Returns a collection of all messages this user has sent on this server that are still in cache. </summary>
		public IEnumerable<Message> Messages => _client.Messages.Where(x => x.UserId == UserId && x.ServerId == ServerId);

		internal Member(DiscordClient client, string userId, string serverId)
		{
			_client = client;
			UserId = userId;
			ServerId = serverId;
			Status = UserStatus.Offline;
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
        }
		internal void Update(API.ExtendedMemberInfo model)
		{
			Update(model as API.MemberInfo);
			IsDeafened = model.IsDeafened;
			IsMuted = model.IsMuted;
		}
		internal void Update(API.PresenceMemberInfo model)
		{
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
			IsDeafened = model.IsDeafened;
			IsMuted = model.IsMuted;
			SessionId = model.SessionId;
			Token = model.Token;

			VoiceChannelId = model.ChannelId;
			if (model.IsSelfDeafened.HasValue)
				IsSelfDeafened = model.IsSelfDeafened.Value;
			if (model.IsSelfMuted.HasValue)
				IsSelfMuted = model.IsSelfMuted.Value;
			if (model.IsSuppressed.HasValue)
				IsSuppressed = model.IsSuppressed.Value;
		}

		internal void UpdateActivity(DateTime? activity = null)
		{
			if (LastActivityAt == null || activity > LastActivityAt.Value)
				LastActivityAt = activity ?? DateTime.UtcNow;
		}
	}
}
