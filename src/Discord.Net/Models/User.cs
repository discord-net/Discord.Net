using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Discord
{
	public sealed class User
	{
		private readonly DiscordClient _client;
		private readonly ConcurrentDictionary<string, bool> _servers;
		private int _refCount;
		//private DateTime? _lastPrivateActivity;

		/// <summary> Returns the unique identifier for this user. </summary>
		public string Id { get; }
		/// <summary> Returns the name of this user on this server. </summary>
		public string Name { get; private set; }
		/// <summary> Returns a by-name unique identifier separating this user from others with the same name. </summary>
		public string Discriminator { get; private set; }
		/// <summary> Returns the unique identifier for this user's current avatar. </summary>
		public string AvatarId { get; private set; }
		/// <summary> Returns the URL to this user's current avatar. </summary>
		public string AvatarUrl => API.Endpoints.UserAvatar(Id, AvatarId);

		/// <summary> Returns the email for this user. </summary>
		/// <remarks> This field is only ever populated for the current logged in user. </remarks>
		[JsonIgnore]
		public string Email { get; private set; }
		/// <summary> Returns if the email for this user has been verified. </summary>
		/// <remarks> This field is only ever populated for the current logged in user. </remarks>
		[JsonIgnore]
		public bool? IsVerified { get; private set; }

		/// <summary> Returns the Id of the private messaging channel with this user, if one exists. </summary>
		public string PrivateChannelId { get; internal set; }
		/// <summary> Returns the private messaging channel with this user, if one exists. </summary>
		[JsonIgnore]
		public Channel PrivateChannel => _client.Channels[PrivateChannelId];

		/// <summary> Returns a collection of all server-specific data for every server this user is a member of. </summary>
		[JsonIgnore]
		public IEnumerable<Member> Memberships => _servers.Select(x => _client.GetMember(x.Key, Id));
		/// <summary> Returns a collection of all servers this user is a member of. </summary>
		[JsonIgnore]
		public IEnumerable<Server> Servers => _servers.Select(x => _client.GetServer(x.Key));
		/// <summary> Returns a collection of the ids of all servers this user is a member of. </summary>
		[JsonIgnore]
		public IEnumerable<string> ServersIds => _servers.Select(x => x.Key);
		/// <summary> Returns a collection of all messages this user has sent that are still in cache. </summary>
		[JsonIgnore]
		public IEnumerable<Message> Messages => _client.Messages.Where(x => x.UserId == Id);

		/// <summary> Returns the id for the game this user is currently playing. </summary>
		/*public string GameId => Memberships.Where(x => x.GameId != null).Select(x => x.GameId).FirstOrDefault();
		/// <summary> Returns the current status for this user. </summary>
		public string Status => Memberships.OrderByDescending(x => x.StatusSince).Select(x => x.Status).FirstOrDefault();
		/// <summary> Returns the time this user's status was last changed. </summary>
		public DateTime StatusSince => Memberships.OrderByDescending(x => x.StatusSince).Select(x => x.StatusSince).First();
		/// <summary> Returns the time this user last sent/edited a message, started typing or sent voice data. </summary>
		public DateTime? LastActivity
		{
			get
			{
				var lastServerActivity = Memberships.OrderByDescending(x => x.LastActivity).Select(x => x.LastActivity).FirstOrDefault();
				if (lastServerActivity == null || (_lastPrivateActivity != null && _lastPrivateActivity.Value > lastServerActivity.Value))
					return _lastPrivateActivity;
				else
					return lastServerActivity;
			}
		}
		/// <summary> Returns the time this user was last seen online. </summary>
		public DateTime? LastOnline => Memberships.OrderByDescending(x => x.LastOnline).Select(x => x.LastOnline).FirstOrDefault();*/

		internal User(DiscordClient client, string id)
		{
			_client = client;
			Id = id;
			_servers = new ConcurrentDictionary<string, bool>();
        }

		internal void Update(UserReference model)
		{
			if (model.Avatar != null)
				AvatarId = model.Avatar;
			if (model.Discriminator != null)
				Discriminator = model.Discriminator;
			if (model.Username != null)
				Name = model.Username;
		}
		internal void Update(UserInfo model)
		{
			Update(model as UserReference);

			if (model.Email != null)
				Email = model.Email;
			if (model.IsVerified != null)
				IsVerified = model.IsVerified;
		}
		/*internal void UpdateActivity(DateTime? activity = null)
		{
			if (_lastPrivateActivity == null || activity > _lastPrivateActivity.Value)
				_lastPrivateActivity = activity ?? DateTime.UtcNow;
		}*/

		public override string ToString() => Name;

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
	}
}
