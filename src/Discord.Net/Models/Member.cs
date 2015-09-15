using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public class Member
	{
		private readonly DiscordClient _client;

		public DateTime JoinedAt { get; internal set; }

		public bool IsMuted { get; internal set; }
		public bool IsDeafened { get; internal set; }
		public bool IsSelfMuted { get; internal set; }
		public bool IsSelfDeafened { get; internal set; }
		public bool IsSuppressed { get; internal set; }

		public string SessionId { get; internal set; }
		public string Token { get; internal set; }
		/// <summary> Returns the id for the game this user is currently playing. </summary>
		public string GameId { get; internal set; }
		/// <summary> Returns the current status for this user. </summary>
		public string Status { get; internal set; }
		public DateTime StatusSince { get; internal set; }

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
		}

		public override string ToString() => UserId;

		internal void Update(Net.API.MemberInfo model)
		{
			RoleIds = model.Roles;
			if (model.JoinedAt.HasValue)
				JoinedAt = model.JoinedAt.Value;
		}
		internal void Update(Net.API.ExtendedMemberInfo model)
		{
			Update(model as Net.API.MemberInfo);
			IsDeafened = model.IsDeafened;
			IsMuted = model.IsMuted;
		}
		internal void Update(Net.API.PresenceMemberInfo model)
		{
			if (Status != model.Status)
			{
				Status = model.Status;
				StatusSince = DateTime.UtcNow;
            }
			GameId = model.GameId;
		}
		internal void Update(Net.API.VoiceMemberInfo model)
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
    }
}
