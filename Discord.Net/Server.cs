using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Server
	{
		private readonly DiscordClient _client;

		public string Id { get; }
		public string Name { get; internal set; }

		public string AFKChannelId { get; internal set; }
		public int AFKTimeout { get; internal set; }
		public DateTime JoinedAt { get; internal set; }
		public string Region { get; internal set; }
		
		public string OwnerId { get; internal set; }
		public User Owner { get { return _client.GetUser(OwnerId); } }
		public bool IsOwner { get { return _client.UserId == OwnerId; } }
		
		public string DefaultChannelId { get { return Id; } }
		public Channel DefaultChannel { get { return _client.GetChannel(DefaultChannelId); } }

		internal ConcurrentDictionary<string, bool> _members;
		public IEnumerable<User> Members { get { return _members.Keys.Select(x => _client.GetUser(x)); } }

		internal ConcurrentDictionary<string, bool> _bans;
		public IEnumerable<User> Bans { get { return _bans.Keys.Select(x => _client.GetUser(x)); } }

		public IEnumerable<Channel> Channels { get { return _client.Channels.Where(x => x.ServerId == Id); } }
		public IEnumerable<Role> Roles { get { return _client.Roles.Where(x => x.ServerId == Id); } }

		//Not Implemented
		public object Presence { get; internal set; }
		public object[] VoiceStates { get; internal set; }

		internal Server(string id, DiscordClient client)
		{
			Id = id;
			_client = client;
			_members = new ConcurrentDictionary<string, bool>();
			_bans = new ConcurrentDictionary<string, bool>();
		}

		public override string ToString()
		{
			return Name;
		}

		internal void AddMember(string id)
		{
			_members.TryAdd(id, true);
		}
		internal bool RemoveMember(string id)
		{
			bool ignored;
			return _members.TryRemove(id, out ignored);
		}

		internal void AddBan(string id)
		{
			_bans.TryAdd(id, true);
		}
		internal bool RemoveBan(string id)
		{
			bool ignored;
			return _bans.TryRemove(id, out ignored);
		}
	}
}
