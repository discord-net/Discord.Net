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
			public string Type { get; internal set; }
			public string Id { get; internal set; }
			public PackedChannelPermissions Deny { get; internal set; }
			public PackedChannelPermissions Allow { get; internal set; }
		}

		private readonly DiscordClient _client;
		private ConcurrentDictionary<string, bool> _messages;
		internal bool _areMembersStale;

		/// <summary> Returns the unique identifier for this channel. </summary>
		public string Id { get; }

		private string _name;
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get { return !IsPrivate ? $"{_name}" : $"@{Recipient.Name}"; } internal set { _name = value; } }

		/// <summary> Returns the topic associated with this channel. </summary>
		public string Topic { get; internal set; }
		/// <summary> Returns the position of this channel in the channel list for this server. </summary>
		public int Position { get; internal set; }
		/// <summary> Returns false is this is a public chat and true if this is a private chat with another user (see Recipient). </summary>
		public bool IsPrivate => ServerId == null;
		/// <summary> Returns the type of this channel (see ChannelTypes). </summary>
		public string Type { get; internal set; }

		/// <summary> Returns the id of the server containing this channel. </summary>
		public string ServerId { get; }
		/// <summary> Returns the server containing this channel. </summary>
		[JsonIgnore]
		public Server Server => _client.Servers[ServerId];

		/// For private chats, returns the Id of the target user, otherwise null.
		public string RecipientId { get; internal set; }
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
		private PermissionOverwrite[] _permissionOverwrites;
		public IEnumerable<PermissionOverwrite> PermissionOverwrites => _permissionOverwrites;

		internal Channel(DiscordClient client, string id, string serverId, string recipientId)
		{
			_client = client;
			Id = id;
			ServerId = serverId;
			RecipientId = recipientId;
			_messages = new ConcurrentDictionary<string, bool>();
			_areMembersStale = true;
        }

		internal void Update(API.ChannelReference model)
		{
			Name = model.Name;
			Type = model.Type;
		}
		internal void Update(API.ChannelInfo model)
		{
			Update(model as API.ChannelReference);
			
			Position = model.Position;

			if (model.PermissionOverwrites != null)
			{
				_permissionOverwrites = model.PermissionOverwrites.Select(x => new PermissionOverwrite
				{
					Type = x.Type,
					Id = x.Id,
					Deny = new PackedChannelPermissions(true,  x.Deny),
					Allow = new PackedChannelPermissions(true,  x.Allow)
				}).ToArray();
			}
			else
				_permissionOverwrites = null;
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
	}
}
