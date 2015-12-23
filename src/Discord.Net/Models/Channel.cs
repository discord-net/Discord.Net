using Discord.API.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using APIChannel = Discord.API.Client.Channel;

namespace Discord
{
    public sealed class Channel
    {
        private struct Member
        {
            public readonly User User;
            public readonly ChannelPermissions Permissions;
            public Member(User user)
            {
                User = user;
                Permissions = new ChannelPermissions();
                Permissions.Lock();
            }
        }

        public sealed class PermissionOverwrite
        {
            public PermissionTarget TargetType { get; }
            public ulong TargetId { get; }
            public DualChannelPermissions Permissions { get; }
            internal PermissionOverwrite(PermissionTarget targetType, ulong targetId, uint allow, uint deny)
            {
                TargetType = targetType;
                TargetId = targetId;
                Permissions = new DualChannelPermissions(allow, deny);
                Permissions.Lock();
            }
        }
        
        private readonly ConcurrentDictionary<ulong, Member> _users;
        private readonly ConcurrentDictionary<ulong, Message> _messages;
        private Dictionary<ulong, PermissionOverwrite> _permissionOverwrites;

        /// <summary> Gets the client that generated this channel object. </summary>
        internal DiscordClient Client { get; }
        /// <summary> Gets the unique identifier for this channel. </summary>
        public ulong Id { get; }
        /// <summary> Gets the server owning this channel, if this is a public chat. </summary>
        public Server Server { get; }
        /// <summary> Gets the target user, if this is a private chat. </summary>
        public User Recipient { get; }

        /// <summary> Gets the name of this channel. </summary>
        public string Name { get; private set; }
        /// <summary> Gets the topic of this channel. </summary>
        public string Topic { get; private set; }
        /// <summary> Gets the position of this channel relative to other channels in this server. </summary>
        public int Position { get; private set; }
        /// <summary> Gets the type of this channel (see ChannelTypes). </summary>
        public string Type { get; private set; }

        /// <summary> Gets true if this is a private chat with another user. </summary>
        public bool IsPrivate => Recipient != null;
        /// <summary> Gets the string used to mention this channel. </summary>
        public string Mention => $"<#{Id}>";
        /// <summary> Gets a collection of all messages the client has seen posted in this channel. This collection does not guarantee any ordering. </summary>
        public IEnumerable<Message> Messages => _messages?.Values ?? Enumerable.Empty<Message>();
        /// <summary> Gets a collection of all custom permissions used for this channel. </summary>
		public IEnumerable<PermissionOverwrite> PermissionOverwrites => _permissionOverwrites.Select(x => x.Value);

        /// <summary> Gets a collection of all users with read access to this channel. </summary>
        public IEnumerable<User> Users
        {
            get
            {
                if (IsPrivate)
                    return _users.Values.Select(x => x.User);
                if (Client.Config.UsePermissionsCache)
                {
                    if (Type == ChannelType.Text)
                        return _users.Values.Where(x => x.Permissions.ReadMessages == true).Select(x => x.User);
                    else if (Type == ChannelType.Voice)
                        return _users.Values.Select(x => x.User).Where(x => x.VoiceChannel == this);
                }
                else
                {
                    if (Type == ChannelType.Text)
                    {
                        ChannelPermissions perms = new ChannelPermissions();
                        return Server.Users.Where(x =>
                        {
                            UpdatePermissions(x, perms);
                            return perms.ReadMessages == true;
                        });
                    }
                    else if (Type == ChannelType.Voice)
                        return Server.Users.Where(x => x.VoiceChannel == this);
                }
                return Enumerable.Empty<User>();
            }
        }

