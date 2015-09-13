using Discord.Helpers;

namespace Discord.Collections
{
    public sealed class Messages : AsyncCollection<Message>
	{
		private readonly MessageCleaner _msgCleaner;
		internal Messages(DiscordClient client)
			: base(client)
		{
			_msgCleaner = new MessageCleaner(client);
        }

		internal Message GetOrAdd(string id, string channelId) => GetOrAdd(id, () => new Message(_client, id, channelId));
		internal new Message TryRemove(string id) => base.TryRemove(id);
		internal new Message Remap(string oldKey, string newKey) => base.Remap(oldKey, newKey);

		protected override void OnCreated(Message item)
		{
			item.Channel.AddMessage(item.UserId);
			item.User.AddRef();
		}
		protected override void OnRemoved(Message item)
		{
			item.Channel.RemoveMessage(item.UserId);
			item.User.RemoveRef();
		}

		internal Message this[string id] => Get(id);

		internal string CleanText(string text) => _msgCleaner.Clean(text);
	}
}
