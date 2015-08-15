using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public class Membership
	{
		private readonly DiscordClient _client;

		public DateTime JoinedAt;

		public bool IsMuted { get; internal set; }
		public bool IsDeafened { get; internal set; }

		public string ServerId { get; }
		public Server Server => _client.GetServer(ServerId);

		public string UserId { get; }
		public User User => _client.GetUser(UserId);
		
		public string[] RoleIds { get; internal set; }
		public IEnumerable<Role> Roles => RoleIds.Select(x => _client.GetRole(x));

		public Membership(string serverId, string userId, DateTime joinedAt, DiscordClient client)
		{
			ServerId = serverId;
			UserId = userId;
			_client = client;
			JoinedAt = joinedAt;
        }
	}
}
