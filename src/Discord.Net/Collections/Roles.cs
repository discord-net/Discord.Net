namespace Discord.Collections
{
    internal sealed class Roles : AsyncCollection<Role>
	{
		public Roles(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		public Role GetOrAdd(string id, string serverId) 
			=> GetOrAdd(id, () => new Role(_client, id, serverId));

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
	}
}
