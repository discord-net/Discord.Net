using Discord.API;
using Discord.API.Models;
using System;
using System.IO;
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

			var response = await _api.CreateServer(name, region);
			return _servers.Update(response.Id, response);
		}

		/// <summary> Leaves the provided server, destroying it if you are the owner. </summary>
		public Task<Server> LeaveServer(Server server)
			=> LeaveServer(server?.Id);
		/// <summary> Leaves the provided server, destroying it if you are the owner. </summary>
		public async Task<Server> LeaveServer(string serverId)
		{
			CheckReady();
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));

			try { await _api.LeaveServer(serverId); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			return _servers.Remove(serverId);
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

			var response = await _api.CreateChannel(serverId, name, type);
			return _channels.Update(response.Id, serverId, response);
		}

		/// <summary> Destroys the provided channel. </summary>
		public Task<Channel> DestroyChannel(Channel channel)
			=> DestroyChannel(channel?.Id);
		/// <summary> Destroys the provided channel. </summary>
		public async Task<Channel> DestroyChannel(string channelId)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			try { await _api.DestroyChannel(channelId); }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			return _channels.Remove(channelId);
		}

		//Bans
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

			try { await _api.Unban(serverId, userId); }
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

			var response = await _api.CreateInvite(channelId, maxAge, maxUses, isTemporary, hasXkcdPass);
			_channels.Update(response.Channel.Id, response.Server.Id, response.Channel);
			_servers.Update(response.Server.Id, response.Server);
			_users.Update(response.Inviter.Id, response.Inviter);
			return new Invite(response.Code, response.XkcdPass, this)
			{
				ChannelId = response.Channel.Id,
				InviterId = response.Inviter.Id,
				ServerId = response.Server.Id,
				IsRevoked = response.IsRevoked,
				IsTemporary = response.IsTemporary,
				MaxAge = response.MaxAge,
				MaxUses = response.MaxUses,
				Uses = response.Uses
			};
		}

		/// <summary> Gets more info about the provided invite. </summary>
		/// <remarks> Supported formats: inviteCode, xkcdCode, https://discord.gg/inviteCode, https://discord.gg/xkcdCode </remarks>
		public async Task<Invite> GetInvite(string id)
		{
			CheckReady();
			if (id == null) throw new ArgumentNullException(nameof(id));

			var response = await _api.GetInvite(id);
			return new Invite(response.Code, response.XkcdPass, this)
			{
				ChannelId = response.Channel.Id,
				InviterId = response.Inviter.Id,
				ServerId = response.Server.Id
			};
		}

		/// <summary> Accepts the provided invite. </summary>
		public Task AcceptInvite(Invite invite)
		{
			CheckReady();
			if (invite == null) throw new ArgumentNullException(nameof(invite));

			return _api.AcceptInvite(invite.Id);
		}
		/// <summary> Accepts the provided invite. </summary>
		public async Task AcceptInvite(string id)
		{
			CheckReady();
			if (id == null) throw new ArgumentNullException(nameof(id));

			//Remove Url Parts
			if (id.StartsWith(Endpoints.BaseShortHttps))
				id = id.Substring(Endpoints.BaseShortHttps.Length);
			if (id.Length > 0 && id[0] == '/')
				id = id.Substring(1);
			if (id.Length > 0 && id[id.Length - 1] == '/')
				id = id.Substring(0, id.Length - 1);

			//Check if this is a human-readable link and get its ID
			var response = await _api.GetInvite(id);
			await _api.AcceptInvite(response.Code);
		}

		/// <summary> Deletes the provided invite. </summary>
		public async Task DeleteInvite(string id)
		{
			CheckReady();
			if (id == null) throw new ArgumentNullException(nameof(id));

			try
			{
				//Check if this is a human-readable link and get its ID
				var response = await _api.GetInvite(id);
				await _api.DeleteInvite(response.Code);
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

			int blockCount = (int)Math.Ceiling(text.Length / (double)DiscordAPI.MaxMessageSize);
			Message[] result = new Message[blockCount];
			for (int i = 0; i < blockCount; i++)
			{
				int index = i * DiscordAPI.MaxMessageSize;
				string blockText = text.Substring(index, Math.Min(2000, text.Length - index));
				var nonce = GenerateNonce();
				if (_config.UseMessageQueue)
				{
					var msg = _messages.Update("nonce_" + nonce, channelId, new API.Models.Message
					{
						Content = blockText,
						Timestamp = DateTime.UtcNow,
						Author = new UserReference { Avatar = User.AvatarId, Discriminator = User.Discriminator, Id = User.Id, Username = User.Name },
						ChannelId = channelId
					});
					msg.IsQueued = true;
					msg.Nonce = nonce;
					result[i] = msg;
					_pendingMessages.Enqueue(msg);
				}
				else
				{
					var msg = await _api.SendMessage(channelId, blockText, mentions, nonce);
					result[i] = _messages.Update(msg.Id, channelId, msg);
					result[i].Nonce = nonce;
					try { RaiseMessageSent(result[i]); } catch { }
				}
				await Task.Delay(1000);
			}
			return result;
		}
		/// <summary> Sends a private message to the provided channel. </summary>
		public async Task<Message[]> SendPrivateMessage(User user, string text)
			=> await SendMessage(await GetPMChannel(user), text, new string[0]);
		/// <summary> Sends a private message to the provided channel. </summary>
		public async Task<Message[]> SendPrivateMessage(string userId, string text)
			=> await SendMessage(await GetPMChannel(userId), text, new string[0]);
		/*/// <summary> Sends a private message to the provided user, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public async Task<Message[]> SendPrivateMessage(User user, string text, string[] mentions)
			=> SendMessage(await GetOrCreatePMChannel(user), text, mentions);
		/// <summary> Sends a private message to the provided user, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public async Task<Message[]> SendPrivateMessage(string userId, string text, string[] mentions)
			=> SendMessage(await GetOrCreatePMChannel(userId), text, mentions);*/


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
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task EditMessage(Message message, string text, string[] mentions)
			=> EditMessage(message?.ChannelId, message?.Id, text, mentions);
		/// <summary> Edits a message the provided message, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public Task EditMessage(Channel channel, string messageId, string text, string[] mentions)
			=> EditMessage(channel?.Id, messageId, text, mentions);
		/// <summary> Edits a message the provided message, mentioning certain users. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see User.Mention). </remarks>
		public async Task EditMessage(string channelId, string messageId, string text, string[] mentions)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (messageId == null) throw new ArgumentNullException(nameof(messageId));
			if (text == null) throw new ArgumentNullException(nameof(text));
			if (mentions == null) throw new ArgumentNullException(nameof(mentions));

			if (text.Length > DiscordAPI.MaxMessageSize)
				text = text.Substring(0, DiscordAPI.MaxMessageSize);

			var msg = await _api.EditMessage(channelId, messageId, text, mentions);
			_messages.Update(msg.Id, channelId, msg);
		}

		/// <summary> Deletes the provided message. </summary>
		public Task DeleteMessage(Message msg)
			=> DeleteMessage(msg?.ChannelId, msg?.Id);
		/// <summary> Deletes the provided message. </summary>
		public async Task<Message> DeleteMessage(string channelId, string msgId)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (msgId == null) throw new ArgumentNullException(nameof(msgId));

			try
			{
				await _api.DeleteMessage(channelId, msgId);
				return _messages.Remove(msgId);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError) { } //TODO: Remove me - temporary fix for deleting nonexisting messages
			return null;
		}

		/// <summary> Sends a file to the provided channel. </summary>
		public Task SendFile(Channel channel, string path)
			=> SendFile(channel?.Id, path);
		/// <summary> Sends a file to the provided channel. </summary>
		public Task SendFile(string channelId, string path)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));
			return SendFile(channelId, File.OpenRead(path), Path.GetFileName(path));
		}
		/// <summary> Reads a stream and sends it to the provided channel as a file. </summary>
		/// <remarks> It is highly recommended that this stream be cached in memory or on disk, or the request may time out. </remarks>
		public Task SendFile(Channel channel, Stream stream, string filename = null)
			=> SendFile(channel?.Id, stream, filename);
		/// <summary> Reads a stream and sends it to the provided channel as a file. </summary>
		/// <remarks> It is highly recommended that this stream be cached in memory or on disk, or the request may time out. </remarks>
		public Task SendFile(string channelId, Stream stream, string filename = null)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (stream == null) throw new ArgumentNullException(nameof(stream));
			if (filename == null) throw new ArgumentNullException(nameof(filename));

			return _api.SendFile(channelId, stream, filename);
		}

		/// <summary> Downloads last count messages from the server, starting at beforeMessageId if it's provided. </summary>
		public Task<Message[]> DownloadMessages(Channel channel, int count, string beforeMessageId = null)
			=> DownloadMessages(channel.Id, count);
		/// <summary> Downloads last count messages from the server, starting at beforeMessageId if it's provided. </summary>
		public async Task<Message[]> DownloadMessages(string channelId, int count, string beforeMessageId = null)
		{
			CheckReady();
			if (channelId == null) throw new NullReferenceException(nameof(channelId));
			if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
			if (count == 0) return new Message[0];

			Channel channel = GetChannel(channelId);
			if (channel != null && channel.Type == ChannelTypes.Text)
			{
				try
				{
					var msgs = await _api.GetMessages(channel.Id, count);
					return msgs.OrderBy(x => x.Timestamp)
						.Select(x =>
						{
							var msg = _messages.Update(x.Id, x.ChannelId, x);
							var user = msg.User;
							if (user != null)
								user.UpdateActivity(x.Timestamp);
							return msg;
						})
						.ToArray();
				}
				catch (HttpException) { } //Bad Permissions?
			}
			return null;
		}

		//Voice
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

