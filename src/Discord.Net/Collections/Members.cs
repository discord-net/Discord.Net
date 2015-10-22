namespace Discord.Collections
{
    internal sealed class Members : AsyncCollection<Member>
	{
		public Members(DiscordClient client, object writerLock)
			: base(client, writerLock) { }
		private string GetKey(string userId, string serverId)
			=> serverId + '_' + userId;

		public Member this[string userId, string serverId] 
			=> this[GetKey(userId, serverId)];
		public Member GetOrAdd(string userId, string serverId) 
			=> GetOrAdd(GetKey(userId, serverId), () => new Member(_client, userId, serverId));
		public Member TryRemove(string userId, string serverId) 
			=> TryRemove(GetKey(userId, serverId));

		protected override void OnCreated(Member item)
		{
			item.Server.AddMember(item);
			item.User.AddServer(item.ServerId);
			item.User.AddRef();
			if (item.UserId == _client.CurrentUserId)
				item.Server.CurrentMember = item;
		}
		protected override void OnRemoved(Member item)
		{
			var server = item.Server;
			if (server != null)
			{
				server.RemoveMember(item);
				if (item.UserId == _client.CurrentUserId)
					server.CurrentMember = null;
			}
			var user = item.User;
			if (user != null)
			{
				user.RemoveServer(item.ServerId);
				user.RemoveRef();
			}
		}
	}
}
