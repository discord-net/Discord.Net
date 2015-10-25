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
		private readonly ConcurrentDictionary<string, bool> _bans;
		private readonly ConcurrentDictionary<string, Channel> _channels;
		private readonly ConcurrentDictionary<string, User> _members;
		private readonly ConcurrentDictionary<string, Role> _roles;
		private readonly ConcurrentDictionary<string, Invite> _invites;

		private string _ownerId;
		
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get; private set; }
		/// <summary> Returns the current logged-in user's data for this server. </summary>
		public User CurrentMember { get; internal set; }
		/// <summary> Returns true if this is a virtual server used by Discord.Net and not a real Discord server. </summary>
		public bool IsVirtual { get; internal set; }

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

		/// <summary> Returns the id of the AFK voice channel for this server (see AFKTimeout). </summary>
		public string AFKChannelId { get; private set; }
		/// <summary> Returns the AFK voice channel for this server (see AFKTimeout). </summary>
		[JsonIgnore]
		public Channel AFKChannel => _client.Channels[AFKChannelId];

		/// <summary> Returns the id of the default channel for this server. </summary>
		public string DefaultChannelId => Id;
		/// <summary> Returns the default channel for this server. </summary>
		[JsonIgnore]
		public Channel DefaultChannel => _client.Channels[DefaultChannelId];
		
		/// <summary> Returns a collection of the ids of all users banned on this server. </summary>
		[JsonIgnore]
		public IEnumerable<string> Bans => _bans.Select(x => x.Key);
		
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> Channels => _channels.Select(x => _client.Channels[x.Key]);
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> TextChannels => _channels.Select(x => _client.Channels[x.Key]).Where(x => x.Type == ChannelType.Text);
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> VoiceChannels => _channels.Select(x => _client.Channels[x.Key]).Where(x => x.Type == ChannelType.Voice);
		
		/// <summary> Returns a collection of all invites to this server. </summary>
		[JsonIgnore]
		public IEnumerable<Invite> Invites => _invites.Values;
		
		/// <summary> Returns a collection of all users within this server with their server-specific data. </summary>
		[JsonIgnore]
		public IEnumerable<User> Members => _members.Select(x => _client.Members[x.Key, Id]);

		/// <summary> Return the id of the role representing all users in a server. </summary>
		public string EveryoneRoleId => Id;
		/// <summary> Return the the role representing all users in a server. </summary>
		[JsonIgnore]
		public Role EveryoneRole => _client.Roles[EveryoneRoleId];
		/// <summary> Returns a collection of all roles within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Role> Roles => _roles.Select(x => _client.Roles[x.Key]);

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
		internal override void OnCached() { }
		internal override void OnUncached()
		{
			//Global Cache
			var channels = _client.Channels;
			foreach (var channel in _channels)
				channels.TryRemove(channel.Key);

			var members = _client.Members;
			foreach (var user in _members)
				members.TryRemove(user.Key, Id);

			var roles = _client.Roles;
			foreach (var role in _roles)
				roles.TryRemove(role.Key);

			//Local Cache
			foreach (var invite in _invites)
				invite.Value.Uncache();
			_invites.Clear();

			_bans.Clear();
        }

		internal void Update(GuildInfo model)
		{
			//Can be null
			AFKChannelId = model.AFKChannelId;

			if (model.AFKTimeout != null)
				AFKTimeout = model.AFKTimeout.Value;
			if (model.JoinedAt != null)
				JoinedAt = model.JoinedAt.Value;
			if (model.Name != null)
				Name = model.Name;
			if (model.OwnerId != null && _ownerId != model.OwnerId)
			{
				_ownerId = model.OwnerId;
				Owner = _client.Members[_ownerId, Id];
			}
			if (model.Region != null)
				Region = model.Region;

			if (model.Roles != null)
			{
				var roles = _client.Roles;
				foreach (var subModel in model.Roles)
				{
					var role = roles.GetOrAdd(subModel.Id, Id);
					role.Update(subModel);
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

			var users = _client.Users;
			var members = _client.Members;
			foreach (var subModel in model.Members)
			{
				var member = members.GetOrAdd(subModel.User.Id, Id);
				member.Update(subModel);
			}
			foreach (var subModel in model.VoiceStates)
			{
				var member = members[subModel.UserId, Id];
				if (member != null)
					member.Update(subModel);
			}
			foreach (var subModel in model.Presences)
			{
				var member = members[subModel.User.Id, Id];
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
			_channels.TryAdd(channel.Id, channel);
			foreach (var member in Members)
				member.AddChannel(channel);
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
			_members.TryAdd(member.Id, member);
			foreach (var channel in Channels)
			{
				member.AddChannel(channel);
				channel.InvalidatePermissionsCache(member.Id);
			}
        }
		internal void RemoveMember(User member)
		{
			foreach (var channel in Channels)
			{
				member.RemoveChannel(channel);
				channel.InvalidatePermissionsCache(member.Id);
			}
			_members.TryRemove(member.Id, out member);
		}
		internal void HasMember(User user) => _members.ContainsKey(user.Id);

		internal void AddRole(Role role) => _roles.TryAdd(role.Id, role);
		internal void RemoveRole(Role role) => _roles.TryRemove(role.Id, out role);
	}
}
