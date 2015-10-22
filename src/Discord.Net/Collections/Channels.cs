using System;

namespace Discord.Collections
{
    internal sealed class Channels : AsyncCollection<Channel>
	{
		public Channels(DiscordClient client, object writerLock) 
			: base(client, writerLock) { }
		
		public Channel GetOrAdd(string id, string serverId, string recipientId = null) 
			=> GetOrAdd(id, () => new Channel(_client, id, serverId, recipientId));

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
	}
}
