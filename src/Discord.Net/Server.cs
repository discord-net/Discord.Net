using Discord.Helpers;
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
		/// <summary> Returns the endpoint for this server's voice server. </summary>
		internal string VoiceServer { get; set; }

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

		internal AsyncCache<Membership, API.Models.MemberInfo> _members;
		/// <summary> Returns a collection of all channels within this server. </summary>
		public IEnumerable<Membership> Members => _members;

		internal ConcurrentDictionary<string, bool> _bans;
		/// <summary> Returns a collection of all users banned on this server. </summary>
		/// <remarks> Only users seen in other servers will be returned. To get a list of all users, use BanIds. </remarks>
		public IEnumerable<User> Bans => _bans.Keys.Select(x => _client.GetUser(x));
		/// <summary> Returns a collection of the ids of all users banned on this server. </summary>
		public IEnumerable<string> BanIds => _bans.Keys;

		/// <summary> Returns a collection of all channels within this server. </summary>
		public IEnumerable<Channel> Channels => _client.Channels.Where(x => x.ServerId == Id);
		/// <summary> Returns a collection of all channels within this server. </summary>
		public IEnumerable<Channel> TextChannels => _client.Channels.Where(x => x.ServerId == Id && x.Type == ChannelTypes.Text);
		/// <summary> Returns a collection of all channels within this server. </summary>
		public IEnumerable<Channel> VoiceChannels => _client.Channels.Where(x => x.ServerId == Id && x.Type == ChannelTypes.Voice);
		/// <summary> Returns a collection of all roles within this server. </summary>
		public IEnumerable<Role> Roles => _client.Roles.Where(x => x.ServerId == Id);

		internal Server(string id, DiscordClient client)
		{
			Id = id;
			_client = client;
			_bans = new ConcurrentDictionary<string, bool>();
			_members = new AsyncCache<Membership, API.Models.MemberInfo>(
				(key, parentKey) =>
				{
					if (_client.Config.EnableDebug)
						_client.RaiseOnDebugMessage(DebugMessageType.Cache, $"Created user {key} in server {parentKey}.");
                    return new Membership(parentKey, key, _client);
				},
				(member, model) =>
				{
					if (model is API.Models.PresenceMemberInfo)
					{
						var extendedModel = model as API.Models.PresenceMemberInfo;
						member.Status = extendedModel.Status;
						member.GameId = extendedModel.GameId;
					}
					if (model is API.Models.VoiceMemberInfo)
					{
						var extendedModel = model as API.Models.VoiceMemberInfo;
						member.VoiceChannelId = extendedModel.ChannelId;
						member.IsDeafened = extendedModel.IsDeafened;
						member.IsMuted = extendedModel.IsMuted;
						if (extendedModel.IsSelfDeafened.HasValue)
							member.IsSelfDeafened = extendedModel.IsSelfDeafened.Value;
						if (extendedModel.IsSelfMuted.HasValue)
							member.IsSelfMuted = extendedModel.IsSelfMuted.Value;
						member.IsSuppressed = extendedModel.IsSuppressed;
						member.SessionId = extendedModel.SessionId;
						member.Token = extendedModel.Token;
					}
					if (model is API.Models.RoleMemberInfo)
					{
						var extendedModel = model as API.Models.RoleMemberInfo;
						member.RoleIds = extendedModel.Roles;
						if (extendedModel.JoinedAt.HasValue)
							member.JoinedAt = extendedModel.JoinedAt.Value;
					}
					if (model is API.Models.InitialMemberInfo)
					{
						var extendedModel = model as API.Models.InitialMemberInfo;
						member.IsDeafened = extendedModel.IsDeafened;
						member.IsMuted = extendedModel.IsMuted;
					}
					if (_client.Config.EnableDebug)
						_client.RaiseOnDebugMessage(DebugMessageType.Cache, $"Updated user {member.User?.Name} ({member.UserId}) in server {member.Server?.Name} ({member.ServerId}).");
				},
				(member) =>
				{
					if (_client.Config.EnableDebug)
						_client.RaiseOnDebugMessage(DebugMessageType.Cache, $"Destroyed user {member.User?.Name} ({member.UserId}) in server {member.Server?.Name} ({member.ServerId}).");
				}
			);
		}

		internal Membership UpdateMember(API.Models.MemberInfo membership)
		{
			return _members.Update(membership.User?.Id ?? membership.UserId, Id, membership);
		}
		internal Membership RemoveMember(string userId)
		{
			return _members.Remove(userId);
		}
		public Membership GetMembership(User user)
			=> GetMember(user.Id);
        public Membership GetMember(string userId)
		{
			return _members[userId];
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
