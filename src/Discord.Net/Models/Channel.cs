using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Channel : CachedObject
	{
		public sealed class PermissionOverwrite
		{
			public PermissionTarget TargetType { get; }
			public string TargetId { get; }
			public ChannelPermissions Allow { get; }
			public ChannelPermissions Deny { get; }

			internal PermissionOverwrite(PermissionTarget targetType, string targetId, uint allow, uint deny)
			{
				TargetType = targetType;
				TargetId = targetId;
				Allow = new ChannelPermissions(allow);
				Allow.Lock();
				Deny = new ChannelPermissions(deny);
				Deny.Lock();
			}
		}
		
		private readonly ConcurrentDictionary<string, bool> _messages;
		private bool _areMembersStale;

		private readonly string _serverId, _recipientId;
		private Server _server;
		private Member _recipient;
		
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get; private set; }
		/// <summary> Returns the topic associated with this channel. </summary>
		public string Topic { get; private set; }
		/// <summary> Returns the position of this channel in the channel list for this server. </summary>
		public int Position { get; private set; }
		/// <summary> Returns false is this is a public chat and true if this is a private chat with another user (see Recipient). </summary>
		public bool IsPrivate => _recipientId != null;
		/// <summary> Returns the type of this channel (see ChannelTypes). </summary>
		public string Type { get; private set; }

		/// <summary> Returns the server containing this channel. </summary>
		[JsonIgnore]
		public Server Server => _client.Servers[_serverId];
		
		/// For private chats, returns the target user, otherwise null.
		[JsonIgnore]
		public Member Recipient => _client.Members[_recipientId, _serverId];

		/// <summary> Returns a collection of the IDs of all users with read access to this channel. </summary>
		public IEnumerable<string> UserIds
		{
			get
			{
				if (!_areMembersStale)
					return _userIds;
				
				_userIds = Server.Members.Where(x => x.GetPermissions(this)?.ReadMessages ?? false).Select(x => x.Id).ToArray();
				_areMembersStale = false;
				return _userIds;
			}
		}
		private string[] _userIds;
		/// <summary> Returns a collection of all users with read access to this channel. </summary>
		[JsonIgnore]
		public IEnumerable<Member> Members => UserIds.Select(x => _client.Members[x, _serverId]);

		/// <summary> Returns a collection of the ids of all messages the client has seen posted in this channel. This collection does not guarantee any ordering. </summary>
		[JsonIgnore]
		public IEnumerable<string> MessageIds => _messages.Select(x => x.Key);
		/// <summary> Returns a collection of all messages the client has seen posted in this channel. This collection does not guarantee any ordering. </summary>
		[JsonIgnore]
		public IEnumerable<Message> Messages => _messages.Select(x => _client.Messages[x.Key]);

		/// <summary> Returns a collection of all custom permissions used for this channel. </summary>
		private static readonly PermissionOverwrite[] _initialPermissionsOverwrites = new PermissionOverwrite[0];
		internal PermissionOverwrite[] _permissionOverwrites;
		public IEnumerable<PermissionOverwrite> PermissionOverwrites => _permissionOverwrites;

		internal Channel(DiscordClient client, string id, string serverId, string recipientId)
			: base(client, id)
		{
			_serverId = serverId ?? _client.Servers.PMServer.Id;
			_recipientId = recipientId;
			_permissionOverwrites = _initialPermissionsOverwrites;
			_areMembersStale = true;

			//Local Cache
			_messages = new ConcurrentDictionary<string, bool>();
		}
		internal override void OnCached()
		{

			if (IsPrivate)
			{
				_recipient = _client.Members[_recipientId, _serverId];
				Name = "@" + _recipient.Name;
			}
			else
			{
				_server = _client.Servers[_serverId];
				_server.AddChannel(this);
			}
		}
		internal override void OnUncached()
		{
			if (_server != null)
				_server.RemoveChannel(this);
			_server = null;

			if (_recipient != null)
				_recipient.GlobalUser.PrivateChannel = null;
			_recipient = null;
		}

		internal void Update(ChannelReference model)
		{
			if (!IsPrivate && model.Name != null)
				Name = model.Name;
			if (model.Type != null)
				Type = model.Type;
		}
		internal void Update(ChannelInfo model)
		{
			Update(model as ChannelReference);
			
			if (model.Position != null)
				Position = model.Position.Value;
			if (model.Topic != null)
				Topic = model.Topic;

			if (model.PermissionOverwrites != null)
			{
				_permissionOverwrites = model.PermissionOverwrites
					.Select(x => new PermissionOverwrite(PermissionTarget.FromString(x.Type), x.Id, x.Allow, x.Deny))
					.ToArray();
				InvalidatePermissionsCache();
            }
		}

		public override string ToString() => Name;

		internal void AddMessage(string messageId)
		{
			_messages.TryAdd(messageId, true);
		}
		internal bool RemoveMessage(string messageId)
		{
			bool ignored;
			return _messages.TryRemove(messageId, out ignored);
		}

		internal void InvalidMembersCache()
		{
			_areMembersStale = true;
		}
        internal void InvalidatePermissionsCache()
		{
			_areMembersStale = true;
			foreach (var member in Members)
				member.UpdatePermissions(this);
		}
		internal void InvalidatePermissionsCache(string userId)
		{
			_areMembersStale = true;
			var member = _client.Members[userId, _serverId];
			if (member != null)
				member.UpdatePermissions(this);
		}
	}
}
