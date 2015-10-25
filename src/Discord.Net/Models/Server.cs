using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Server : CachedObject
	{		
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get; private set; }
		/// <summary> Returns the current logged-in user's data for this server. </summary>
		public User CurrentMember { get; internal set; }

		/// <summary> Returns the amount of time (in seconds) a user must be inactive for until they are automatically moved to the AFK channel (see AFKChannel). </summary>
		public int AFKTimeout { get; private set; }
		/// <summary> Returns the date and time your joined this server. </summary>
		public DateTime JoinedAt { get; private set; }
		/// <summary> Returns the region for this server (see Regions). </summary>
		public string Region { get; private set; }
		/*/// <summary> Returns the endpoint for this server's voice server. </summary>
		internal string VoiceServer { get; set; }*/

		/// <summary> Returns true if the current user created this server. </summary>
		public bool IsOwner => _client.CurrentUserId == _ownerId;
		/// <summary> Returns the user that first created this server. </summary>
		[JsonIgnore]
		public User Owner { get; private set; }
		private string _ownerId;
		
		/// <summary> Returns the AFK voice channel for this server (see AFKTimeout). </summary>
		[JsonIgnore]
		public Channel AFKChannel { get; private set; }
	
		/// <summary> Returns the default channel for this server. </summary>
		[JsonIgnore]
		public Channel DefaultChannel { get; private set; }

		/// <summary> Returns a collection of the ids of all users banned on this server. </summary>
		[JsonIgnore]
		public IEnumerable<string> Bans => _bans.Select(x => x.Key);
		private ConcurrentDictionary<string, bool> _bans;
		
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> Channels => _channels.Select(x => x.Value);
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> TextChannels => _channels.Select(x => x.Value).Where(x => x.Type == ChannelType.Text);
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> VoiceChannels => _channels.Select(x => x.Value).Where(x => x.Type == ChannelType.Voice);
		private ConcurrentDictionary<string, Channel> _channels;

		/// <summary> Returns a collection of all invites to this server. </summary>
		[JsonIgnore]
		public IEnumerable<Invite> Invites => _invites.Values;
		private ConcurrentDictionary<string, Invite> _invites;

		/// <summary> Returns a collection of all users within this server with their server-specific data. </summary>
		[JsonIgnore]
		public IEnumerable<User> Members => _members.Select(x => x.Value);
		private ConcurrentDictionary<string, User> _members;

		/// <summary> Return the the role representing all users in a server. </summary>
		[JsonIgnore]
		public Role EveryoneRole { get; private set; }
		/// <summary> Returns a collection of all roles within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Role> Roles => _roles.Select(x => x.Value);
		private ConcurrentDictionary<string, Role> _roles;

		internal Server(DiscordClient client, string id)
			: base(client, id)
		{
			//Global Cache
			_channels = new ConcurrentDictionary<string, Channel>();
			_members = new ConcurrentDictionary<string, User>();
			_roles = new ConcurrentDictionary<string, Role>();

			//Local Cache
			_bans = new ConcurrentDictionary<string, bool>();
			_invites = new ConcurrentDictionary<string, Invite>();
		}
		internal override void LoadReferences() { }
		internal override void UnloadReferences()
		{
			//Global Cache
			var globalChannels = _client.Channels;
			var channels = _channels;
			foreach (var channel in channels)
				globalChannels.TryRemove(channel.Key);
			channels.Clear();

			var globalMembers = _client.Users;
			var members = _members;
			foreach (var user in members)
				globalMembers.TryRemove(user.Key, Id);
			members.Clear();

			var globalRoles = _client.Roles;
			var roles = _roles;
			foreach (var role in roles)
				globalRoles.TryRemove(role.Key);
			roles.Clear();

			//Local Cache
			var invites = _invites;
			foreach (var invite in invites)
				invite.Value.Uncache();
			invites.Clear();

			_bans.Clear();
		}

		internal void Update(GuildInfo model)
		{
			//Can be null
			AFKChannel = _client.Channels[model.AFKChannelId];

			if (model.AFKTimeout != null)
				AFKTimeout = model.AFKTimeout.Value;
			if (model.JoinedAt != null)
				JoinedAt = model.JoinedAt.Value;
			if (model.Name != null)
				Name = model.Name;
			if (model.OwnerId != null && _ownerId != model.OwnerId)
			{
				_ownerId = model.OwnerId;
				Owner = _client.Users[_ownerId, Id];
			}
			if (model.Region != null)
				Region = model.Region;

			if (model.Roles != null)
			{
				var roleCache = _client.Roles;
				foreach (var x in model.Roles)
				{
					var role = roleCache.GetOrAdd(x.Id, Id);
					role.Update(x);
                }
            }
		}
		internal void Update(ExtendedGuildInfo model)
		{
			Update(model as GuildInfo);
			
			var channels = _client.Channels;
			foreach (var subModel in model.Channels)
			{
				var channel = channels.GetOrAdd(subModel.Id, Id);
				channel.Update(subModel);
			}

			var usersCache = _client.GlobalUsers;
			var membersCache = _client.Users;
			foreach (var subModel in model.Members)
			{
				var member = membersCache.GetOrAdd(subModel.User.Id, Id);
				member.Update(subModel);
			}
			foreach (var subModel in model.VoiceStates)
			{
				var member = membersCache[subModel.UserId, Id];
				if (member != null)
					member.Update(subModel);
			}
			foreach (var subModel in model.Presences)
			{
				var member = membersCache[subModel.User.Id, Id];
				if (member != null)
					member.Update(subModel);
			}
		}

		public override string ToString() => Name;

		internal void AddBan(string banId)
		{
			_bans.TryAdd(banId, true);
		}
		internal bool RemoveBan(string banId)
		{
			bool ignored;
			return _bans.TryRemove(banId, out ignored);
		}

		internal void AddChannel(Channel channel)
		{
			if (_channels.TryAdd(channel.Id, channel))
			{
				if (channel.Id == Id)
					DefaultChannel = channel;
				foreach (var member in Members)
					member.AddChannel(channel);
			}
		}
		internal void RemoveChannel(Channel channel)
		{
			foreach (var member in Members)
				member.RemoveChannel(channel);
			_channels.TryRemove(channel.Id, out channel);
		}

		internal void AddInvite(Invite invite) => _invites.TryAdd(invite.Id, invite);
		internal void RemoveInvite(Invite invite) => _invites.TryRemove(invite.Id, out invite);

		internal void AddMember(User member)
		{
			if (_members.TryAdd(member.Id, member))
			{
				if (member.Id == _ownerId)
					Owner = member;

				foreach (var channel in Channels)
				{
					member.AddChannel(channel);
					channel.InvalidatePermissionsCache(member);
				}
			}
        }
		internal void RemoveMember(User member)
		{
			if (_members.TryRemove(member.Id, out member))
			{
				if (member.Id == _ownerId)
					Owner = null;

				foreach (var channel in Channels)
				{
					member.RemoveChannel(channel);
					channel.InvalidatePermissionsCache(member);
				}
			}
		}
		internal void HasMember(User user) => _members.ContainsKey(user.Id);

		internal void AddRole(Role role)
		{
			if (_roles.TryAdd(role.Id, role))
			{
				if (role.Id == Id)
					EveryoneRole = role;
			}
		}
		internal void RemoveRole(Role role)
		{
			if (_roles.TryRemove(role.Id, out role))
			{
				if (role.Id == Id)
					EveryoneRole = null;
			}
		}
	}
}
