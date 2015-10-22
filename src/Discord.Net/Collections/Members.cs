using System;

namespace Discord.Collections
{
    public sealed class Members : AsyncCollection<Member>
	{
		internal Members(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		private string GetKey(string userId, string serverId) => serverId + '_' + userId;

		internal Member GetOrAdd(string userId, string serverId) => GetOrAdd(GetKey(userId, serverId), () => new Member(_client, userId, serverId));
		internal Member TryRemove(string userId, string serverId) => base.TryRemove(GetKey(userId, serverId));

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
		
		internal Member this[string userId, string serverId]
		{
			get
			{
				if (serverId == null) throw new ArgumentNullException(nameof(serverId));
				if (userId == null) throw new ArgumentNullException(nameof(userId));
				return Get(GetKey(userId, serverId));
			}
		}
	}
}
