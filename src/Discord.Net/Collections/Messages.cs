namespace Discord.Collections
{
	internal sealed class Messages : AsyncCollection<Message>
	{
		public Messages(DiscordClient client, object writerLock)
			: base(client, writerLock) { }
		
		public Message GetOrAdd(string id, string channelId, string userId) 
			=> GetOrAdd(id, () => new Message(_client, id, channelId, userId));

		protected override void OnCreated(Message item)
		{
			item.Channel.AddMessage(item.Id);
			item.User.AddRef();
		}
		protected override void OnRemoved(Message item)
		{
			var channel = item.Channel;
			if (channel != null)
				channel.RemoveMessage(item.Id);
			var user = item.User;
			if (user != null)
				user.RemoveRef();
		}
	}
}
