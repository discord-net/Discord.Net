using System;

namespace Discord.Collections
{
	public sealed class Users : AsyncCollection<User>
	{
		internal Users(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		internal User GetOrAdd(string id) => GetOrAdd(id, () => new User(_client, id));
		internal new User TryRemove(string id) => base.TryRemove(id);

		protected override void OnCreated(User item) { }
		protected override void OnRemoved(User item) { }

		internal User this[string id]
		{
			get
			{
				if (id == null) throw new ArgumentNullException(nameof(id));
				return Get(id);
			}
		}
	}
}
