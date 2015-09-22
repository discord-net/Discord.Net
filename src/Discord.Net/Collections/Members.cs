using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Collections
{
    public sealed class Members : AsyncCollection<Member>
	{
		internal Members(DiscordClient client)
			: base(client) { }

		private string GetKey(string userId, string serverId) => serverId + '_' + userId;

		internal Member GetOrAdd(string userId, string serverId) => GetOrAdd(GetKey(userId, serverId), () => new Member(_client, userId, serverId));
		internal Member TryRemove(string userId, string serverId) => base.TryRemove(GetKey(userId, serverId));

		protected override void OnCreated(Member item)
		{
			item.Server.AddMember(item.UserId);
			item.User.AddServer(item.ServerId);
			item.User.AddRef();
			if (item.UserId == _client.CurrentUserId)
				item.Server.CurrentMember = item;
		}
		protected override void OnRemoved(Member item)
		{
			item.Server.RemoveMember(item.UserId);
			item.User.RemoveServer(item.ServerId);
			item.User.RemoveRef();
			if (item.UserId == _client.CurrentUserId)
				item.Server.CurrentMember = null;
		}
		
		internal Member this[string userId, string serverId]
		{
			get
			{
				if (serverId == null) throw new ArgumentNullException(nameof(serverId));
				if (userId == null) throw new ArgumentNullException(nameof(userId));

				return Get(GetKey(userId, serverId));
			}
		}
		
		internal IEnumerable<Member> Find(Server server, string name)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			if (name == null) throw new ArgumentNullException(nameof(name));

			if (name.StartsWith("@"))
			{
				string name2 = name.Substring(1);
				return server.Members.Where(x =>
				{
					var user = x.User;
					if (user == null)
						return false;
					return string.Equals(user.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(user.Name, name2, StringComparison.OrdinalIgnoreCase);
				});
			}
			else
			{
				return server.Members.Where(x =>
				{
					var user = x.User;
					if (user == null)
						return false;
					return string.Equals(x.User.Name, name, StringComparison.OrdinalIgnoreCase);
				});
			}
		}

		internal Member Find(string username, string discriminator)
		{
            if (username == null) throw new ArgumentNullException(nameof(username));
			if (discriminator == null) throw new ArgumentNullException(nameof(discriminator));

			if (username.StartsWith("@"))
				username = username.Substring(1);

			return this.Where(x =>
					string.Equals(x.Name, username, StringComparison.OrdinalIgnoreCase) &&
					x.Discriminator == discriminator
				)
				.FirstOrDefault();
		}
	}
}
