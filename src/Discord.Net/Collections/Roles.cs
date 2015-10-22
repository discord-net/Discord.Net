using System;

namespace Discord.Collections
{
    public sealed class Roles : AsyncCollection<Role>
	{
		internal Roles(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		internal Role GetOrAdd(string id, string serverId) => GetOrAdd(id, () => new Role(_client, id, serverId));
		internal new Role TryRemove(string id) => base.TryRemove(id);

		protected override void OnCreated(Role item)
		{
			item.Server.AddRole(item.Id);
		}
		protected override void OnRemoved(Role item)
		{
			var server = item.Server;
			if (server != null)
				item.Server.RemoveRole(item.Id);
		}

		internal Role this[string id]
		{
			get
			{
				if (id == null) throw new ArgumentNullException(nameof(id));
				return Get(id);
			}
		}
	}
}
