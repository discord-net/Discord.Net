using Discord.API;
using Discord.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public enum AvatarImageType
	{
		None,
		Jpeg,
		Png
	}

	public partial class DiscordClient
	{
		public const int MaxMessageSize = 2000;

		//Bans
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(Member member)
			=> Ban(member?.ServerId, member?.UserId);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(Server server, User user)
			=> Ban(server?.Id, user?.Id);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(Server server, string userId)
			=> Ban(server?.Id, userId);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(string server, User user)
			=> Ban(server, user?.Id);
		/// <summary> Bans a user from the provided server. </summary>
		public Task Ban(string serverId, string userId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _api.Ban(serverId, userId);
		}

		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(Member member)
			=> Unban(member?.ServerId, member?.UserId);
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(Server server, User user)
			=> Unban(server?.Id, user?.Id);
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(Server server, string userId)
			=> Unban(server?.Id, userId);
		/// <summary> Unbans a user from the provided server. </summary>
		public Task Unban(string server, User user)
			=> Unban(server, user?.Id);
		/// <summary> Unbans a user from the provided server. </summary>
		public async Task Unban(string serverId, string userId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			try { await _api.Unban(serverId, userId).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
		
		//Channels
		/// <summary> Creates a new channel with the provided name and type (see ChannelTypes). </summary>
		public Task<Channel> CreateChannel(Server server, string name, string type)
			=> CreateChannel(server?.Id, name, type);
		/// <summary> Creates a new channel with the provided name and type (see ChannelTypes). </summary>
		public async Task<Channel> CreateChannel(string serverId, string name, string type)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (type == null) throw new ArgumentNullException(nameof(type));

			var response = await _api.CreateChannel(serverId, name, type).ConfigureAwait(false);
			var channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient?.Id);
			channel.Update(response);
			return channel;
		}

		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public Task<Channel> CreatePMChannel(string userId) => CreatePMChannel(_users[userId], userId);
		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public Task<Channel> CreatePMChannel(User user) => CreatePMChannel(user, user?.Id);
		/// <summary> Returns the private channel with the provided user, creating one if it does not currently exist. </summary>
		public Task<Channel> CreatePMChannel(Member member) => CreatePMChannel(member.User, member.UserId);
		private async Task<Channel> CreatePMChannel(User user, string userId)
		{
			CheckReady();
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			Channel channel = null;
			if (user != null)
				channel = user.PrivateChannel;
			if (channel == null)
			{
				var response = await _api.CreatePMChannel(CurrentUserId, userId).ConfigureAwait(false);
				user = _users.GetOrAdd(response.Recipient?.Id);
				user.Update(response.Recipient);
				channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient?.Id);
				channel.Update(response);
			}
			return channel;
		}

		/// <summary> Edits the provided channel, changing only non-null attributes. </summary>
		public Task EditChannel(string channelId, string name = null, string topic = null, int? position = null)
			=> EditChannel(_channels[channelId], name: name, topic: topic, position: position);
		/// <summary> Edits the provided channel, changing only non-null attributes. </summary>
        public async Task EditChannel(Channel channel, string name = null, string topic = null, int? position = null)
		{
			CheckReady();
			if (channel == null) throw new ArgumentNullException(nameof(channel));

			await _api.EditChannel(channel.Id, name: name, topic: topic);

			if (position != null)
			{
				int oldPos = channel.Position;
				int newPos = position.Value;
				int minPos;
				Channel[] channels = channel.Server.Channels.OrderBy(x => x.Position).ToArray();

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
				await _api.ReorderChannels(channel.ServerId, channels.Skip(minPos).Select(x => x.Id), minPos);
			}
		}

		public Task ReorderChannels(Server server, IEnumerable<object> channels, int startPos = 0)
			=> ReorderChannels(server.Id, channels, startPos);
        public Task ReorderChannels(string serverId, IEnumerable<object> channels, int startPos = 0)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (channels == null) throw new ArgumentNullException(nameof(channels));
			if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");

			var channelIds = channels.Select(x =>
			{
				if (x is string)
					return x as string;
				else if (x is Channel)
					return (x as Channel).Id;
				else
					throw new ArgumentException("Channels must be a collection of string or Channel.", nameof(channels));
			});

            return _api.ReorderChannels(serverId, channelIds, startPos);
		}

		/// <summary> Destroys the provided channel. </summary>
		public Task<Channel> DestroyChannel(Channel channel)
			=> DestroyChannel(channel?.Id);
		/// <summary> Destroys the provided channel. </summary>
		public async Task<Channel> DestroyChannel(string channelId)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			try { await _api.DestroyChannel(channelId).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			return _channels.TryRemove(channelId);
		}

		//Invites
		/// <summary> Creates a new invite to the default channel of the provided server. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcdPass"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. </param>
		public Task<Invite> CreateInvite(Server server, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
			=> CreateInvite(server?.DefaultChannelId, maxAge, maxUses, isTemporary, hasXkcdPass);
		/// <summary> Creates a new invite to the provided channel. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcdPass"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. </param>
		public Task<Invite> CreateInvite(Channel channel, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
			=> CreateInvite(channel?.Id, maxAge, maxUses, isTemporary, hasXkcdPass);
		/// <summary> Creates a new invite to the provided channel. </summary>
		/// <param name="maxAge"> Time (in seconds) until the invite expires. Set to 0 to never expire. </param>
		/// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the server after closing their client. </param>
		/// <param name="hasXkcdPass"> If true, creates a human-readable link. Not supported if maxAge is set to 0. </param>
		/// <param name="maxUses"> The max amount  of times this invite may be used. </param>
		public async Task<Invite> CreateInvite(string channelId, int maxAge, int maxUses, bool isTemporary, bool hasXkcdPass)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (maxAge <= 0) throw new ArgumentOutOfRangeException(nameof(maxAge));
			if (maxUses <= 0) throw new ArgumentOutOfRangeException(nameof(maxUses));

			var response = await _api.CreateInvite(channelId, maxAge, maxUses, isTemporary, hasXkcdPass).ConfigureAwait(false);
			var invite = new Invite(this, response.Code, response.XkcdPass, response.Guild.Id);
			invite.Update(response);
			return invite;
		}

		/// <summary> Deletes the provided invite. </summary>
		public async Task DestroyInvite(string inviteId)
		{
			CheckReady();
			if (inviteId == null) throw new ArgumentNullException(nameof(inviteId));

			try
			{
				//Check if this is a human-readable link and get its ID
				var response = await _api.GetInvite(inviteId).ConfigureAwait(false);
				await _api.DeleteInvite(response.Code).ConfigureAwait(false);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		/// <summary> Gets more info about the provided invite code. </summary>
		/// <remarks> Supported formats: inviteCode, xkcdCode, https://discord.gg/inviteCode, https://discord.gg/xkcdCode </remarks>
		public async Task<Invite> GetInvite(string inviteIdOrXkcd)
		{
			CheckReady();
			if (inviteIdOrXkcd == null) throw new ArgumentNullException(nameof(inviteIdOrXkcd));
			
			var response = await _api.GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
			var invite = new Invite(this, response.Code, response.XkcdPass, response.Guild.Id);
            invite.Update(response);
			return invite;
		}

		/// <summary> Accepts the provided invite. </summary>
		public Task AcceptInvite(Invite invite)
		{
			CheckReady();
			if (invite == null) throw new ArgumentNullException(nameof(invite));

			return _api.AcceptInvite(invite.Id);
		}
		/// <summary> Accepts the provided invite. </summary>
		public async Task AcceptInvite(string inviteId)
		{
			CheckReady();
			if (inviteId == null) throw new ArgumentNullException(nameof(inviteId));

			//Remove trailing slash and any non-code url parts
			if (inviteId.Length > 0 && inviteId[inviteId.Length - 1] == '/')
				inviteId = inviteId.Substring(0, inviteId.Length - 1);
			int index = inviteId.LastIndexOf('/');
			if (index >= 0)
				inviteId = inviteId.Substring(index + 1);

			//Check if this is a human-readable link and get its ID
			var invite = await GetInvite(inviteId).ConfigureAwait(false);
			await _api.AcceptInvite(invite.Id).ConfigureAwait(false);
		}

		//Members
		public Task EditMember(Member member, bool? mute = null, bool? deaf = null, IEnumerable<object> roles = null)
			=> EditMember(member?.ServerId, member?.UserId, mute, deaf, roles);
		public Task EditMember(Server server, User user, bool? mute = null, bool? deaf = null, IEnumerable<object> roles = null)
			=> EditMember(server?.Id, user?.Id, mute, deaf, roles);
		public Task EditMember(Server server, string userId, bool? mute = null, bool? deaf = null, IEnumerable<string> roles = null)
			=> EditMember(server?.Id, userId, mute, deaf, roles);
		public Task EditMember(string serverId, User user, bool? mute = null, bool? deaf = null, IEnumerable<object> roles = null)
			=> EditMember(serverId, user?.Id, mute, deaf, roles);
        public Task EditMember(string serverId, string userId, bool? mute = null, bool? deaf = null, IEnumerable<object> roles = null)
		{
			CheckReady();
			if (serverId == null) throw new NullReferenceException(nameof(serverId));
			if (userId == null) throw new NullReferenceException(nameof(userId));

			IEnumerable<string> newRoles = roles?.Select(x =>
			{
				if (x is string)
					return x as string;
				else if (x is Role)
					return (x as Role).Id;
				else
					throw new ArgumentException("Roles must be a collection of string or Role.", nameof(roles));
			});

			return _api.EditMember(serverId, userId, mute: mute, deaf: deaf, roles: newRoles);
		}

		//Messages
		/// <summary> Sends a message to the provided channel, optionally mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task<Message[]> SendMessage(Channel channel, string text, params string[] mentions)
			=> SendMessage(channel, text, mentions, false);
		/// <summary> Sends a message to the provided channel, optionally mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task<Message[]> SendMessage(string channelId, string text, params string[] mentions)
			=> SendMessage(_channels[channelId], text, mentions, false);
		/// <summary> Sends a message to the provided channel, optionally mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task<Message[]> SendTTSMessage(Channel channel, string text, params string[] mentions)
			=> SendMessage(channel, text, mentions, true);
		/// <summary> Sends a message to the provided channel, optionally mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task<Message[]> SendTTSMessage(string channelId, string text, params string[] mentions)
			=> SendMessage(_channels[channelId], text, mentions, true);
		/// <summary> Sends a message to the provided channel, optionally mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		private async Task<Message[]> SendMessage(Channel channel, string text, string[] mentions, bool isTextToSpeech)
		{
			CheckReady();
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (text == null) throw new ArgumentNullException(nameof(text));
			mentions = mentions ?? new string[0];

			int blockCount = (int)Math.Ceiling(text.Length / (double)MaxMessageSize);
			Message[] result = new Message[blockCount];
			for (int i = 0; i < blockCount; i++)
			{
				int index = i * MaxMessageSize;
				string blockText = text.Substring(index, Math.Min(2000, text.Length - index));
				var nonce = GenerateNonce();
				if (Config.UseMessageQueue)
				{
					var msg = _messages.GetOrAdd("nonce_" + nonce, channel.Id, CurrentUserId);
					var currentMember = _members[msg.UserId, channel.ServerId];
                    msg.Update(new API.Message
					{
						Content = blockText,
						Timestamp = DateTime.UtcNow,
						Author = new UserReference { Avatar = currentMember.AvatarId, Discriminator = currentMember.Discriminator, Id = CurrentUserId, Username = currentMember.Name },
						ChannelId = channel.Id,
						IsTextToSpeech = isTextToSpeech
					});
					msg.IsQueued = true;
					msg.Nonce = nonce;
					result[i] = msg;
					_pendingMessages.Enqueue(msg);
				}
				else
				{
					var model = await _api.SendMessage(channel.Id, blockText, mentions, nonce, isTextToSpeech).ConfigureAwait(false);
					var msg = _messages.GetOrAdd(model.Id, channel.Id, model.Author.Id);
					msg.Update(model);
					RaiseMessageSent(msg);
					result[i] = msg;
				}
				await Task.Delay(1000).ConfigureAwait(false);
			}
			return result;
		}

		/// <summary> Sends a private message to the provided user. </summary>
		public Task<Message[]> SendPrivateMessage(Member member, string text)
			=> SendPrivateMessage(member?.UserId, text);
		/// <summary> Sends a private message to the provided user. </summary>
		public Task<Message[]> SendPrivateMessage(User user, string text)
			=> SendPrivateMessage(user?.Id, text);
		/// <summary> Sends a private message to the provided user. </summary>
		public async Task<Message[]> SendPrivateMessage(string userId, string text)
		{
			var channel = await CreatePMChannel(userId).ConfigureAwait(false);
			return await SendMessage(channel, text, new string[0]).ConfigureAwait(false);
		}

		/// <summary> Sends a file to the provided channel. </summary>
		public Task SendFile(Channel channel, string filePath)
			=> SendFile(channel?.Id, filePath);
		/// <summary> Sends a file to the provided channel. </summary>
		public Task SendFile(string channelId, string filePath)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (filePath == null) throw new ArgumentNullException(nameof(filePath));

			return _api.SendFile(channelId, filePath);
		}

		/// <summary> Edits the provided message, changing only non-null attributes. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public Task EditMessage(Message message, string text = null, params string[] mentions)
			=> EditMessage(message?.ChannelId, message?.Id, text, mentions);
		/// <summary> Edits the provided message, changing only non-null attributes. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public Task EditMessage(Channel channel, string messageId, string text = null, params string[] mentions)
			=> EditMessage(channel?.Id, messageId, text, mentions);
		/// <summary> Edits the provided message, changing only non-null attributes. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public async Task EditMessage(string channelId, string messageId, string text = null, params string[] mentions)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (messageId == null) throw new ArgumentNullException(nameof(messageId));
			mentions = mentions ?? new string[0];

			if (text != null && text.Length > MaxMessageSize)
				text = text.Substring(0, MaxMessageSize);

			var model = await _api.EditMessage(messageId, channelId, text, mentions).ConfigureAwait(false);
			var msg = _messages[messageId];
			if (msg != null)
				msg.Update(model);
		}

		/// <summary> Deletes the provided message. </summary>
		public Task DeleteMessage(Message msg)
			=> DeleteMessage(msg?.ChannelId, msg?.Id);
		/// <summary> Deletes the provided message. </summary>
		public async Task DeleteMessage(string channelId, string msgId)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (msgId == null) throw new ArgumentNullException(nameof(msgId));

			try
			{
				await _api.DeleteMessage(msgId, channelId).ConfigureAwait(false);
				_messages.TryRemove(msgId);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
		public async Task DeleteMessages(IEnumerable<Message> msgs)
		{
			CheckReady();
			if (msgs == null) throw new ArgumentNullException(nameof(msgs));

			foreach (var msg in msgs)
			{
				try
				{
					await _api.DeleteMessage(msg.Id, msg.ChannelId).ConfigureAwait(false);
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			}
		}
		public async Task DeleteMessages(string channelId, IEnumerable<string> msgIds)
		{
			CheckReady();
			if (msgIds == null) throw new ArgumentNullException(nameof(msgIds));

			foreach (var msgId in msgIds)
			{
				try
				{
					await _api.DeleteMessage(msgId, channelId).ConfigureAwait(false);
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			}
		}

		/// <summary> Downloads last count messages from the server, starting at beforeMessageId if it's provided. </summary>
		public Task<Message[]> DownloadMessages(Channel channel, int count, string beforeMessageId = null, bool cache = true)
			=> DownloadMessages(channel.Id, count, beforeMessageId, cache);
		/// <summary> Downloads last count messages from the server, starting at beforeMessageId if it's provided. </summary>
		public async Task<Message[]> DownloadMessages(string channelId, int count, string beforeMessageId = null, bool cache = true)
		{
			CheckReady();
			if (channelId == null) throw new NullReferenceException(nameof(channelId));
			if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
			if (count == 0) return new Message[0];

			Channel channel = _channels[channelId];
			if (channel != null && channel.Type == ChannelTypes.Text)
			{
				try
				{
					var msgs = await _api.GetMessages(channel.Id, count).ConfigureAwait(false);
					return msgs.Select(x =>
						{
							Message msg;
							if (cache)
								msg = _messages.GetOrAdd(x.Id, x.ChannelId, x.Author.Id);
							else
								msg = _messages[x.Id] ?? new Message(this, x.Id, x.ChannelId, x.Author.Id);
							if (msg != null)
							{
								msg.Update(x);
								if (Config.TrackActivity)
								{
									if (channel.IsPrivate)
									{
										var user = msg.User;
										if (user != null)
											user.UpdateActivity(msg.EditedTimestamp ?? msg.Timestamp);
									}
									else
									{
										var member = msg.Member;
										if (member != null)
											member.UpdateActivity(msg.EditedTimestamp ?? msg.Timestamp);
									}
								}
							}
							return msg;
						})
						.ToArray();
				}
				catch (HttpException) { } //Bad Permissions?
			}
			return null;
		}

		//Permissions
		public Task SetChannelUserPermissions(Channel channel, Member member, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, member?.UserId, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(string channelId, Member member, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], member?.UserId, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(Channel channel, User user, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, user?.Id, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(string channelId, User user, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], user?.Id, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(Channel channel, string userId, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, userId, PermissionTarget.Member, allow, deny);
		public Task SetChannelUserPermissions(string channelId, string userId, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], userId, PermissionTarget.Member, allow, deny);

		public Task SetChannelRolePermissions(Channel channel, Role role, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, role?.Id, PermissionTarget.Role, allow, deny);
		public Task SetChannelRolePermissions(string channelId, Role role, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], role?.Id, PermissionTarget.Role, allow, deny);
		public Task SetChannelRolePermissions(Channel channel, string userId, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(channel, userId, PermissionTarget.Role, allow, deny);
		public Task SetChannelRolePermissions(string channelId, string userId, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
			=> SetChannelPermissions(_channels[channelId], userId, PermissionTarget.Role, allow, deny);

		private async Task SetChannelPermissions(Channel channel, string targetId, string targetType, PackedChannelPermissions allow = null, PackedChannelPermissions deny = null)
		{
			CheckReady();
			if (channel == null) throw new NullReferenceException(nameof(channel));
			if (targetId == null) throw new NullReferenceException(nameof(targetId));
			if (targetType == null) throw new NullReferenceException(nameof(targetType));

			uint allowValue = allow?.RawValue ?? 0;
			uint denyValue = deny?.RawValue ?? 0;
			bool changed = false;

			var perms = channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != targetId).FirstOrDefault();
			if (allowValue != 0 || denyValue != 0)
			{
				await _api.SetChannelPermissions(channel.Id, targetId, targetType, allowValue, denyValue);
				if (perms != null)
				{
                    perms.Allow.SetRawValueInternal(allowValue);
					perms.Deny.SetRawValueInternal(denyValue);
				}
				else
				{
					var oldPerms = channel._permissionOverwrites;
					var newPerms = new Channel.PermissionOverwrite[oldPerms.Length + 1];
					Array.Copy(oldPerms, newPerms, oldPerms.Length);
					newPerms[oldPerms.Length] = new Channel.PermissionOverwrite(targetType, targetId, allowValue, denyValue);
					channel._permissionOverwrites = newPerms;
				}
				changed = true;
			}
			else
			{
				try
				{
					await _api.DeleteChannelPermissions(channel.Id, targetId);
					if (perms != null)
					{
						channel._permissionOverwrites = channel.PermissionOverwrites.Where(x => x.TargetType != targetType || x.TargetId != targetId).ToArray();
						changed = true;
					}
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			}

			if (changed)
			{
				if (targetType == PermissionTarget.Role)
					channel.InvalidatePermissionsCache();
				else if (targetType == PermissionTarget.Member)
					channel.InvalidatePermissionsCache(targetId);
			}
		}

		public Task RemoveChannelUserPermissions(Channel channel, Member member)
			=> RemoveChannelPermissions(channel, member?.UserId, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(string channelId, Member member)
			=> RemoveChannelPermissions(_channels[channelId], member?.UserId, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(Channel channel, User user)
			=> RemoveChannelPermissions(channel, user?.Id, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(string channelId, User user)
			=> RemoveChannelPermissions(_channels[channelId], user?.Id, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(Channel channel, string userId)
			=> RemoveChannelPermissions(channel, userId, PermissionTarget.Member);
		public Task RemoveChannelUserPermissions(string channelId, string userId)
			=> RemoveChannelPermissions(_channels[channelId], userId, PermissionTarget.Member);

		public Task RemoveChannelRolePermissions(Channel channel, Role role)
			=> RemoveChannelPermissions(channel, role?.Id, PermissionTarget.Role);
		public Task RemoveChannelRolePermissions(string channelId, Role role)
			=> RemoveChannelPermissions(_channels[channelId], role?.Id, PermissionTarget.Role);
		public Task RemoveChannelRolePermissions(Channel channel, string roleId)
			=> RemoveChannelPermissions(channel, roleId, PermissionTarget.Role);
		public Task RemoveChannelRolePermissions(string channelId, string roleId)
			=> RemoveChannelPermissions(_channels[channelId], roleId, PermissionTarget.Role);

		private async Task RemoveChannelPermissions(Channel channel, string userOrRoleId, string idType)
		{
			CheckReady();
			if (channel == null) throw new NullReferenceException(nameof(channel));
			if (userOrRoleId == null) throw new NullReferenceException(nameof(userOrRoleId));
			if (idType == null) throw new NullReferenceException(nameof(idType));

			try
			{
				var perms = channel.PermissionOverwrites.Where(x => x.TargetType != idType || x.TargetId != userOrRoleId).FirstOrDefault();
				await _api.DeleteChannelPermissions(channel.Id, userOrRoleId).ConfigureAwait(false);
				if (perms != null)
				{
					channel.PermissionOverwrites.Where(x => x.TargetType != idType || x.TargetId != userOrRoleId).ToArray();

					if (idType == PermissionTarget.Role)
						channel.InvalidatePermissionsCache();
					else if (idType == PermissionTarget.Member)
						channel.InvalidatePermissionsCache(userOrRoleId);
				}
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		//Profile
		public Task<EditProfileResponse> EditProfile(string currentPassword = "",
			string username = null, string email = null, string password = null,
			AvatarImageType avatarType = AvatarImageType.Png, byte[] avatar = null)
		{
			if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));

			return _api.EditProfile(currentPassword: currentPassword, username: username ?? _currentUser?.Name, email: email ?? _currentUser?.Email, password: password,
				avatarType: avatarType, avatar: avatar);
		}
		public Task SetStatus(string status)
		{
			switch (status)
			{
				case UserStatus.Online:
				case UserStatus.Away:
					_status = status;
					break;
				default:
					throw new ArgumentException($"Invalid status, must be {UserStatus.Online} or {UserStatus.Away}");
			}
			return SendStatus();
		}
		public Task SetGame(int? gameId)
		{
			_gameId = gameId;
			return SendStatus();
		}
		private Task SendStatus()
		{
			_dataSocket.SendStatus(_status == UserStatus.Away ? EpochTime.GetMilliseconds() - (10 * 60 * 1000) : (ulong?)null, _gameId);
			return TaskHelper.CompletedTask;
		}

		//Roles
		/// <summary> Note: due to current API limitations, the created role cannot be returned. </summary>
		public Task<Role> CreateRole(Server server, string name)
			=> CreateRole(server?.Id);
		/// <summary> Note: due to current API limitations, the created role cannot be returned. </summary>
		public async Task<Role> CreateRole(string serverId, string name)
		{
			CheckReady();
			if (serverId == null) throw new NullReferenceException(nameof(serverId));

			var response = await _api.CreateRole(serverId).ConfigureAwait(false);
			var role = _roles.GetOrAdd(response.Id, serverId, false);
			role.Update(response);

			await EditRole(role, name: name);

			return role;
		}

		public Task EditRole(Role role, string name = null, PackedServerPermissions permissions = null, PackedColor color = null, bool? hoist = null, int? position = null)
			=> EditRole(role.ServerId, role.Id, name: name, permissions: permissions, color: color, hoist: hoist, position: position);
		public async Task EditRole(string serverId, string roleId, string name = null, PackedServerPermissions permissions = null, PackedColor color = null, bool? hoist = null, int? position = null)
		{
			CheckReady();
			if (serverId == null) throw new NullReferenceException(nameof(serverId));
			if (roleId == null) throw new NullReferenceException(nameof(roleId));
			
			var response = await _api.EditRole(serverId, roleId, name: name, 
				permissions: permissions?.RawValue, color: color?.RawValue, hoist: hoist);

			var role = _roles[response.Id];
			if (role != null)
				role.Update(response);

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
				await _api.ReorderRoles(role.ServerId, roles.Skip(minPos).Select(x => x.Id), minPos);
			}
		}

		public Task DeleteRole(Role role)
			=> DeleteRole(role?.ServerId, role?.Id);
		public Task DeleteRole(string serverId, string roleId)
		{
			CheckReady();
			if (serverId == null) throw new NullReferenceException(nameof(serverId));
			if (roleId == null) throw new NullReferenceException(nameof(roleId));

			return _api.DeleteRole(serverId, roleId);
		}

		public Task ReorderRoles(Server server, IEnumerable<object> roles, int startPos = 0)
			=> ReorderChannels(server.Id, roles, startPos);
		public Task ReorderRoles(string serverId, IEnumerable<object> roles, int startPos = 0)
		{
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (roles == null) throw new ArgumentNullException(nameof(roles));
			if (startPos < 0) throw new ArgumentOutOfRangeException(nameof(startPos), "startPos must be a positive integer.");

			var roleIds = roles.Select(x =>
			{
				if (x is string)
					return x as string;
				else if (x is Role)
					return (x as Role).Id;
				else
					throw new ArgumentException("Channels must be a collection of string or Role.", nameof(roles));
			});

			return _api.ReorderRoles(serverId, roleIds, startPos);
		}

		//Servers
		/// <summary> Creates a new server with the provided name and region (see Regions). </summary>
		public async Task<Server> CreateServer(string name, string region)
		{
			CheckReady();
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (region == null) throw new ArgumentNullException(nameof(region));

			var response = await _api.CreateServer(name, region).ConfigureAwait(false);
			var server = _servers.GetOrAdd(response.Id);
			server.Update(response);
			return server;
		}

		/// <summary> Edits the provided server, changing only non-null attributes. </summary>
		public Task EditServer(Server server)
			=> EditServer(server?.Id);
		/// <summary> Edits the provided server, changing only non-null attributes. </summary>
		public async Task EditServer(string serverId, string name = null, string region = null)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			var response = await _api.EditServer(serverId, name: name, region: region);
			var server = _servers[response.Id];
			if (server != null)
				server.Update(response);
		}

		/// <summary> Leaves the provided server, destroying it if you are the owner. </summary>
		public Task<Server> LeaveServer(Server server)
			=> LeaveServer(server?.Id);
		/// <summary> Leaves the provided server, destroying it if you are the owner. </summary>
		public async Task<Server> LeaveServer(string serverId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			try { await _api.LeaveServer(serverId).ConfigureAwait(false); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			return _servers.TryRemove(serverId);
		}
	}
}
