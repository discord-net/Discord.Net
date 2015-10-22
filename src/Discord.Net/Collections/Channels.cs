using System;

namespace Discord.Collections
{
    public sealed class Channels : AsyncCollection<Channel>
    {
		internal Channels(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		internal Channel GetOrAdd(string id, string serverId, string recipientId = null) => GetOrAdd(id, () => new Channel(_client, id, serverId, recipientId));
		internal new Channel TryRemove(string id) => base.TryRemove(id);

		protected override void OnCreated(Channel item)
		{
			if (!item.IsPrivate)
				item.Server.AddChannel(item.Id);
			if (item.RecipientId != null)
			{
				var user = item.Recipient;
				if (user.PrivateChannelId != null)
					throw new Exception("User already has a private channel.");
				user.PrivateChannelId = item.Id;
				user.AddRef();
			}
		}
		protected override void OnRemoved(Channel item)
		{
			if (!item.IsPrivate)
			{
				var server = item.Server;
				if (server != null)
					item.Server.RemoveChannel(item.Id);
			}
			if (item.RecipientId != null)
			{
				var user = item.Recipient;
				if (user != null)
				{
					if (user.PrivateChannelId != item.Id)
						throw new Exception("User has a different private channel.");
					user.PrivateChannelId = null;
					user.RemoveRef();
				}
			}
        }

		internal Channel this[string id]
		{
			get
			{
				if (id == null) throw new ArgumentNullException(nameof(id));
				return Get(id);
			}
		}
	}
}
