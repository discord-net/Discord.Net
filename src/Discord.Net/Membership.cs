using Discord.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public class Membership
	{
		private readonly DiscordClient _client;

		public DateTime JoinedAt { get; internal set; }

		public bool IsMuted { get; internal set; }
		public bool IsDeafened { get; internal set; }
		public bool IsSelfMuted { get; internal set; }
		public bool IsSelfDeafened { get; internal set; }
		public bool IsSuppressed { get; internal set; }

		public string SessionId { get; internal set; }

		public string ServerId { get; }
		public Server Server => _client.GetServer(ServerId);

		public string UserId { get; }
		public User User => _client.GetUser(UserId);

		public string VoiceChannelId { get; internal set; }
		public Channel VoiceChannel => _client.GetChannel(VoiceChannelId);

		public string[] RoleIds { get; internal set; }
		public IEnumerable<Role> Roles => RoleIds.Select(x => _client.GetRole(x));

		public Membership(string serverId, string userId, DateTime joinedAt, DiscordClient client)
		{
			ServerId = serverId;
			UserId = userId;
			JoinedAt = joinedAt;
			_client = client;
		}

		internal void Update(ExtendedServerInfo.Membership data)
		{
			IsDeafened = data.IsDeaf;
			IsMuted = data.IsMuted;
			RoleIds = data.Roles;
		}
		internal void Update(WebSocketEvents.VoiceStateUpdate data)
		{
			VoiceChannelId = data.ChannelId;
			IsDeafened = data.IsDeafened;
			IsMuted = data.IsMuted;
			IsSelfDeafened = data.IsSelfDeafened;
			IsSelfMuted = data.IsSelfMuted;
			IsSuppressed = data.IsSuppressed;
			SessionId = data.SessionId;
		}
	}
}
