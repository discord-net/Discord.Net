using Discord.Helpers;
using Discord.Net;
using Discord.Net.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public enum AvatarImageType
	{
		Jpeg,
		Png
	}

	public partial class DiscordClient
	{
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
				var response = await _api.CreatePMChannel(_currentUserId, userId).ConfigureAwait(false);
				channel = _channels.GetOrAdd(response.Id, response.GuildId, response.Recipient?.Id);
				channel.Update(response);
			}
			return channel;
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

		/// <summary> Gets more info about the provided invite code. </summary>
		/// <remarks> Supported formats: inviteCode, xkcdCode, https://discord.gg/inviteCode, https://discord.gg/xkcdCode </remarks>
		public async Task<Invite> GetInvite(string id)
		{
			CheckReady();
			if (id == null) throw new ArgumentNullException(nameof(id));
			
			var response = await _api.GetInvite(id).ConfigureAwait(false);
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
		public async Task AcceptInvite(string code)
		{
			CheckReady();
			if (code == null) throw new ArgumentNullException(nameof(code));

			//Remove trailing slash and any non-code url parts
			if (code.Length > 0 && code[code.Length - 1] == '/')
				code = code.Substring(0, code.Length - 1);
			int index = code.LastIndexOf('/');
			if (index >= 0)
				code = code.Substring(index + 1);

			//Check if this is a human-readable link and get its ID
			var invite = await GetInvite(code).ConfigureAwait(false);
			await _api.AcceptInvite(invite.Id).ConfigureAwait(false);
		}

		/// <summary> Deletes the provided invite. </summary>
		public async Task DeleteInvite(string code)
		{
			CheckReady();
			if (code == null) throw new ArgumentNullException(nameof(code));

			try
			{
				//Check if this is a human-readable link and get its ID
				var response = await _api.GetInvite(code).ConfigureAwait(false);
				await _api.DeleteInvite(response.Code).ConfigureAwait(false);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		//Chat
		/// <summary> Sends a message to the provided channel. </summary>
		public Task<Message[]> SendMessage(Channel channel, string text)
			=> SendMessage(channel?.Id, text, new string[0]);
		/// <summary> Sends a message to the provided channel. </summary>
		public Task<Message[]> SendMessage(string channelId, string text)
			=> SendMessage(channelId, text, new string[0]);
		/// <summary> Sends a message to the provided channel, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task<Message[]> SendMessage(Channel channel, string text, string[] mentions)
			=> SendMessage(channel?.Id, text, mentions);
		/// <summary> Sends a message to the provided channel, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public async Task<Message[]> SendMessage(string channelId, string text, string[] mentions)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (text == null) throw new ArgumentNullException(nameof(text));
			if (mentions == null) throw new ArgumentNullException(nameof(mentions));

			int blockCount = (int)Math.Ceiling(text.Length / (double)DiscordAPIClient.MaxMessageSize);
			Message[] result = new Message[blockCount];
			for (int i = 0; i < blockCount; i++)
			{
				int index = i * DiscordAPIClient.MaxMessageSize;
				string blockText = text.Substring(index, Math.Min(2000, text.Length - index));
				var nonce = GenerateNonce();
				if (_config.UseMessageQueue)
				{
					var msg = _messages.GetOrAdd("nonce_" + nonce, channelId, _currentUserId);
                    msg.Update(new Net.API.Message
					{
						Content = blockText,
						Timestamp = DateTime.UtcNow,
						Author = new UserReference { Avatar = _currentUser.AvatarId, Discriminator = _currentUser.Discriminator, Id = _currentUser.Id, Username = _currentUser.Name },
						ChannelId = channelId
					});
					msg.IsQueued = true;
					msg.Nonce = nonce;
					result[i] = msg;
					_pendingMessages.Enqueue(msg);
				}
				else
				{
					var model = await _api.SendMessage(channelId, blockText, mentions, nonce).ConfigureAwait(false);
					var msg = _messages.GetOrAdd(model.Id, channelId, model.Author.Id);
					msg.Update(model);
					RaiseMessageSent(msg);
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

		/// <summary> Edits a message the provided message. </summary>
		public Task EditMessage(Message message, string text)
			=> EditMessage(message?.ChannelId, message?.Id, text, new string[0]);
		/// <summary> Edits a message the provided message. </summary>
		public Task EditMessage(Channel channel, string messageId, string text)
			=> EditMessage(channel?.Id, messageId, text, new string[0]);
		/// <summary> Edits a message the provided message. </summary>
		public Task EditMessage(string channelId, string messageId, string text)
			=> EditMessage(channelId, messageId, text, new string[0]);
		/// <summary> Edits a message the provided message, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public Task EditMessage(Message message, string text, string[] mentions)
			=> EditMessage(message?.ChannelId, message?.Id, text, mentions);
		/// <summary> Edits a message the provided message, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public Task EditMessage(Channel channel, string messageId, string text, string[] mentions)
			=> EditMessage(channel?.Id, messageId, text, mentions);
		/// <summary> Edits a message the provided message, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public async Task EditMessage(string channelId, string messageId, string text, string[] mentions)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (messageId == null) throw new ArgumentNullException(nameof(messageId));
			if (text == null) throw new ArgumentNullException(nameof(text));
			if (mentions == null) throw new ArgumentNullException(nameof(mentions));

			if (text.Length > DiscordAPIClient.MaxMessageSize)
				text = text.Substring(0, DiscordAPIClient.MaxMessageSize);

			var model = await _api.EditMessage(channelId, messageId, text, mentions).ConfigureAwait(false);
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
				await _api.DeleteMessage(channelId, msgId).ConfigureAwait(false);
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
					await _api.DeleteMessage(msg.ChannelId, msg.Id).ConfigureAwait(false);
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
					await _api.DeleteMessage(channelId, msgId).ConfigureAwait(false);
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			}
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
								msg.Update(x);
							if (_config.TrackActivity)
							{
								var user = _users[x.Author.Id];
								if (user != null)
									user.UpdateActivity(x.Timestamp);
							}
							return msg;
						})
						.ToArray();
				}
				catch (HttpException) { } //Bad Permissions?
			}
			return null;
		}

		//Roles
		/// <summary>Note: due to current API limitations, the created role cannot be returned. </summary>
		public Task CreateRole(Server server)
			=> CreateRole(server?.Id);
		/// <summary>Note: due to current API limitations, the created role cannot be returned. </summary>
		public Task CreateRole(string serverId)
		{
			CheckReady();
			if (serverId == null) throw new NullReferenceException(nameof(serverId));

			return _api.CreateRole(serverId);
		}

		public Task RenameRole(Role role, string newName)
			=> RenameRole(role?.ServerId, role?.Id, newName);
        public Task RenameRole(string serverId, string roleId, string newName)
		{
			CheckReady();
			if (roleId == null) throw new NullReferenceException(nameof(roleId));
			if (newName == null) throw new NullReferenceException(nameof(newName));

			return _api.RenameRole(serverId, roleId, newName);
		}

		public Task DeleteRole(Role role)
			=> DeleteRole(role?.ServerId, role?.Id);
		public Task DeleteRole(string serverId, string roleId)
		{
			CheckReady();
			if (roleId == null) throw new NullReferenceException(nameof(roleId));

			return _api.DeleteRole(serverId, roleId);
		}

		public Task AddRoleMember(Role role, string serverId, string userId)
			=> AddRoleMember(role?.Id, GetMember(serverId, userId));
		public Task AddRoleMember(Role role, string serverId, User user)
			=> AddRoleMember(role?.Id, GetMember(serverId, user));
		public Task AddRoleMember(Role role, Server server, string userId)
			=> AddRoleMember(role?.Id, GetMember(server, userId));
		public Task AddRoleMember(Role role, Server server, User user)
			=> AddRoleMember(role?.Id, GetMember(server, user));
		public Task AddRoleMember(Role role, Member member)
			=> AddRoleMember(role?.Id, member);
        public Task AddRoleMember(string roleId, string serverId, string userId)
			=> AddRoleMember(roleId, GetMember(serverId, userId));
        public Task AddRoleMember(string roleId, string serverId, User user)
			=> AddRoleMember(roleId, GetMember(serverId, user));
		public Task AddRoleMember(string roleId, Server server, string userId)
			=> AddRoleMember(roleId, GetMember(server, userId));
		public Task AddRoleMember(string roleId, Server server, User user)
			=> AddRoleMember(roleId, GetMember(server, user));
        public Task AddRoleMember(string roleId, Member member)
		{
			CheckReady();
			if (roleId == null) throw new NullReferenceException(nameof(roleId));
			if (member == null) throw new NullReferenceException(nameof(member));

			if (!member.RoleIds.Contains(roleId))
			{
				var oldRoles = member.RoleIds;
                string[] newRoles = new string[oldRoles.Length + 1];
				for (int i = 0; i < oldRoles.Length; i++)
					newRoles[i] = oldRoles[i];
				return _api.SetMemberRoles(member.ServerId, member.UserId, newRoles);
			}
			return TaskHelper.CompletedTask;
		}

		public Task RemoveRoleMember(Role role, string serverId, string userId)
			=> RemoveRoleMember(role?.Id, GetMember(serverId, userId));
		public Task RemoveRoleMember(Role role, string serverId, User user)
			=> RemoveRoleMember(role?.Id, GetMember(serverId, user));
		public Task RemoveRoleMember(Role role, Server server, string userId)
			=> RemoveRoleMember(role?.Id, GetMember(server, userId));
		public Task RemoveRoleMember(Role role, Server server, User user)
			=> RemoveRoleMember(role?.Id, GetMember(server, user));
		public Task RemoveRoleMember(Role role, Member member)
			=> RemoveRoleMember(role?.Id, member);
		public Task RemoveRoleMember(string roleId, string serverId, string userId)
			=> RemoveRoleMember(roleId, GetMember(serverId, userId));
		public Task RemoveRoleMember(string roleId, string serverId, User user)
			=> RemoveRoleMember(roleId, GetMember(serverId, user));
		public Task RemoveRoleMember(string roleId, Server server, string userId)
			=> RemoveRoleMember(roleId, GetMember(server, userId));
		public Task RemoveRoleMember(string roleId, Server server, User user)
			=> RemoveRoleMember(roleId, GetMember(server, user));
		public Task RemoveRoleMember(string roleId, Member member)
		{
			CheckReady();
			if (roleId == null) throw new NullReferenceException(nameof(roleId));
			if (member == null) throw new NullReferenceException(nameof(member));

			if (member.RoleIds.Contains(roleId))
			{
				var oldRoles = member.RoleIds;
				string[] newRoles = new string[oldRoles.Length - 1];
				for (int i = 0, j = 0; i < oldRoles.Length; i++)
				{
					if (oldRoles[i] != roleId)
						newRoles[j++] = oldRoles[i];
				}
				return _api.SetMemberRoles(member.ServerId, member.UserId, newRoles);
			}
			return TaskHelper.CompletedTask;
		}

		//Permissions
		public Task SetChannelUserPermissions(Channel channel, Member member, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channel?.Id, member?.UserId, "member", allow, deny);
		public Task SetChannelUserPermissions(string channelId, Member member, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channelId, member?.UserId, "member", allow, deny);
		public Task SetChannelUserPermissions(Channel channel, User user, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channel?.Id, user?.Id, "member", allow, deny);
		public Task SetChannelUserPermissions(string channelId, User user, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channelId, user?.Id, "member", allow, deny);
		public Task SetChannelUserPermissions(Channel channel, string userId, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channel?.Id, userId, "member", allow, deny);
		public Task SetChannelUserPermissions(string channelId, string userId, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channelId, userId, "member", allow, deny);

		public Task SetChannelRolePermissions(Channel channel, Role role, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channel?.Id, role?.Id, "role", allow, deny);
		public Task SetChannelRolePermissions(string channelId, Role role, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channelId, role?.Id, "role", allow, deny);
		public Task SetChannelRolePermissions(Channel channel, string userId, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channel?.Id, userId, "role", allow, deny);
		public Task SetChannelRolePermissions(string channelId, string userId, PackedPermissions allow, PackedPermissions deny)
			=> SetChannelPermissions(channelId, userId, "role", allow, deny);

		private Task SetChannelPermissions(string channelId, string userOrRoleId, string idType, PackedPermissions allow, PackedPermissions deny)
		{
			CheckReady();
			if (channelId == null) throw new NullReferenceException(nameof(channelId));
			if (userOrRoleId == null) throw new NullReferenceException(nameof(userOrRoleId));

			return _api.SetChannelPermissions(channelId, userOrRoleId, idType, allow, deny);
			//TODO: Remove permission from cache
		}

		public Task RemoveChannelUserPermissions(Channel channel, Member member)
			=> RemoveChannelPermissions(channel?.Id, member?.UserId);
		public Task RemoveChannelUserPermissions(string channelId, Member member)
			=> RemoveChannelPermissions(channelId, member?.UserId);
		public Task RemoveChannelUserPermissions(Channel channel, User user)
			=> RemoveChannelPermissions(channel?.Id, user?.Id);
		public Task RemoveChannelUserPermissions(string channelId, User user)
			=> RemoveChannelPermissions(channelId, user?.Id);
		public Task RemoveChannelUserPermissions(Channel channel, string userId)
			=> RemoveChannelPermissions(channel?.Id, userId);
		public Task RemoveChannelUserPermissions(string channelId, string userId)
			=> RemoveChannelPermissions(channelId, userId);

		public Task RemoveChannelRolePermissions(Channel channel, Role role)
			=> RemoveChannelPermissions(channel?.Id, role?.Id);
		public Task RemoveChannelRolePermissions(string channelId, Role role)
			=> RemoveChannelPermissions(channelId, role?.Id);
		public Task RemoveChannelRolePermissions(Channel channel, string roleId)
			=> RemoveChannelUserPermissions(channel?.Id, roleId);
		public Task RemoveChannelRolePermissions(string channelId, string roleId)
			=> RemoveChannelPermissions(channelId, roleId);

		private async Task RemoveChannelPermissions(string channelId, string userOrRoleId)
		{
			CheckReady();
			if (channelId == null) throw new NullReferenceException(nameof(channelId));
			if (userOrRoleId == null) throw new NullReferenceException(nameof(userOrRoleId));

			try
			{
				await _api.DeleteChannelPermissions(channelId, userOrRoleId).ConfigureAwait(false);
				//TODO: Remove permission from cache
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}

		//Voice
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(Member member)
			=> Mute(member?.ServerId, member?.UserId);
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(Server server, User user)
			=> Mute(server?.Id, user?.Id);
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(Server server, string userId)
			=> Mute(server?.Id, userId);
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(string server, User user)
			=> Mute(server, user?.Id);
		/// <summary> Mutes a user on the provided server. </summary>
		public Task Mute(string serverId, string userId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _api.Mute(serverId, userId);
		}

		/// <summary> Mutes a user on the provided server. </summary>
		public Task Unmute(Member member)
			=> Unmute(member?.ServerId, member?.UserId);
		/// <summary> Unmutes a user on the provided server. </summary>
		public Task Unmute(Server server, User user)
			=> Unmute(server?.Id, user?.Id);
		/// <summary> Unmutes a user on the provided server. </summary>
		public Task Unmute(Server server, string userId)
			=> Unmute(server?.Id, userId);
		/// <summary> Unmutes a user on the provided server. </summary>
		public Task Unmute(string server, User user)
			=> Unmute(server, user?.Id);
		/// <summary> Unmutes a user on the provided server. </summary>
		public Task Unmute(string serverId, string userId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _api.Unmute(serverId, userId);
		}

		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(Member member)
			=> Deafen(member?.ServerId, member?.UserId);
		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(Server server, User user)
			=> Deafen(server?.Id, user?.Id);
		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(Server server, string userId)
			=> Deafen(server?.Id, userId);
		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(string server, User user)
			=> Deafen(server, user?.Id);
		/// <summary> Deafens a user on the provided server. </summary>
		public Task Deafen(string serverId, string userId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _api.Deafen(serverId, userId);
		}

		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(Member member)
			=> Undeafen(member?.ServerId, member?.UserId);
		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(Server server, User user)
			=> Undeafen(server?.Id, user?.Id);
		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(Server server, string userId)
			=> Undeafen(server?.Id, userId);
		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(string server, User user)
			=> Undeafen(server, user?.Id);
		/// <summary> Undeafens a user on the provided server. </summary>
		public Task Undeafen(string serverId, string userId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (userId == null) throw new ArgumentNullException(nameof(userId));

			return _api.Undeafen(serverId, userId);
		}

		//Profile
		/// <summary> Changes your username to newName. </summary>
		public async Task ChangeUsername(string newName, string currentEmail, string currentPassword)
		{
			CheckReady();
			var response = await _api.ChangeUsername(newName, currentEmail, currentPassword).ConfigureAwait(false);
			_currentUser.Update(response);
		}
		/// <summary> Changes your email to newEmail. </summary>
		public async Task ChangeEmail(string newEmail, string currentPassword)
		{
			CheckReady();
			var response = await _api.ChangeEmail(newEmail, currentPassword).ConfigureAwait(false);
			_currentUser.Update(response);
		}
		/// <summary> Changes your password to newPassword. </summary>
		public async Task ChangePassword(string newPassword, string currentEmail, string currentPassword)
		{
			CheckReady();
			var response = await _api.ChangePassword(newPassword, currentEmail, currentPassword).ConfigureAwait(false);
			_currentUser.Update(response);
		}

		/// <summary> Changes your avatar. </summary>
		/// <remarks>Only supports PNG and JPEG (see AvatarImageType)</remarks>
		public async Task ChangeAvatar(AvatarImageType imageType, byte[] bytes, string currentEmail, string currentPassword)
		{
			CheckReady();
			var response = await _api.ChangeAvatar(imageType, bytes, currentEmail, currentPassword).ConfigureAwait(false);
			_currentUser.Update(response);
		}
	}
}
