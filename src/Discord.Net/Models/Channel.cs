using Discord.API.Client;
using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            {
                Recipient.Update(model.Recipient);
                Name = $"@{Recipient}";
            }

            if (model.PermissionOverwrites != null)
            {
                _permissionOverwrites = model.PermissionOverwrites
                    .Select(x => new PermissionOverwrite(PermissionTarget.FromString(x.Type), x.Id, x.Allow, x.Deny))
                    .ToDictionary(x => x.TargetId);
                UpdatePermissions();
            }
        }

        /// <summary> Edits this channel, changing only non-null attributes. </summary>
        public async Task Edit(string name = null, string topic = null, int? position = null)
        {
            if (name != null || topic != null)
            {
                var request = new UpdateChannelRequest(Id)
                {
                    Name = name ?? Name,
                    Topic = topic ?? Topic,
                    Position = Position
                };
                await Client.ClientAPI.Send(request).ConfigureAwait(false);
            }

            if (position != null)
            {
                Channel[] channels = Server.Channels.Where(x => x.Type == Type).OrderBy(x => x.Position).ToArray();
                int oldPos = Array.IndexOf(channels, this);
                var newPosChannel = channels.Where(x => x.Position > position).FirstOrDefault();
                int newPos = (newPosChannel != null ? Array.IndexOf(channels, newPosChannel) : channels.Length) - 1;
                if (newPos < 0)
                    newPos = 0;
                int minPos;

                if (oldPos < newPos) //Moving Down
                {
                    minPos = oldPos;
                    for (int i = oldPos; i < newPos; i++)
                        channels[i] = channels[i + 1];
                    channels[newPos] = this;
                }
                else //(oldPos > newPos) Moving Up
                {
                    minPos = newPos;
                    for (int i = oldPos; i > newPos; i--)
                        channels[i] = channels[i - 1];
                    channels[newPos] = this;
                }
                Channel after = minPos > 0 ? channels.Skip(minPos - 1).FirstOrDefault() : null;
                await Server.ReorderChannels(channels.Skip(minPos), after).ConfigureAwait(false);
            }
        }
        
        public async Task Delete()
        {
            try { await Client.ClientAPI.Send(new DeleteChannelRequest(Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        #region Invites
        /// <summary> Gets all active (non-expired) invites to this server. </summary>
        public async Task<IEnumerable<Invite>> GetInvites()
            => (await Server.GetInvites()).Where(x => x.Channel.Id == Id);

        /// <summary> Creates a new invite to this channel. </summary>
        /// <param name="maxAge"> Time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        public async Task<Invite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool tempMembership = false, bool withXkcd = false)
        {
            if (maxAge < 0) throw new ArgumentOutOfRangeException(nameof(maxAge));
            if (maxUses < 0) throw new ArgumentOutOfRangeException(nameof(maxUses));

            var request = new CreateInviteRequest(Id)
            {
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                IsTemporary = tempMembership,
                WithXkcdPass = withXkcd
            };

            var response = await Client.ClientAPI.Send(request).ConfigureAwait(false);
            var invite = new Invite(Client, response.Code, response.XkcdPass);
            return invite;
        }
        #endregion

        #region Messages
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
            if (Client.Config.MessageCacheSize > 0)
            {
                Message result;
                _messages.TryGetValue(id, out result);
                return result;
            }
            return null;
        }
        public async Task<Message[]> DownloadMessages(int limit = 100, ulong? relativeMessageId = null, 
            RelativeDirection relativeDir = RelativeDirection.Before, bool useCache = true)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0 || Type != ChannelType.Text) return new Message[0];
            
            try
            {
                var request = new GetMessagesRequest(Id)
                {
                    Limit = limit,
                    RelativeDir = relativeMessageId.HasValue ? relativeDir == RelativeDirection.Before ? "before" : "after" : null,
                    RelativeId = relativeMessageId ?? 0
                };
                var msgs = await Client.ClientAPI.Send(request).ConfigureAwait(false);
                return msgs.Select(x =>
                {
                    Message msg = null;
                    if (useCache)
                    {
                        msg = AddMessage(x.Id, x.Author.Id, x.Timestamp.Value);
                        var user = msg.User;
                        if (user != null)
                            user.UpdateActivity(msg.EditedTimestamp ?? msg.Timestamp);
                    }
                    else
                        msg = new Message(x.Id, this, x.Author.Id);
                    msg.Update(x);
                    return msg;
                })
                .ToArray();
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                return new Message[0];
            }
        }

        /// <summary> Returns all members of this channel with the specified name. </summary>
        /// <remarks> Name formats supported: Name, @Name and &lt;@Id&gt;. Search is case-insensitive if exactMatch is false.</remarks>
        public IEnumerable<User> FindUsers(string name, bool exactMatch = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _users.Select(x => x.Value.User).Find(name, exactMatch: exactMatch);
        }

        public Task<Message> SendMessage(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return SendMessageInternal(text, false);
        }
        public Task<Message> SendTTSMessage(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return SendMessageInternal(text, true);
        }
        private async Task<Message> SendMessageInternal(string text, bool isTTS)
        {
            Message msg = null;
            var mentionedUsers = new List<User>();
            text = Message.CleanUserMentions(this, text, mentionedUsers);
            if (text.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {DiscordConfig.MaxMessageSize} characters or less.");

            if (Client.Config.UseMessageQueue)
                Client.MessageQueue.QueueSend(Id, text, mentionedUsers.Select(x => x.Id).ToArray(), isTTS);
            else
            {
                var request = new SendMessageRequest(Id)
                {
                    Content = text,
                    MentionedUserIds = mentionedUsers.Select(x => x.Id).ToArray(),
                    Nonce = null,
                    IsTTS = isTTS
                };
                var model = await Client.ClientAPI.Send(request).ConfigureAwait(false);
                msg = AddMessage(model.Id, model.Author.Id, model.Timestamp.Value);
                msg.Update(model);
            }
            return msg;
        }

        public Task<Message> SendFile(string filePath)
            => SendFile(Path.GetFileName(filePath), File.OpenRead(filePath));
        public async Task<Message> SendFile(string filename, Stream stream)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var request = new SendFileRequest(Id)
            {
                Filename = filename,
                Stream = stream
            };
            var model = await Client.ClientAPI.Send(request).ConfigureAwait(false);

            var msg = AddMessage(model.Id, model.Author.Id, model.Timestamp.Value);
            msg.Update(model);
            return msg;
        }
        #endregion

        #region Permissions
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

        [Obsolete("Use Channel.GetPermissions")]
        public DualChannelPermissions GetPermissionsRule(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return PermissionOverwrites
                .Where(x => x.TargetType == PermissionTarget.User && x.TargetId == user.Id)
                .Select(x => x.Permissions)
                .FirstOrDefault();
        }
        [Obsolete("Use Channel.GetPermissions")]
        public DualChannelPermissions GetPermissionsRule(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            return PermissionOverwrites
                .Where(x => x.TargetType == PermissionTarget.Role && x.TargetId == role.Id)
                .Select(x => x.Permissions)
                .FirstOrDefault();
        }

        [Obsolete("Use Channel.SetPermissions")]
        public Task AddPermissionsRule(User user, ChannelPermissions allow = null, ChannelPermissions deny = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return AddPermissionsRule(user.Id, PermissionTarget.User, allow, deny);
        }
        [Obsolete("Use Channel.SetPermissions")]
        public Task AddPermissionsRule(User user, DualChannelPermissions permissions = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return AddPermissionsRule(user.Id, PermissionTarget.User, permissions?.Allow, permissions?.Deny);
        }
        [Obsolete("Use Channel.SetPermissions")]
        public Task AddPermissionsRule(Role role, ChannelPermissions allow = null, ChannelPermissions deny = null)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            return AddPermissionsRule(role.Id, PermissionTarget.Role, allow, deny);
        }
        [Obsolete("Use Channel.SetPermissions")]
        public Task AddPermissionsRule(Role role, DualChannelPermissions permissions = null)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            return AddPermissionsRule(role.Id, PermissionTarget.Role, permissions?.Allow, permissions?.Deny);
        }
        private Task AddPermissionsRule(ulong targetId, PermissionTarget targetType, ChannelPermissions allow = null, ChannelPermissions deny = null)
        {
            var request = new AddChannelPermissionsRequest(Id)
            {
                TargetId = targetId,
                TargetType = targetType.Value,
                Allow = allow?.RawValue ?? 0,
                Deny = deny?.RawValue ?? 0
            };
            return Client.ClientAPI.Send(request);
        }

        [Obsolete("Use Channel.RemovePermissions")]
        public Task RemovePermissionsRule(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return RemovePermissionsRule(user.Id, PermissionTarget.User);
        }
        [Obsolete("Use Channel.RemovePermissions")]
        public Task RemovePermissionsRule(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            return RemovePermissionsRule(role.Id, PermissionTarget.Role);
        }
        private async Task RemovePermissionsRule(ulong userOrRoleId, PermissionTarget targetType)
        {
            try
            {
                var perms = PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != userOrRoleId).FirstOrDefault();
                await Client.ClientAPI.Send(new RemoveChannelPermissionsRequest(Id, userOrRoleId)).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        #endregion

        #region Users
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
            if (!Client.Config.UsePermissionsCache)
            {
                var user = Server.GetUser(id);
                ChannelPermissions perms = new ChannelPermissions();
                UpdatePermissions(user, perms);
                if (perms.ReadMessages)
                    return user;
                else
                    return null;
            }

            Member result;
            _users.TryGetValue(id, out result);
            return result.User;
        }
        #endregion

        public override bool Equals(object obj) => obj is Channel && (obj as Channel).Id == Id;
        public override int GetHashCode() => unchecked(Id.GetHashCode() + 5658);
        public override string ToString() => Name ?? Id.ToIdString();
    }
}
