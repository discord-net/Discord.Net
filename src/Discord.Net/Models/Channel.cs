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
		public bool IsPrivate => _recipientId != null;
		/// <summary> Returns the type of this channel (see ChannelTypes). </summary>
		public string Type { get; private set; }

		/// <summary> Returns the server containing this channel. </summary>
		[JsonIgnore]
		public Server Server { get; private set; }
		private readonly string _serverId;

		/// For private chats, returns the target user, otherwise null.
		[JsonIgnore]
		public User Recipient { get; private set; }
		private readonly string _recipientId;
		
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
			_serverId = serverId;
			_recipientId = recipientId;
			_permissionOverwrites = _initialPermissionsOverwrites;
			_areMembersStale = true;

			//Local Cache
			_messages = new ConcurrentDictionary<string, Message>();
		}
		internal override void OnCached()
		{

			if (IsPrivate)
			{
				var recipient = _client.Users[_recipientId, _serverId];
				Name = "@" + recipient.Name;
				Recipient = recipient;
			}
			else
			{
				var server = _client.Servers[_serverId];
				server.AddChannel(this);
				Server = server;
			}
		}
		internal override void OnUncached()
		{
			var server = Server;
			if (server != null)
				server.RemoveChannel(this);
			Server = null;

			var recipient = Recipient;
			if (recipient != null)
				recipient.GlobalUser.PrivateChannel = null;
			Recipient = recipient;
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
						_client.Messages.TryRemove(oldest);
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
			_members = Server.Members.Where(x => x.GetPermissions(this)?.ReadMessages ?? false).ToDictionary(x => x.Id, x => x);
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
