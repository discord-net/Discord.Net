using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Models
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

		internal ConcurrentDictionary<string, bool> _members;
		public IEnumerable<string> MemberIds { get { return _members.Keys; } }
		[JsonIgnore]
		public IEnumerable<User> Members { get { return _members.Keys.Select(x => _client.GetUser(x)); } }

		internal ConcurrentDictionary<string, bool> _channels;
		public IEnumerable<string> ChannelIds { get { return _channels.Keys; } }
		[JsonIgnore]
		public IEnumerable<Channel> Channels { get { return _channels.Keys.Select(x => _client.GetChannel(x)); } }

		//Not Implemented
		public object Presence { get; internal set; }
		public object[] Roles { get; internal set; }
		public object[] VoiceStates { get; internal set; }

		internal Server(string id, DiscordClient client)
		{
			Id = id;
			_client = client;
			_members = new ConcurrentDictionary<string, bool>();
			_channels = new ConcurrentDictionary<string, bool>();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