#if !DNXCORE50
		public Task JoinVoiceServer(Server server, Channel channel)
			=> JoinVoiceServer(server?.Id, channel?.Id);
		public Task JoinVoiceServer(Server server, string channelId)
			=> JoinVoiceServer(server?.Id, channelId);
		public Task JoinVoiceServer(string serverId, Channel channel)
			=> JoinVoiceServer(serverId, channel?.Id);
		public async Task JoinVoiceServer(string serverId, string channelId)
		{
			CheckReady();
			if (!_config.EnableVoice) throw new InvalidOperationException("Voice is not enabled for this client.");
			if (serverId == null) throw new ArgumentNullException(nameof(serverId));
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));

			await LeaveVoiceServer();
			_currentVoiceServerId = serverId;
			_webSocket.JoinVoice(serverId, channelId);
		}

		public async Task LeaveVoiceServer()
		{
			if (!_config.EnableVoice) throw new InvalidOperationException("Voice is not enabled for this client.");

			await _voiceWebSocket.DisconnectAsync();
			if (_currentVoiceServerId != null)
				_webSocket.LeaveVoice();
			_currentVoiceServerId = null;
			_currentVoiceToken = null;
		}

		/// <summary> Sends a PCM frame to the voice server. </summary>
		/// <param name="data">PCM frame to send.</param>
		/// <param name="count">Number of bytes in this frame. </param>
		public void SendVoicePCM(byte[] data, int count)
		{
			CheckReady();
			if (!_config.EnableVoice) throw new InvalidOperationException("Voice is not enabled for this client.");
			if (count == 0) return;

			if (_isDebugMode)
				RaiseOnDebugMessage(DebugMessageType.VoiceOutput, $"Queued {count} bytes for voice output.");
			_voiceWebSocket.SendPCMFrame(data, count);
		}

		/// <summary> Clears the PCM buffer. </summary>
		public void ClearVoicePCM()
		{
			CheckReady();
			if (!_config.EnableVoice) throw new InvalidOperationException("Voice is not enabled for this client.");

			if (_isDebugMode)
				RaiseOnDebugMessage(DebugMessageType.VoiceOutput, $"Cleared the voice buffer.");
			_voiceWebSocket.ClearPCMFrames();
		}
