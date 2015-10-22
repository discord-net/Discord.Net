using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Collections
{
    internal sealed class Servers : AsyncCollection<Server>
	{
		public Servers(DiscordClient client, object writerLock) 
			: base(client, writerLock) { }

		public Server GetOrAdd(string id) 
			=> base.GetOrAdd(id, () => new Server(_client, id));
		
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
	}
}
