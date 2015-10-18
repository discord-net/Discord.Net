using Discord.API;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Channel
	{
		public sealed class PermissionOverwrite
		{
			public string TargetType { get; }
			public string TargetId { get; }
			public PackedChannelPermissions Allow { get; }
			public PackedChannelPermissions Deny { get; }

			internal PermissionOverwrite(string type, string targetId, uint allow, uint deny)
			{
				TargetType = type;
				TargetId = targetId;
				Allow = new PackedChannelPermissions(allow);
				Deny = new PackedChannelPermissions( deny);
				Allow.Lock();
				Deny.Lock();
			}
		}

		private readonly DiscordClient _client;
		private readonly ConcurrentDictionary<string, bool> _messages;
		private bool _areMembersStale;

		/// <summary> Returns the unique identifier for this channel. </summary>
		public string Id { get; }

		private string _name;
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get { return !IsPrivate ? $"{_name}" : $"@{Recipient.Name}"; } internal set { _name = value; } }

		/// <summary> Returns the topic associated with this channel. </summary>
		public string Topic { get; private set; }
		/// <summary> Returns the position of this channel in the channel list for this server. </summary>
		public int Position { get; private set; }
		/// <summary> Returns false is this is a public chat and true if this is a private chat with another user (see Recipient). </summary>
		public bool IsPrivate => ServerId == null;
		/// <summary> Returns the type of this channel (see ChannelTypes). </summary>
		public string Type { get; private set; }

		/// <summary> Returns the id of the server containing this channel. </summary>
		public string ServerId { get; }
		/// <summary> Returns the server containing this channel. </summary>
		[JsonIgnore]
		public Server Server => _client.Servers[ServerId];

		/// For private chats, returns the Id of the target user, otherwise null.
		public string RecipientId { get; set; }
		/// For private chats, returns the target user, otherwise null.
		[JsonIgnore]
		public User Recipient => _client.Users[RecipientId];

		/// <summary> Returns a collection of the IDs of all users with read access to this channel. </summary>
		public IEnumerable<string> UserIds
		{
			get
			{
				if (!_areMembersStale)
					return _userIds;
				
				_userIds = Server.Members.Where(x => x.GetPermissions(Id)?.Text_ReadMessages ?? false).Select(x => x.UserId).ToArray();
				_areMembersStale = false;
				return _userIds;
			}
		}
		private string[] _userIds;
		/// <summary> Returns a collection of all users with read access to this channel. </summary>
		public IEnumerable<Member> Members => UserIds.Select(x => _client.Members[x, ServerId]);
		/// <summary> Returns a collection of all users with read access to this channel. </summary>
		public IEnumerable<User> Users => UserIds.Select(x => _client.Users[x]);

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
		{
			_client = client;
			Id = id;
			ServerId = serverId;
			RecipientId = recipientId;
			_messages = new ConcurrentDictionary<string, bool>();
			_permissionOverwrites = _initialPermissionsOverwrites;
			_areMembersStale = true;
		}

		internal void Update(ChannelReference model)
		{
			if (model.Name != null)
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
					.Select(x => new PermissionOverwrite(x.Type, x.Id, x.Allow, x.Deny))
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
				member.UpdatePermissions(Id);
		}
		internal void InvalidatePermissionsCache(string userId)
		{
			_areMembersStale = true;
			var member = _client.Members[userId, ServerId];
			if (member != null)
				member.UpdatePermissions(Id);
		}
	}
}
