using Discord.API;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Discord
{
	internal sealed class GlobalUser : CachedObject
	{
		private readonly ConcurrentDictionary<string, bool> _servers;
		private int _refCount;
		
		/// <summary> Returns the email for this user. </summary>
		/// <remarks> This field is only ever populated for the current logged in user. </remarks>
		[JsonIgnore]
		public string Email { get; private set; }
		/// <summary> Returns if the email for this user has been verified. </summary>
		/// <remarks> This field is only ever populated for the current logged in user. </remarks>
		[JsonIgnore]
		public bool? IsVerified { get; private set; }

		/// <summary> Returns the private messaging channel with this user, if one exists. </summary>
		[JsonIgnore]
		public Channel PrivateChannel { get; internal set; }

		/// <summary> Returns a collection of all server-specific data for every server this user is a member of. </summary>
		[JsonIgnore]
		public IEnumerable<Member> Memberships => _servers.Select(x => _client.Members[Id, x.Key]);
		/// <summary> Returns a collection of all servers this user is a member of. </summary>
		[JsonIgnore]
		public IEnumerable<Server> Servers => _servers.Select(x => _client.Servers[x.Key]);

		internal GlobalUser(DiscordClient client, string id)
			: base(client, id)
		{
			_servers = new ConcurrentDictionary<string, bool>();
		}
		internal override void OnCached() { }
		internal override void OnUncached() { }
		
		internal void Update(UserInfo model)
		{
			if (model.Email != null)
				Email = model.Email;
			if (model.IsVerified != null)
				IsVerified = model.IsVerified;
		}

		internal void AddServer(string serverId)
		{
			_servers.TryAdd(serverId, true);
		}
		internal bool RemoveServer(string serverId)
		{
			bool ignored;
			return _servers.TryRemove(serverId, out ignored);
		}

		internal void AddRef()
		{
			Interlocked.Increment(ref _refCount);
		}
		internal void RemoveRef()
		{
			if (Interlocked.Decrement(ref _refCount) == 0)
				_client.Users.TryRemove(Id);
		}

		public override string ToString() => Id;
	}
}
