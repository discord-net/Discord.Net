using Discord.API;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	public sealed class Channel : CachedObject<long>
	{
		private struct ChannelMember
		{
			public readonly User User;
			public readonly ChannelPermissions Permissions;

			public ChannelMember(User user)
			{
				User = user;
				Permissions = new ChannelPermissions();
				Permissions.Lock();
			}
		}

		public sealed class PermissionOverwrite
		{
			public PermissionTarget TargetType { get; }
			public long TargetId { get; }
			public DualChannelPermissions Permissions { get; }

			internal PermissionOverwrite(PermissionTarget targetType, long targetId, uint allow, uint deny)
			{
				TargetType = targetType;
				TargetId = targetId;
				Permissions = new DualChannelPermissions(allow, deny);
				Permissions.Lock();
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
		[JsonProperty]
		private long? ServerId { get { return _server.Id; } set { _server.Id = value; } }
		private readonly Reference<Server> _server;

		/// For private chats, returns the target user, otherwise null.
		[JsonIgnore]
		public User Recipient => _recipient.Value;
		[JsonProperty]
		private long? RecipientId { get { return _recipient.Id; } set { _recipient.Id = value; } }
		private readonly Reference<User> _recipient;

		//Collections
		/// <summary> Returns a collection of all users with read access to this channel. </summary>
		[JsonIgnore]
		public IEnumerable<User> Members
		{
			get
			{
                if (IsPrivate)
                    return _members.Values.Select(x => x.User);
                if (_client.Config.UsePermissionsCache)
                {
                    if (Type == ChannelType.Text)
                        return _members.Values.Where(x => x.Permissions.ReadMessages == true).Select(x => x.User);
                    else if (Type == ChannelType.Voice)
                        return _members.Values.Select(x => x.User).Where(x => x.VoiceChannel == this);
                }
                else
                {
                    if (Type == ChannelType.Text)
                    {
                        ChannelPermissions perms = new ChannelPermissions();
                        return Server.Members.Where(x =>
                        {
                            UpdatePermissions(x, perms);
                            return perms.ReadMessages == true;
                        });
                    }
                    else if (Type == ChannelType.Voice)
                        return Server.Members.Where(x => x.VoiceChannel == this);
                }
				return Enumerable.Empty<User>();
            }
		}
		[JsonProperty]
		private IEnumerable<long> MemberIds => Members.Select(x => x.Id);
		private ConcurrentDictionary<long, ChannelMember> _members;

		/// <summary> Returns a collection of all messages the client has seen posted in this channel. This collection does not guarantee any ordering. </summary>
		[JsonIgnore]
		public IEnumerable<Message> Messages => _messages?.Values ?? Enumerable.Empty<Message>();
		[JsonProperty]
		private IEnumerable<long> MessageIds => Messages.Select(x => x.Id);
		private readonly ConcurrentDictionary<long, Message> _messages;

		/// <summary> Returns a collection of all custom permissions used for this channel. </summary>
		private PermissionOverwrite[] _permissionOverwrites;
		public IEnumerable<PermissionOverwrite> PermissionOverwrites { get { return _permissionOverwrites; } internal set { _permissionOverwrites = value.ToArray(); } }

		/// <summary> Returns the string used to mention this channel. </summary>
		public string Mention => $"<#{Id}>";

		internal Channel(DiscordClient client, long id, long? serverId, long? recipientId)
			: base(client, id)
		{
			_server = new Reference<Server>(serverId, 
				x => _client.Servers[x], 
				x => x.AddChannel(this), 
				x => x.RemoveChannel(this));
			_recipient = new Reference<User>(recipientId, 
				x => _client.Users.GetOrAdd(x, _server.Id), 
				x =>
				{
					Name = "@" + x.Name;
					if (_server.Id == null)
						x.Global.PrivateChannel = this;
				},
				x =>
				{
					if (_server.Id == null)
						x.Global.PrivateChannel = null;
                });
			_permissionOverwrites = new PermissionOverwrite[0];
            _members = new ConcurrentDictionary<long, ChannelMember>();

            if (recipientId != null)
            {
                AddMember(client.PrivateUser);
                AddMember(Recipient);
            }

			//Local Cache
			if (client.Config.MessageCacheSize > 0)
				_messages = new ConcurrentDictionary<long, Message>();
		}
		internal override bool LoadReferences()
		{
			if (IsPrivate)
				return _recipient.Load();
			else
				return _server.Load();
		}
		internal override void UnloadReferences()
		{
			_server.Unload();
			_recipient.Unload();
			
			var globalMessages = _client.Messages;
            if (_client.Config.MessageCacheSize > 0)
            {
                var messages = _messages;
                foreach (var message in messages)
                    globalMessages.TryRemove(message.Key);
                messages.Clear();
            }
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
				UpdatePermissions();
            }
		}

		internal void AddMessage(Message message)
		{
			//Race conditions are okay here - it just means the queue will occasionally go higher than the requested cache size, and fixed later.
			var cacheLength = _client.Config.MessageCacheSize;
			if (cacheLength > 0)
			{
				var oldestIds = _messages.Where(x => x.Value.Timestamp < message.Timestamp).Select(x => x.Key).OrderBy(x => x).Take(_messages.Count - cacheLength);
				foreach (var id in oldestIds)
				{
					Message removed;
					if (_messages.TryRemove(id, out removed))
						_client.Messages.TryRemove(id);
				}
				_messages.TryAdd(message.Id, message);
			}
		}
        internal void RemoveMessage(Message message)
        {
            if (_client.Config.MessageCacheSize > 0)
                _messages.TryRemove(message.Id, out message);
        }
		
		internal void AddMember(User user)
        {
            if (!_client.Config.UsePermissionsCache)
                return;

            var member = new ChannelMember(user);
            if (_members.TryAdd(user.Id, member))
				UpdatePermissions(user, member.Permissions);
        }
		internal void RemoveMember(User user)
        {
            if (!_client.Config.UsePermissionsCache)
                return;

            ChannelMember ignored;
			_members.TryRemove(user.Id, out ignored);
		}

        internal ChannelPermissions GetPermissions(User user)
        {
            if (_client.Config.UsePermissionsCache)
            {
                ChannelMember member;
                if (_members.TryGetValue(user.Id, out member))
                    return member.Permissions;
                else
                    return null;
            }
            else
            {
                ChannelPermissions perms = new ChannelPermissions();
                UpdatePermissions(user, perms);
                return perms;
            }
		}
		internal void UpdatePermissions()
        {
            if (!_client.Config.UsePermissionsCache)
                return;

            foreach (var pair in _members)
            {
                ChannelMember member = pair.Value;
                UpdatePermissions(member.User, member.Permissions);
            }
		}
        internal void UpdatePermissions(User user)
        {
            if (!_client.Config.UsePermissionsCache)
                return;

            ChannelMember member;
            if (_members.TryGetValue(user.Id, out member))
                UpdatePermissions(member.User, member.Permissions);
        }
        internal void UpdatePermissions(User user, ChannelPermissions permissions)
		{
			uint newPermissions = 0;
			var server = Server;

			//Load the mask of all permissions supported by this channel type
			var mask = ChannelPermissions.All(this).RawValue;

			if (server != null)
			{
				//Start with this user's server permissions
				newPermissions = server.GetPermissions(user).RawValue;

				if (IsPrivate || user.IsOwner)
					newPermissions = mask; //Owners always have all permissions
				else
				{
					var channelOverwrites = PermissionOverwrites;

					var roles = user.Roles;
					foreach (var denyRole in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Role && x.Permissions.Deny.RawValue != 0 && roles.Any(y => y.Id == x.TargetId)))
						newPermissions &= ~denyRole.Permissions.Deny.RawValue;
					foreach (var allowRole in channelOverwrites.Where(x => x.TargetType == PermissionTarget.Role && x.Permissions.Allow.RawValue != 0 && roles.Any(y => y.Id == x.TargetId)))
						newPermissions |= allowRole.Permissions.Allow.RawValue;
					foreach (var denyUser in channelOverwrites.Where(x => x.TargetType == PermissionTarget.User && x.TargetId == Id && x.Permissions.Deny.RawValue != 0))
						newPermissions &= ~denyUser.Permissions.Deny.RawValue;
					foreach (var allowUser in channelOverwrites.Where(x => x.TargetType == PermissionTarget.User && x.TargetId == Id && x.Permissions.Allow.RawValue != 0))
						newPermissions |= allowUser.Permissions.Allow.RawValue;

					if (BitHelper.GetBit(newPermissions, (int)PermissionsBits.ManageRolesOrPermissions))
						newPermissions = mask; //ManageRolesOrPermissions gives all permisions
					else if (Type == ChannelType.Text && !BitHelper.GetBit(newPermissions, (int)PermissionsBits.ReadMessages))
						newPermissions = 0; //No read permission on a text channel removes all other permissions
					else
						newPermissions &= mask; //Ensure we didnt get any permissions this channel doesnt support (from serverPerms, for example)
				}
			}
			else
				newPermissions = mask; //Private messages always have all permissions

            if (newPermissions != permissions.RawValue)
                permissions.SetRawValueInternal(newPermissions);
		}

		public override bool Equals(object obj) => obj is Channel && (obj as Channel).Id == Id;
		public override int GetHashCode() => unchecked(Id.GetHashCode() + 5658);
		public override string ToString() => Name ?? IdConvert.ToString(Id);
	}
}
