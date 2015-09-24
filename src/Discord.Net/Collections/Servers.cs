using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Collections
{
    public sealed class Servers : AsyncCollection<Server>
	{
		internal Servers(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		internal Server GetOrAdd(string id) => base.GetOrAdd(id, () => new Server(_client, id));
		internal new Server TryRemove(string id) => base.TryRemove(id);

		protected override void OnCreated(Server item) { }
		protected override void OnRemoved(Server item)
		{
			var channels = _client.Channels;
			foreach (var channelId in item.ChannelIds)
				channels.TryRemove(channelId);

			var members = _client.Members;
			foreach (var userId in item.UserIds)
				members.TryRemove(userId, item.Id);

			var roles = _client.Roles;
			foreach (var roleId in item.RoleIds)
				roles.TryRemove(roleId);
		}

		internal Server this[string id] => Get(id);

		internal IEnumerable<Server> Find(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));

			return this.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
		}
	}
}