#endif

		//Profile
		/// <summary> Changes your username to newName. </summary>
		public async Task ChangeUsername(string newName, string currentEmail, string currentPassword)
		{
			CheckReady();
			var response = await _api.ChangeUsername(newName, currentEmail, currentPassword);
			_users.Update(response.Id, response);
		}
		/// <summary> Changes your email to newEmail. </summary>
		public async Task ChangeEmail(string newEmail, string currentPassword)
		{
			CheckReady();
			var response = await _api.ChangeEmail(newEmail, currentPassword);
			_users.Update(response.Id, response);
		}
		/// <summary> Changes your password to newPassword. </summary>
		public async Task ChangePassword(string newPassword, string currentEmail, string currentPassword)
		{
			CheckReady();
			var response = await _api.ChangePassword(newPassword, currentEmail, currentPassword);
			_users.Update(response.Id, response);
		}

		/// <summary> Changes your avatar. </summary>
		/// <remarks>Only supports PNG and JPEG (see AvatarImageType)</remarks>
		public async Task ChangeAvatar(AvatarImageType imageType, byte[] bytes, string currentEmail, string currentPassword)
		{
			CheckReady();
			var response = await _api.ChangeAvatar(imageType, bytes, currentEmail, currentPassword);
			_users.Update(response.Id, response);
		}
	}
}
