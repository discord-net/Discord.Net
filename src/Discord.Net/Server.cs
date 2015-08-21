using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Server
	{
		private readonly DiscordClient _client;

		/// <summary> Returns the unique identifier for this server. </summary>
		public string Id { get; }
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get; internal set; }

		/// <summary> Returns the amount of time (in seconds) a user must be inactive for until they are automatically moved to the AFK channel (see AFKChannel). </summary>
		public int AFKTimeout { get; internal set; }
		/// <summary> Returns the date and time your joined this server. </summary>
		public DateTime JoinedAt { get; internal set; }
		/// <summary> Returns the region for this server (see Regions). </summary>
		public string Region { get; internal set; }

		/// <summary> Returns the id of the user that first created this server. </summary>
		public string OwnerId { get; internal set; }
		/// <summary> Returns the user that first created this server. </summary>
		public User Owner => _client.GetUser(OwnerId);
		/// <summary> Returns true if the current user created this server. </summary>
		public bool IsOwner => _client.UserId == OwnerId;

		/// <summary> Returns the id of the AFK voice channel for this server (see AFKTimeout). </summary>
		public string AFKChannelId { get; internal set; }
		/// <summary> Returns the AFK voice channel for this server (see AFKTimeout). </summary>
		public Channel AFKChannel => _client.GetChannel(AFKChannelId);

		/// <summary> Returns the id of the default channel for this server. </summary>
		public string DefaultChannelId => Id;
		/// <summary> Returns the default channel for this server. </summary>
		public Channel DefaultChannel =>_client.GetChannel(DefaultChannelId);

		internal ConcurrentDictionary<string, Membership> _members;
		/// <summary> Returns a collection of all channels within this server. </summary>
		public IEnumerable<Membership> Members => _members.Values;

		internal ConcurrentDictionary<string, bool> _bans;
		/// <summary> Returns a collection of all users banned on this server. </summary>
		/// <remarks> Only users seen in other servers will be returned. To get a list of all users, use BanIds. </remarks>
		public IEnumerable<User> Bans => _bans.Keys.Select(x => _client.GetUser(x));
		/// <summary> Returns a collection of the ids of all users banned on this server. </summary>
		public IEnumerable<string> BanIds => _bans.Keys;

		/// <summary> Returns a collection of all channels within this server. </summary>
		public IEnumerable<Channel> Channels => _client.Channels.Where(x => x.ServerId == Id);
		/// <summary> Returns a collection of all roles within this server. </summary>
		public IEnumerable<Role> Roles => _client.Roles.Where(x => x.ServerId == Id);

		//TODO: Not Implemented
		/// <summary> Not implemented, stored for reference. </summary>
		public object Presence { get; internal set; }
		//TODO: Not Implemented
		/// <summary> Not implemented, stored for reference. </summary>
		public object[] VoiceStates { get; internal set; }

		internal Server(string id, DiscordClient client)
		{
			Id = id;
			_client = client;
			_members = new ConcurrentDictionary<string, Membership>();
			_bans = new ConcurrentDictionary<string, bool>();
		}

		internal void AddMember(Membership membership)
		{
			_members[membership.UserId] = membership;
		}
		internal Membership RemoveMember(string userId)
		{
			Membership result = null;
			_members.TryRemove(userId, out result);
			return result;
		}
		public Membership GetMembership(User user)
			=> GetMembership(user.Id);
        public Membership GetMembership(string userId)
		{
			Membership result = null;
			_members.TryGetValue(userId, out result);
			return result;
		}

		internal void AddBan(string banId)
		{
			_bans.TryAdd(banId, true);
		}
		internal bool RemoveBan(string banId)
		{
			bool ignored;
			return _bans.TryRemove(banId, out ignored);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