        internal Channel(DiscordClient client, ulong id, Server server)
            : this(client, id)
        {
            Server = server;
        }
        internal Channel(DiscordClient client, ulong id, User recipient)
            : this(client, id)
        {
            Recipient = recipient;
            Name = $"@{recipient}";
            AddUser(client.PrivateUser);
            AddUser(recipient);
        }
        private Channel(DiscordClient client, ulong id)
        {
            Client = client;
            Id = id;

            _permissionOverwrites = new Dictionary<ulong, PermissionOverwrite>();
            _users = new ConcurrentDictionary<ulong, Member>();
            if (client.Config.MessageCacheSize > 0)
                _messages = new ConcurrentDictionary<ulong, Message>();
        }

        internal void Update(ChannelReference model)
        {
            if (!IsPrivate && model.Name != null)
                Name = model.Name;
            if (model.Type != null)
                Type = model.Type;
        }
        internal void Update(APIChannel model)
        {
            Update(model as ChannelReference);

            if (model.Position != null)
                Position = model.Position.Value;
            if (model.Topic != null)
                Topic = model.Topic;
            if (model.Recipient != null)
                Recipient.Update(model.Recipient);

            if (model.PermissionOverwrites != null)
            {
                _permissionOverwrites = model.PermissionOverwrites
                    .Select(x => new PermissionOverwrite(PermissionTarget.FromString(x.Type), x.Id, x.Allow, x.Deny))
                    .ToDictionary(x => x.TargetId);
                UpdatePermissions();
            }
        }

        //Members
        internal void AddUser(User user)
        {
            if (!Client.Config.UsePermissionsCache)
                return;

            var member = new Member(user);
            if (_users.TryAdd(user.Id, member))
                UpdatePermissions(user, member.Permissions);
        }
        internal void RemoveUser(ulong id)
        {
            if (!Client.Config.UsePermissionsCache)
                return;

            Member ignored;
            _users.TryRemove(id, out ignored);
        }
        public User GetUser(ulong id)
        {
            Member result;
            _users.TryGetValue(id, out result);
            return result.User;
        }

        //Messages
        internal Message AddMessage(ulong id, ulong userId, DateTime timestamp)
        {
            Message message = new Message(id, this, userId);
            var cacheLength = Client.Config.MessageCacheSize;
            if (cacheLength > 0)
            {
                var oldestIds = _messages
                    .Where(x => x.Value.Timestamp < timestamp)
                    .Select(x => x.Key).OrderBy(x => x)
                    .Take(_messages.Count - cacheLength);
                Message removed;
                foreach (var removeId in oldestIds)
                    _messages.TryRemove(removeId, out removed);
                return _messages.GetOrAdd(message.Id, message);
            }
            return message;
        }
        internal Message RemoveMessage(ulong id)
        {
            if (Client.Config.MessageCacheSize > 0)
            {
                Message msg;
                _messages.TryRemove(id, out msg);
                return msg;
            }
            return null;
        }
        public Message GetMessage(ulong id)
        {
            Message result;
            _messages.TryGetValue(id, out result);
            return result;
        }

        //Permissions
        internal void UpdatePermissions()
        {
            if (!Client.Config.UsePermissionsCache)
                return;

            foreach (var pair in _users)
            {
                Member member = pair.Value;
                UpdatePermissions(member.User, member.Permissions);
            }
        }
        internal void UpdatePermissions(User user)
        {
            if (!Client.Config.UsePermissionsCache)
                return;

            Member member;
            if (_users.TryGetValue(user.Id, out member))
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

                if (IsPrivate || user == Server.Owner)
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

                    if (newPermissions.HasBit((byte)PermissionsBits.ManageRolesOrPermissions))
                        newPermissions = mask; //ManageRolesOrPermissions gives all permisions
                    else if (Type == ChannelType.Text && !newPermissions.HasBit((byte)PermissionsBits.ReadMessages))
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
        internal ChannelPermissions GetPermissions(User user)
        {
            if (Client.Config.UsePermissionsCache)
            {
                Member member;
                if (_users.TryGetValue(user.Id, out member))
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

        public override bool Equals(object obj) => obj is Channel && (obj as Channel).Id == Id;
        public override int GetHashCode() => unchecked(Id.GetHashCode() + 5658);
        public override string ToString() => Name ?? Id.ToIdString();
    }
}
