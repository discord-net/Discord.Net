using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Server
	{
		private readonly DiscordClient _client;
		private ConcurrentDictionary<string, bool> _bans, _channels, _invites, _members, _roles;

		/// <summary> Returns the unique identifier for this server. </summary>
		public string Id { get; }
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get; internal set; }
		/// <summary> Returns the current logged-in user's data for this server. </summary>
		public Member CurrentMember { get; internal set; }

		/// <summary> Returns the amount of time (in seconds) a user must be inactive for until they are automatically moved to the AFK channel (see AFKChannel). </summary>
		public int AFKTimeout { get; internal set; }
		/// <summary> Returns the date and time your joined this server. </summary>
		public DateTime JoinedAt { get; internal set; }
		/// <summary> Returns the region for this server (see Regions). </summary>
		public string Region { get; internal set; }
		/*/// <summary> Returns the endpoint for this server's voice server. </summary>
		internal string VoiceServer { get; set; }*/

		/// <summary> Returns true if the current user created this server. </summary>
		public bool IsOwner => _client.CurrentUserId == OwnerId;
		/// <summary> Returns the id of the user that first created this server. </summary>
		public string OwnerId { get; internal set; }
		/// <summary> Returns the user that first created this server. </summary>
		[JsonIgnore]
		public User Owner => _client.Users[OwnerId];

		/// <summary> Returns the id of the AFK voice channel for this server (see AFKTimeout). </summary>
		public string AFKChannelId { get; internal set; }
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
		public IEnumerable<string> BanIds => _bans.Select(x => x.Key);

		/// <summary> Returns a collection of the ids of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<string> ChannelIds => _channels.Select(x => x.Key);
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> Channels => _channels.Select(x => _client.Channels[x.Key]);
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> TextChannels => _channels.Select(x => _client.Channels[x.Key]).Where(x => x.Type == ChannelTypes.Text);
		/// <summary> Returns a collection of all channels within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Channel> VoiceChannels => _channels.Select(x => _client.Channels[x.Key]).Where(x => x.Type == ChannelTypes.Voice);
		
		/// <summary> Returns a collection of all invite codes to this server. </summary>
		[JsonIgnore]
		public IEnumerable<string> InviteCodes => _invites.Select(x => x.Key);
		/*/// <summary> Returns a collection of all invites to this server. </summary>
		[JsonIgnore]
		public IEnumerable<Invite> Invites => _invites.Select(x => _client.Invites[x.Key]);*/

		/// <summary> Returns a collection of all users within this server with their server-specific data. </summary>
		[JsonIgnore]
		public IEnumerable<Member> Members => _members.Select(x => _client.Members[x.Key, Id]);
		/// <summary> Returns a collection of the ids of all users within this server. </summary>
		[JsonIgnore]
		public IEnumerable<string> UserIds => _members.Select(x => x.Key);
		/// <summary> Returns a collection of all users within this server. </summary>
		[JsonIgnore]
		public IEnumerable<User> Users => _members.Select(x => _client.Users[x.Key]);

		/// <summary> Return the id of the role representing all users in a server. </summary>
		public string EveryoneRoleId { get; private set; }
		/// <summary> Return the the role representing all users in a server. </summary>
		public Role EveryoneRole => _client.Roles[EveryoneRoleId];
		/// <summary> Returns a collection of the ids of all roles within this server. </summary>
		[JsonIgnore]
		public IEnumerable<string> RoleIds => _roles.Select(x => x.Key);
		/// <summary> Returns a collection of all roles within this server. </summary>
		[JsonIgnore]
		public IEnumerable<Role> Roles => _roles.Select(x => _client.Roles[x.Key]);

		internal Server(DiscordClient client, string id)
		{
			_client = client;
			Id = id;
			_bans = new ConcurrentDictionary<string, bool>();
			_channels = new ConcurrentDictionary<string, bool>();
			_invites = new ConcurrentDictionary<string, bool>();
			_members = new ConcurrentDictionary<string, bool>();
			_roles = new ConcurrentDictionary<string, bool>();
		}

		internal void Update(API.GuildInfo model)
		{
			AFKChannelId = model.AFKChannelId;
			AFKTimeout = model.AFKTimeout;
			if (model.JoinedAt.HasValue)
				JoinedAt = model.JoinedAt.Value;
			Name = model.Name;
			OwnerId = model.OwnerId;
			Region = model.Region;

			var roles = _client.Roles;
			bool isEveryone = true; //Assumes first role is always everyone
			foreach (var subModel in model.Roles)
			{
                var role = roles.GetOrAdd(subModel.Id, Id, isEveryone);
				role.Update(subModel);
				if (isEveryone)
					EveryoneRoleId = subModel.Id;
                isEveryone = false;
			}
		}
		internal void Update(API.ExtendedGuildInfo model)
		{
			Update(model as API.GuildInfo);

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
				var user = users.GetOrAdd(subModel.User.Id);
				var member = members.GetOrAdd(subModel.User.Id, Id);
				user.Update(subModel.User);
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

		internal void AddChannel(string channelId)
		{
			_channels.TryAdd(channelId, true);
			foreach (var member in Members)
				member.AddChannel(channelId);
		}
		internal bool RemoveChannel(string channelId)
		{
			bool ignored;
			foreach (var member in Members)
				member.RemoveChannel(channelId);
			return _channels.TryRemove(channelId, out ignored);
		}

		internal void AddInvite(string inviteId)
		{
			_invites.TryAdd(inviteId, true);
		}
		internal bool RemoveInvite(string inviteId)
		{
			bool ignored;
			return _invites.TryRemove(inviteId, out ignored);
		}

		internal void AddMember(Member member)
		{
			_members.TryAdd(member.UserId, true);
			foreach (var channel in Channels)
			{
				member.AddChannel(channel.Id);
                channel._areMembersStale = true;
			}
        }
		internal bool RemoveMember(Member member)
		{
			bool ignored;
			foreach (var channel in Channels)
			{
				member.RemoveChannel(channel.Id);
				channel._areMembersStale = true;
			}
			return _members.TryRemove(member.UserId, out ignored);
		}
		internal bool HasMember(string userId)
		{
			return _members.ContainsKey(userId);
		}

		internal void AddRole(string roleId)
		{
			_roles.TryAdd(roleId, true);
		}
		internal bool RemoveRole(string roleId)
		{
			bool ignored;
			return _roles.TryRemove(roleId, out ignored);
		}
	}
}
