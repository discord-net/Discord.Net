using Discord.API;
using Newtonsoft.Json;
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
				
		/// <summary> Returns the name of this channel. </summary>
		public string Name { get; private set; }
		/// <summary> Returns the topic associated with this channel. </summary>
		public string Topic { get; private set; }
		/// <summary> Returns the position of this channel in the channel list for this server. </summary>
		public int Position { get; private set; }
		/// <summary> Returns false is this is a public chat and true if this is a private chat with another user (see Recipient). </summary>
		public bool IsPrivate => _recipient.Id != null;
		/// <summary> Returns the type of this channel (see ChannelTypes). </summary>
		public string Type { get; private set; }

		/// <summary> Returns the server containing this channel. </summary>
		[JsonIgnore]
		public Server Server => _server.Value;
		private readonly Reference<Server> _server;

		/// For private chats, returns the target user, otherwise null.
		[JsonIgnore]
		public User Recipient => _recipient.Value;
        private readonly Reference<User> _recipient;
		
		/// <summary> Returns a collection of all users with read access to this channel. </summary>
		[JsonIgnore]
		public IEnumerable<User> Members
		{
			get
			{
				if (_areMembersStale)
					UpdateMembersCache();
				return _members.Select(x => x.Value);
            }
		}
		private Dictionary<string, User> _members;
		private bool _areMembersStale;

		/// <summary> Returns a collection of all messages the client has seen posted in this channel. This collection does not guarantee any ordering. </summary>
		[JsonIgnore]
		public IEnumerable<Message> Messages => _messages.Values;
		private readonly ConcurrentDictionary<string, Message> _messages;

		/// <summary> Returns a collection of all custom permissions used for this channel. </summary>
		private static readonly PermissionOverwrite[] _initialPermissionsOverwrites = new PermissionOverwrite[0];
		private PermissionOverwrite[] _permissionOverwrites;
		public IEnumerable<PermissionOverwrite> PermissionOverwrites { get { return _permissionOverwrites; } internal set { _permissionOverwrites = value.ToArray(); } }

		internal Channel(DiscordClient client, string id, string serverId, string recipientId)
			: base(client, id)
		{
			_server = new Reference<Server>(serverId, 
				x => _client.Servers[x], 
				x => x.AddChannel(this), 
				x => x.RemoveChannel(this));
			_recipient = new Reference<User>(recipientId, 
				x => _client.Users[x, _server.Id], 
				x =>
				{
					Name = "@" + x.Name;
					x.GlobalUser.PrivateChannel = this;
				},
				x => x.GlobalUser.PrivateChannel = null);
			_permissionOverwrites = _initialPermissionsOverwrites;
			_areMembersStale = true;

			//Local Cache
			_messages = new ConcurrentDictionary<string, Message>();
		}
		internal override void LoadReferences()
		{
			if (IsPrivate)
				_recipient.Load();
			else
				_server.Load();
		}
		internal override void UnloadReferences()
		{
			_server.Unload();
			_recipient.Unload();
			
			var globalMessages = _client.Messages;
			var messages = _messages;
			foreach (var message in messages)
				globalMessages.TryRemove(message.Key);
			_messages.Clear();
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

		internal void AddMessage(Message message)
		{
			var cacheLength = _client.Config.MessageCacheLength;
			if (cacheLength > 0)
			{
				while (_messages.Count > cacheLength - 1)
				{
					var oldest = _messages.Select(x => x.Value.Id).OrderBy(x => x).FirstOrDefault();
					if (oldest != null)
					{
						if (_messages.TryRemove(oldest, out message))
							_client.Messages.TryRemove(oldest);
					}
				}
				_messages.TryAdd(message.Id, message);
			}
		}
		internal void RemoveMessage(Message message) => _messages.TryRemove(message.Id, out message);

		internal void InvalidateMembersCache()
		{
			_areMembersStale = true;
		}
		private void UpdateMembersCache()
		{
			if (_server.Id != null)
				_members = Server.Members.Where(x => x.GetPermissions(this)?.ReadMessages ?? false).ToDictionary(x => x.Id, x => x);
			else
				_members = new Dictionary<string, User>();
			_areMembersStale = false;
		}

		internal void InvalidatePermissionsCache()
		{
			UpdateMembersCache();
			foreach (var member in _members)
				member.Value.UpdateChannelPermissions(this);
		}
		internal void InvalidatePermissionsCache(Role role)
		{
			_areMembersStale = true;
			foreach (var member in role.Members)
				member.UpdateChannelPermissions(this);
		}
		internal void InvalidatePermissionsCache(User user)
		{
			_areMembersStale = true;
			user.UpdateChannelPermissions(this);
		}
	}
}
