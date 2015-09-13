using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Collections
{
    public sealed class Channels : AsyncCollection<Channel>
    {
		internal Channels(DiscordClient client)
			: base(client) { }

		internal Channel GetOrAdd(string id, string serverId, string recipientId = null) => GetOrAdd(id, () => new Channel(_client, id, serverId, recipientId));
		internal new Channel TryRemove(string id) => base.TryRemove(id);

		protected override void OnCreated(Channel item)
		{
			item.Server.AddChannel(item.Id);
			if (item.RecipientId != null)
			{
				var user = item.Recipient;
				if (user.PrivateChannelId != null)
					throw new Exception("User already has a private channel.");
				user.PrivateChannelId = item.Id;
				item.Recipient.AddRef();
			}
		}
		protected override void OnRemoved(Channel item)
		{
			item.Server.RemoveChannel(item.Id);

			if (item.RecipientId != null)
			{
				var user = item.Recipient;
				if (user.PrivateChannelId != item.Id)
					throw new Exception("User has a different private channel.");
				user.PrivateChannelId = null;
				user.RemoveRef();
			}
        }

		internal Channel this[string id] => Get(id);
		
		internal IEnumerable<Channel> Find(string serverId, string name)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			if (name.StartsWith("#"))
			{
				string name2 = name.Substring(1);
				return this.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				return this.Where(x => x.ServerId == serverId &&
					string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			}
		}
	}
}
