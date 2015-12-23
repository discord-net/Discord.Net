namespace Discord
{
    /*public enum RelativeDirection { Before, After }
    public partial class DiscordClient
    {
        /// <summary> Returns the channel with the specified id, or null if none was found. </summary>
        public Channel GetChannel(ulong id)
        {
            CheckReady();

            return _channels[id];
        }

        /// <summary> Returns all channels with the specified server and name. </summary>
        /// <remarks> Name formats supported: Name, #Name and &lt;#Id&gt;. Search is case-insensitive if exactMatch is false.</remarks>
        public IEnumerable<Channel> FindChannels(Server server, string name, ChannelType type = null, bool exactMatch = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (name == null) throw new ArgumentNullException(nameof(name));
            CheckReady();

            var query = server.Channels.Where(x => string.Equals(x.Name, name, exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));

            if (!exactMatch && name.Length >= 2)
            {
                if (name[0] == '<' && name[1] == '#' && name[name.Length - 1] == '>') //Parse mention
                {
                    var id = IdConvert.ToLong(name.Substring(2, name.Length - 3));
                    var channel = _channels[id];
                    if (channel != null)
                        query = query.Concat(new Channel[] { channel });
                }
                else if (name[0] == '#' && (type == null || type == ChannelType.Text)) //If we somehow get text starting with # but isn't a mention
                {
                    string name2 = name.Substring(1);
                    query = query.Concat(server.TextChannels.Where(x => string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase)));
                }
            }

            if (type != null)
                query = query.Where(x => x.Type == type);
            return query;
        }

        /// <summary> Creates a new channel with the provided name and type. </summary>
        public async Task<Channel> CreateChannel(Server server, string name, ChannelType type)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));
            CheckReady();

            var request = new CreateChannelRequest(server.Id) { Name = name, Type = type.Value };
            var response = await _clientRest.Send(request).ConfigureAwait(false);

            var channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient?.Id);
            channel.Update(response);
            return channel;
        }

        /// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
        public async Task<Channel> CreatePMChannel(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            CheckReady();

            Channel channel = null;
            if (user != null)
                channel = user.Global.PrivateChannel;
            if (channel == null)
            {
                var request = new CreatePrivateChannelRequest() { RecipientId = user.Id };
                var response = await _clientRest.Send(request).ConfigureAwait(false);

                var recipient = _users.GetOrAdd(response.Recipient.Id, null);
                recipient.Update(response.Recipient);
                channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient.Id);
                channel.Update(response);
            }
            return channel;
        }

        /// <summary> Edits the provided channel, changing only non-null attributes. </summary>
        public async Task EditChannel(Channel channel, string name = null, string topic = null, int? position = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            CheckReady();

            if (name != null || topic != null)
            {
                var request = new UpdateChannelRequest(channel.Id)
                {
                    Name = name ?? channel.Name,
                    Topic = topic ?? channel.Topic,
                    Position = channel.Position
                };
                await _clientRest.Send(request).ConfigureAwait(false);
            }

            if (position != null)
            {
                Channel[] channels = channel.Server.Channels.Where(x => x.Type == channel.Type).OrderBy(x => x.Position).ToArray();
                int oldPos = Array.IndexOf(channels, channel);
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
                    channels[newPos] = channel;
                }
                else //(oldPos > newPos) Moving Up
                {
                    minPos = newPos;
                    for (int i = oldPos; i > newPos; i--)
                        channels[i] = channels[i - 1];
                    channels[newPos] = channel;
                }
                Channel after = minPos > 0 ? channels.Skip(minPos - 1).FirstOrDefault() : null;
                await ReorderChannels(channel.Server, channels.Skip(minPos), after).ConfigureAwait(false);
            }
        }

        /// <summary> Reorders the provided channels in the server's channel list and places them after a certain channel. </summary>
        public Task ReorderChannels(Server server, IEnumerable<Channel> channels, Channel after = null)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (channels == null) throw new ArgumentNullException(nameof(channels));
            CheckReady();

            var request = new ReorderChannelsRequest(server.Id)
            {
                ChannelIds = channels.Select(x => x.Id).ToArray(),
                StartPos = after != null ? after.Position + 1 : channels.Min(x => x.Position)
            };
            return _clientRest.Send(request);
        }

        /// <summary> Destroys the provided channel. </summary>
        public async Task DeleteChannel(Channel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            CheckReady();

            try { await _clientRest.Send(new DeleteChannelRequest(channel.Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        /// <summary> Gets more info about the provided invite code. </summary>
        /// <remarks> Supported formats: inviteCode, xkcdCode, https://discord.gg/inviteCode, https://discord.gg/xkcdCode </remarks>
        public async Task<Invite> GetInvite(string inviteIdOrXkcd)
        {
            if (inviteIdOrXkcd == null) throw new ArgumentNullException(nameof(inviteIdOrXkcd));
            CheckReady();

            //Remove trailing slash
            if (inviteIdOrXkcd.Length > 0 && inviteIdOrXkcd[inviteIdOrXkcd.Length - 1] == '/')
                inviteIdOrXkcd = inviteIdOrXkcd.Substring(0, inviteIdOrXkcd.Length - 1);
            //Remove leading URL
            int index = inviteIdOrXkcd.LastIndexOf('/');
            if (index >= 0)
                inviteIdOrXkcd = inviteIdOrXkcd.Substring(index + 1);

            var response = await _clientRest.Send(new GetInviteRequest(inviteIdOrXkcd)).ConfigureAwait(false);
            var invite = new Invite(response.Code, response.XkcdPass);
            invite.Update(response);
            return invite;
        }

        /// <summary> Gets all active (non-expired) invites to a provided server. </summary>
        public async Task<Invite[]> GetInvites(Server server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            CheckReady();

            var response = await _clientRest.Send(new GetInvitesRequest(server.Id)).ConfigureAwait(false);
            return response.Select(x =>
            {
                var invite = new Invite(x.Code, x.XkcdPass);
                invite.Update(x);
                return invite;
            }).ToArray();
        }

        /// <summary> Creates a new invite to the default channel of the provided server. </summary>
        /// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
        /// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
        /// <param name="hasXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to 0 to have unlimited uses. </param>
        public Task<Invite> CreateInvite(Server server, int maxAge = 1800, int maxUses = 0, bool tempMembership = false, bool hasXkcd = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            CheckReady();

            return CreateInvite(server.DefaultChannel, maxAge, maxUses, tempMembership, hasXkcd);
        }
        /// <summary> Creates a new invite to the provided channel. </summary>
        /// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
        /// <param name="tempMembership"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
        /// <param name="hasXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to 0 to have unlimited uses. </param>
        public async Task<Invite> CreateInvite(Channel channel, int maxAge = 1800, int maxUses = 0, bool isTemporary = false, bool withXkcd = false)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (maxAge < 0) throw new ArgumentOutOfRangeException(nameof(maxAge));
            if (maxUses < 0) throw new ArgumentOutOfRangeException(nameof(maxUses));
            CheckReady();

            var request = new CreateInviteRequest(channel.Id)
            {
                MaxAge = maxAge,
                MaxUses = maxUses,
                IsTemporary = isTemporary,
                WithXkcdPass = withXkcd
            };

            var response = await _clientRest.Send(request).ConfigureAwait(false);
            var invite = new Invite(response.Code, response.XkcdPass);
            return invite;
        }

        /// <summary> Deletes the provided invite. </summary>
        public async Task DeleteInvite(Invite invite)
        {
            if (invite == null) throw new ArgumentNullException(nameof(invite));
            CheckReady();

            try { await _clientRest.Send(new DeleteInviteRequest(invite.Code)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        /// <summary> Accepts the provided invite. </summary>
        public Task AcceptInvite(Invite invite)
        {
            if (invite == null) throw new ArgumentNullException(nameof(invite));
            CheckReady();

            return _clientRest.Send(new AcceptInviteRequest(invite.Code));
        }


        /// <summary> Returns the message with the specified id, or null if none was found. </summary>
        public Message GetMessage(ulong id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            CheckReady();

            return _messages[id];
        }

        /// <summary> Sends a message to the provided channel. To include a mention, see the Mention static helper class. </summary>
        public Task<Message> SendMessage(Channel channel, string text)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (text == null) throw new ArgumentNullException(nameof(text));
            CheckReady();

            return SendMessageInternal(channel, text, false);
        }
        /// <summary> Sends a private message to the provided user. </summary>
        public async Task<Message> SendMessage(User user, string text)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (text == null) throw new ArgumentNullException(nameof(text));
            CheckReady();

            var channel = await CreatePMChannel(user).ConfigureAwait(false);
            return await SendMessageInternal(channel, text, false).ConfigureAwait(false);
        }
        /// <summary> Sends a text-to-speech message to the provided channel. To include a mention, see the Mention static helper class. </summary>
        public Task<Message> SendTTSMessage(Channel channel, string text)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (text == null) throw new ArgumentNullException(nameof(text));
            CheckReady();

            return SendMessageInternal(channel, text, true);
        }
        /// <summary> Sends a file to the provided channel. </summary>
        public Task<Message> SendFile(Channel channel, string filePath)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            CheckReady();

            return SendFile(channel, Path.GetFileName(filePath), File.OpenRead(filePath));
        }
        /// <summary> Sends a file to the provided channel. </summary>
        public async Task<Message> SendFile(Channel channel, string filename, Stream stream)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            CheckReady();

            var request = new SendFileRequest(channel.Id)
            {
                Filename = filename,
                Stream = stream
            };
            var model = await _clientRest.Send(request).ConfigureAwait(false);

            var msg = _messages.GetOrAdd(model.Id, channel.Id, model.Author.Id);
            msg.Update(model);
            RaiseMessageSent(msg);
            return msg;
        }
        /// <summary> Sends a file to the provided channel. </summary>
        public async Task<Message> SendFile(User user, string filePath)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            CheckReady();

            var channel = await CreatePMChannel(user).ConfigureAwait(false);
            return await SendFile(channel, Path.GetFileName(filePath), File.OpenRead(filePath)).ConfigureAwait(false);
        }
        /// <summary> Sends a file to the provided channel. </summary>
        public async Task<Message> SendFile(User user, string filename, Stream stream)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            CheckReady();

            var channel = await CreatePMChannel(user).ConfigureAwait(false);
            return await SendFile(channel, filename, stream).ConfigureAwait(false);
        }
        private async Task<Message> SendMessageInternal(Channel channel, string text, bool isTextToSpeech)
        {
            Message msg;
            var server = channel.Server;

            var mentionedUsers = new List<User>();
            text = Mention.CleanUserMentions(this, server, text, mentionedUsers);
            if (text.Length > MaxMessageSize)
                throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {MaxMessageSize} characters or less.");

            if (Config.UseMessageQueue)
            {
                var nonce = GenerateNonce();
                msg = new Message(this, 0, channel.Id, _currentUser.Id); //_messages.GetOrAdd(nonce, channel.Id, _privateUser.Id);
                var currentUser = msg.User;
                msg.Update(new APIMessage
                {
                    Content = text,
                    Timestamp = DateTime.UtcNow,
                    Author = new APIUser { Avatar = currentUser.AvatarId, Discriminator = currentUser.Discriminator, Id = _currentUser.Id, Username = currentUser.Name },
                    ChannelId = channel.Id,
                    Nonce = IdConvert.ToString(nonce),
                    IsTextToSpeech = isTextToSpeech
                });
                msg.State = MessageState.Queued;

                _pendingMessages.Enqueue(new MessageQueueItem(msg, text, mentionedUsers.Select(x => x.Id).ToArray()));
            }
            else
            {
                var request = new SendMessageRequest(channel.Id)
                {
                    Content = text,
                    MentionedUserIds = mentionedUsers.Select(x => x.Id).ToArray(),
                    Nonce = null,
                    IsTTS = isTextToSpeech
                };
                var model = await _clientRest.Send(request).ConfigureAwait(false);
                msg = _messages.GetOrAdd(model.Id, channel.Id, model.Author.Id);
                msg.Update(model);
                RaiseMessageSent(msg);
            }
            return msg;
        }

        /// <summary> Edits the provided message, changing only non-null attributes. </summary>
        /// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
        public async Task EditMessage(Message message, string text)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (text == null) throw new ArgumentNullException(nameof(text));
            CheckReady();

            var channel = message.Channel;
            var mentionedUsers = new List<User>();
            if (!channel.IsPrivate)
                text = Mention.CleanUserMentions(this, channel.Server, text, mentionedUsers);

            if (text.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {DiscordConfig.MaxMessageSize} characters or less.");

            if (Config.UseMessageQueue)
                _pendingMessages.Enqueue(new MessageQueueItem(message, text, mentionedUsers.Select(x => x.Id).ToArray()));
            else
            {
                var request = new UpdateMessageRequest(message.Channel.Id, message.Id)
                {
                    Content = text,
                    MentionedUserIds = mentionedUsers.Select(x => x.Id).ToArray()
                };
                await _clientRest.Send(request).ConfigureAwait(false);
            }
        }

        /// <summary> Deletes the provided message. </summary>
        public async Task DeleteMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            CheckReady();

            var request = new DeleteMessageRequest(message.Id, message.Channel.Id);
            try { await _clientRest.Send(request).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
        public async Task DeleteMessages(IEnumerable<Message> messages)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));
            CheckReady();

            foreach (var message in messages)
            {
                var request = new DeleteMessageRequest(message.Id, message.Channel.Id);
                try { await _clientRest.Send(request).ConfigureAwait(false); }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
            }
        }

        /// <summary> Downloads messages from the server, returning all messages before or after relativeMessageId, if it's provided. </summary>
        public async Task<Message[]> DownloadMessages(Channel channel, int limit = 100, ulong? relativeMessageId = null, RelativeDirection relativeDir = RelativeDirection.Before, bool useCache = true)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (limit < 0) throw new ArgumentNullException(nameof(limit));
            CheckReady();

            if (limit == 0) return new Message[0];
            if (channel != null && channel.Type == ChannelType.Text)
            {
                try
                {
                    var request = new GetMessagesRequest(channel.Id)
                    {
                        Limit = limit,
                        RelativeDir = relativeDir == RelativeDirection.Before ? "before" : "after",
                        RelativeId = relativeMessageId
                    };
                    var msgs = await _clientRest.Send(request).ConfigureAwait(false);
                    return msgs.Select(x =>
                    {
                        Message msg = null;
                        if (useCache)
                        {
                            msg = _messages.GetOrAdd(x.Id, x.ChannelId, x.Author.Id);
                            var user = msg.User;
                            if (user != null)
                                user.UpdateActivity(msg.EditedTimestamp ?? msg.Timestamp);
                        }
                        else
                            msg = new Message(this, x.Id, x.ChannelId, x.Author.Id);
                        msg.Update(x);
                        return msg;
                    })
                    .ToArray();
                }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.Forbidden) { } //Bad Permissions
            }
            return new Message[0];
        }

        /// <summary> Marks a given message as read. </summary>
        public void AckMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!message.IsAuthor)
                _clientRest.Send(new AckMessageRequest(message.Id, message.Channel.Id));
        }

        /// <summary> Deserializes messages from JSON format and imports them into the message cache.</summary>
        public IEnumerable<Message> ImportMessages(Channel channel, string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var dic = JArray.Parse(json)
                .Select(x =>
                {
                    var msg = new Message(this,
                        x["Id"].Value<ulong>(),
                        channel.Id,
                        x["UserId"].Value<ulong>());

                    var reader = x.CreateReader();
                    _messageImporter.Populate(reader, msg);
                    msg.Text = Mention.Resolve(msg, msg.RawText);
                    return msg;
                })
                .ToDictionary(x => x.Id);
            _messages.Import(dic);
            foreach (var msg in dic.Values)
            {
                var user = msg.User;
                if (user != null)
                    user.UpdateActivity(msg.EditedTimestamp ?? msg.Timestamp);
            }
            return dic.Values;
        }

        /// <summary> Serializes the message cache for a given channel to JSON.</summary>
        public string ExportMessages(Channel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));

            return JsonConvert.SerializeObject(channel.Messages);
        }

        /// <summary> Returns the user with the specified id, along with their server-specific data, or null if none was found. </summary>
        public User GetUser(Server server, ulong userId)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            CheckReady();

            return _users[userId, server.Id];
        }
        /// <summary> Returns the user with the specified name and discriminator, along withtheir server-specific data, or null if they couldn't be found. </summary>
        public User GetUser(Server server, string username, ushort discriminator)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (username == null) throw new ArgumentNullException(nameof(username));
            CheckReady();

            return FindUsers(server.Members, server.Id, username, discriminator, true).FirstOrDefault();
        }

        /// <summary> Returns all users with the specified server and name, along with their server-specific data. </summary>
        /// <remarks> Name formats supported: Name, @Name and &lt;@Id&gt;. Search is case-insensitive if exactMatch is false.</remarks>
        public IEnumerable<User> FindUsers(Server server, string name, bool exactMatch = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (name == null) throw new ArgumentNullException(nameof(name));
            CheckReady();

            return FindUsers(server.Members, server.Id, name, exactMatch: exactMatch);
        }
        /// <summary> Returns all users with the specified channel and name, along with their server-specific data. </summary>
        /// <remarks> Name formats supported: Name, @Name and &lt;@Id&gt;. Search is case-insensitive if exactMatch is false.</remarks>
        public IEnumerable<User> FindUsers(Channel channel, string name, bool exactMatch = false)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (name == null) throw new ArgumentNullException(nameof(name));
            CheckReady();

            return FindUsers(channel.Members, channel.IsPrivate ? (ulong?)null : channel.Server.Id, name, exactMatch: exactMatch);
        }

        private IEnumerable<User> FindUsers(IEnumerable<User> users, ulong? serverId, string name, ushort? discriminator = null, bool exactMatch = false)
        {
            var query = users.Where(x => string.Equals(x.Name, name, exactMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));

            if (!exactMatch && name.Length >= 2)
            {
                if (name[0] == '<' && name[1] == '@' && name[name.Length - 1] == '>') //Parse mention
                {
                    ulong id = IdConvert.ToLong(name.Substring(2, name.Length - 3));
                    var user = _users[id, serverId];
                    if (user != null)
                        query = query.Concat(new User[] { user });
                }
                else if (name[0] == '@') //If we somehow get text starting with @ but isn't a mention
                {
                    string name2 = name.Substring(1);
                    query = query.Concat(users.Where(x => string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase)));
                }
            }

            if (discriminator != null)
                query = query.Where(x => x.Discriminator == discriminator.Value);
            return query;
        }

        public Task EditUser(User user, bool? isMuted = null, bool? isDeafened = null, Channel voiceChannel = null, IEnumerable<Role> roles = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.IsPrivate) throw new InvalidOperationException("Unable to edit users in a private channel");
            CheckReady();

            //Modify the roles collection and filter out the everyone role
            var roleIds = roles == null ? null : user.Roles.Where(x => !x.IsEveryone).Select(x => x.Id);

            var request = new UpdateMemberRequest(user.Server.Id, user.Id)
            {
                IsMuted = isMuted ?? user.IsServerMuted,
                IsDeafened = isDeafened ?? user.IsServerDeafened,
                VoiceChannelId = voiceChannel?.Id,
                RoleIds = roleIds.ToArray()
            };
            return _clientRest.Send(request);
        }

        public Task KickUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.IsPrivate) throw new InvalidOperationException("Unable to kick users from a private channel");
            CheckReady();

            var request = new KickMemberRequest(user.Server.Id, user.Id);
            return _clientRest.Send(request);
        }
        public Task BanUser(User user, int pruneDays = 0)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.IsPrivate) throw new InvalidOperationException("Unable to ban users from a private channel");
            CheckReady();

            var request = new AddGuildBanRequest(user.Server.Id, user.Id);
            request.PruneDays = pruneDays;
            return _clientRest.Send(request);
        }
        public async Task UnbanUser(Server server, ulong userId)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            CheckReady();

            try { await _clientRest.Send(new RemoveGuildBanRequest(server.Id, userId)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        public async Task<int> PruneUsers(Server server, int days, bool simulate = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (days <= 0) throw new ArgumentOutOfRangeException(nameof(days));
            CheckReady();

            var request = new PruneMembersRequest(server.Id)
            {
                Days = days,
                IsSimulation = simulate
            };
            var response = await _clientRest.Send(request).ConfigureAwait(false);
            return response.Pruned;
        }

        /// <summary>When Config.UseLargeThreshold is enabled, running this command will request the Discord server to provide you with all offline users for a particular server.</summary>
        public void RequestOfflineUsers(Server server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));

            _webSocket.SendRequestMembers(server.Id, "", 0);
        }

        public async Task EditProfile(string currentPassword = "",
            string username = null, string email = null, string password = null,
            Stream avatar = null, ImageType avatarType = ImageType.Png)
        {
            if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));
            CheckReady();

            var request = new UpdateProfileRequest()
            {
                CurrentPassword = currentPassword,
                Email = email ?? _currentUser?.Email,
                Password = password,
                Username = username ?? _privateUser?.Name,
                AvatarBase64 = Base64Image(avatarType, avatar, _privateUser?.AvatarId)
            };

            await _clientRest.Send(request).ConfigureAwait(false);

            if (password != null)
            {
                var loginRequest = new LoginRequest()
                {
                    Email = _currentUser.Email,
                    Password = password
                };
                var loginResponse = await _clientRest.Send(loginRequest).ConfigureAwait(false);
                _clientRest.SetToken(loginResponse.Token);
            }
        }
        
        /// <summary> Returns the role with the specified id, or null if none was found. </summary>
        public Role GetRole(ulong id)
        {
            CheckReady();

            return _roles[id];
        }
        /// <summary> Returns all roles with the specified server and name. </summary>
        /// <remarks> Name formats supported: Name and @Name. Search is case-insensitive. </remarks>
        public IEnumerable<Role> FindRoles(Server server, string name)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (name == null) throw new ArgumentNullException(nameof(name));
            CheckReady();

            // if (name.StartsWith("@"))
			// {
			// 	string name2 = name.Substring(1);
			// 	return _roles.Where(x => x.Server.Id == server.Id &&
			// 		string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) || 
			// 		string.Equals(x.Name, name2, StringComparison.OrdinalIgnoreCase));
			// }
			// else
			// {
            return _roles.Where(x => x.Server.Id == server.Id &&
                string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            // }
        }

        /// <summary> Note: due to current API limitations, the created role cannot be returned. </summary>
        public async Task<Role> CreateRole(Server server, string name, ServerPermissions permissions = null, Color color = null, bool isHoisted = false)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (name == null) throw new ArgumentNullException(nameof(name));
            CheckReady();

            var request1 = new CreateRoleRequest(server.Id);
            var response1 = await _clientRest.Send(request1).ConfigureAwait(false);
            var role = _roles.GetOrAdd(response1.Id, server.Id);
            role.Update(response1);

            var request2 = new UpdateRoleRequest(role.Server.Id, role.Id)
            {
                Name = name,
                Permissions = (permissions ?? role.Permissions).RawValue,
                Color = (color ?? Color.Default).RawValue,
                IsHoisted = isHoisted
            };
            var response2 = await _clientRest.Send(request2).ConfigureAwait(false);
            role.Update(response2);

            return role;
        }

        public async Task EditRole(Role role, string name = null, ServerPermissions permissions = null, Color color = null, bool? isHoisted = null, int? position = null)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            CheckReady();

            var request1 = new UpdateRoleRequest(role.Server.Id, role.Id)
            {
                Name = name ?? role.Name,
                Permissions = (permissions ?? role.Permissions).RawValue,
                Color = (color ?? role.Color).RawValue,
                IsHoisted = isHoisted ?? role.IsHoisted
            };

            var response = await _clientRest.Send(request1).ConfigureAwait(false);

            if (position != null)
            {
                int oldPos = role.Position;
                int newPos = position.Value;
                int minPos;
                Role[] roles = role.Server.Roles.OrderBy(x => x.Position).ToArray();

                if (oldPos < newPos) //Moving Down
                {
                    minPos = oldPos;
                    for (int i = oldPos; i < newPos; i++)
                        roles[i] = roles[i + 1];
                    roles[newPos] = role;
                }
                else //(oldPos > newPos) Moving Up
                {
                    minPos = newPos;
                    for (int i = oldPos; i > newPos; i--)
                        roles[i] = roles[i - 1];
                    roles[newPos] = role;
                }

                var request2 = new ReorderRolesRequest(role.Server.Id)
                {
                    RoleIds = roles.Skip(minPos).Select(x => x.Id).ToArray(),
                    StartPos = minPos
                };
                await _clientRest.Send(request2).ConfigureAwait(false);
            }
        }

        public async Task DeleteRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            CheckReady();

            try { await _clientRest.Send(new DeleteRoleRequest(role.Server.Id, role.Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        public Task ReorderRoles(Server server, IEnumerable<Role> roles, int startPos = 0)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (roles == null) throw new ArgumentNullException(nameof(roles));
            if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");
            CheckReady();

            return _clientRest.Send(new ReorderRolesRequest(server.Id)
            {
                RoleIds = roles.Select(x => x.Id).ToArray(),
                StartPos = startPos
            });
        }

        /// <summary> Returns the server with the specified id, or null if none was found. </summary>
        public Server GetServer(ulong id)
        {
            CheckReady();

            return _servers[id];
        }

        /// <summary> Returns all servers with the specified name. </summary>
        /// <remarks> Search is case-insensitive. </remarks>
        public IEnumerable<Server> FindServers(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            CheckReady();

            return _servers.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary> Creates a new server with the provided name and region (see Regions). </summary>
        public async Task<Server> CreateServer(string name, Region region, ImageType iconType = ImageType.None, Stream icon = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (region == null) throw new ArgumentNullException(nameof(region));
            CheckReady();

            var request = new CreateGuildRequest()
            {
                Name = name,
                Region = region.Id,
                IconBase64 = Base64Image(iconType, icon, null)
            };
            var response = await _clientRest.Send(request).ConfigureAwait(false);

            var server = _servers.GetOrAdd(response.Id);
            server.Update(response);
            return server;
        }

        /// <summary> Edits the provided server, changing only non-null attributes. </summary>
        public async Task EditServer(Server server, string name = null, string region = null, Stream icon = null, ImageType iconType = ImageType.Png)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            CheckReady();

            var request = new UpdateGuildRequest(server.Id)
            {
                Name = name ?? server.Name,
                Region = region ?? server.Region,
                IconBase64 = Base64Image(iconType, icon, server.IconId),
                AFKChannelId = server.AFKChannel?.Id,
                AFKTimeout = server.AFKTimeout
            };
            var response = await _clientRest.Send(request).ConfigureAwait(false);
            server.Update(response);
        }

        /// <summary> Leaves the provided server, destroying it if you are the owner. </summary>
        public async Task LeaveServer(Server server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            CheckReady();

            try { await _clientRest.Send(new LeaveGuildRequest(server.Id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        public async Task<IEnumerable<Region>> GetVoiceRegions()
        {
            CheckReady();

            var regions = await _clientRest.Send(new GetVoiceRegionsRequest()).ConfigureAwait(false);
            return regions.Select(x => new Region(x.Id, x.Name, x.Hostname, x.Port));
        }
        public DualChannelPermissions GetChannelPermissions(Channel channel, User user)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (user == null) throw new ArgumentNullException(nameof(user));
            CheckReady();

            return channel.PermissionOverwrites
                .Where(x => x.TargetType == PermissionTarget.User && x.TargetId == user.Id)
                .Select(x => x.Permissions)
                .FirstOrDefault();
        }
        public DualChannelPermissions GetChannelPermissions(Channel channel, Role role)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (role == null) throw new ArgumentNullException(nameof(role));
            CheckReady();

            return channel.PermissionOverwrites
                .Where(x => x.TargetType == PermissionTarget.Role && x.TargetId == role.Id)
                .Select(x => x.Permissions)
                .FirstOrDefault();
        }

        public Task SetChannelPermissions(Channel channel, User user, ChannelPermissions allow = null, ChannelPermissions deny = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (user == null) throw new ArgumentNullException(nameof(user));
            CheckReady();

            return SetChannelPermissions(channel, user.Id, PermissionTarget.User, allow, deny);
        }
        public Task SetChannelPermissions(Channel channel, User user, DualChannelPermissions permissions = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (user == null) throw new ArgumentNullException(nameof(user));
            CheckReady();

            return SetChannelPermissions(channel, user.Id, PermissionTarget.User, permissions?.Allow, permissions?.Deny);
        }
        public Task SetChannelPermissions(Channel channel, Role role, ChannelPermissions allow = null, ChannelPermissions deny = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (role == null) throw new ArgumentNullException(nameof(role));
            CheckReady();

            return SetChannelPermissions(channel, role.Id, PermissionTarget.Role, allow, deny);
        }
        public Task SetChannelPermissions(Channel channel, Role role, DualChannelPermissions permissions = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (role == null) throw new ArgumentNullException(nameof(role));
            CheckReady();

            return SetChannelPermissions(channel, role.Id, PermissionTarget.Role, permissions?.Allow, permissions?.Deny);
        }
        private Task SetChannelPermissions(Channel channel, ulong targetId, PermissionTarget targetType, ChannelPermissions allow = null, ChannelPermissions deny = null)
        {
            var request = new AddChannelPermissionsRequest(channel.Id)
            {
                TargetId = targetId,
                TargetType = targetType.Value,
                Allow = allow?.RawValue ?? 0,
                Deny = deny?.RawValue ?? 0
            };
            return _clientRest.Send(request);
        }

        public Task RemoveChannelPermissions(Channel channel, User user)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (user == null) throw new ArgumentNullException(nameof(user));
            CheckReady();

            return RemoveChannelPermissions(channel, user.Id, PermissionTarget.User);
        }
        public Task RemoveChannelPermissions(Channel channel, Role role)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (role == null) throw new ArgumentNullException(nameof(role));
            CheckReady();

            return RemoveChannelPermissions(channel, role.Id, PermissionTarget.Role);
        }
        private async Task RemoveChannelPermissions(Channel channel, ulong userOrRoleId, PermissionTarget targetType)
        {
            try
            {
                var perms = channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != userOrRoleId).FirstOrDefault();
                await _clientRest.Send(new RemoveChannelPermissionsRequest(channel.Id, userOrRoleId)).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }
    }*/
}
