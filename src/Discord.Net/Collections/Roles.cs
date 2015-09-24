using System;
using System.Collections.Generic;
using System.Linq;

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
			item.Server.RemoveRole(item.Id);
		}

		internal Role this[string id] => Get(id);
		
		internal IEnumerable<Role> Find(string serverId, string name)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (name == null) throw new ArgumentNullException(nameof(name));

			if (name.StartsWith("@"))
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
