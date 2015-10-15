using Discord.Helpers;

namespace Discord.Collections
{
    public sealed class Messages : AsyncCollection<Message>
	{
		private readonly MessageCleaner _msgCleaner;
		internal Messages(DiscordClient client, object writerLock)
			: base(client, writerLock)
		{
			_msgCleaner = new MessageCleaner(client);
        }

		internal Message GetOrAdd(string id, string channelId, string userId) => GetOrAdd(id, () => new Message(_client, id, channelId, userId));
		internal new Message TryRemove(string id) => base.TryRemove(id);
		internal new Message Remap(string oldKey, string newKey) => base.Remap(oldKey, newKey);

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

		internal Message this[string id] => Get(id);

		internal string CleanText(string text) => _msgCleaner.Clean(text);
	}
}
