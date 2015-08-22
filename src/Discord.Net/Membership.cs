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
		public string Token { get; internal set; }
		/// <summary> Returns the id for the game this user is currently playing. </summary>
		public string GameId { get; internal set; }
		/// <summary> Returns the current status for this user. </summary>
		public string Status { get; internal set; }

		public string ServerId { get; }
		public Server Server => _client.GetServer(ServerId);

		public string UserId { get; }
		public User User => _client.GetUser(UserId);

		public string VoiceChannelId { get; internal set; }
		public Channel VoiceChannel => _client.GetChannel(VoiceChannelId);

		public string[] RoleIds { get; internal set; }
		public IEnumerable<Role> Roles => RoleIds.Select(x => _client.GetRole(x));

		public Membership(string serverId, string userId, DiscordClient client)
		{
			ServerId = serverId;
			UserId = userId;
			_client = client;
		}
	}
}
