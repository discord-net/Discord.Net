using Discord.API;
using Discord.Collections;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	public partial class DiscordClient
	{
		public const int MaxMessageSize = 2000;

		/// <summary> Returns a collection of all messages this client has seen since logging in and currently has in cache. </summary>
		public Messages Messages => _messages;
		private readonly Messages _messages;

		public event EventHandler<MessageEventArgs> MessageCreated;
		private void RaiseMessageCreated(Message msg)
		{
			if (MessageCreated != null)
				RaiseEvent(nameof(MessageCreated), () => MessageCreated(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageDeleted;
		private void RaiseMessageDeleted(Message msg)
		{
			if (MessageDeleted != null)
				RaiseEvent(nameof(MessageDeleted), () => MessageDeleted(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageUpdated;
		private void RaiseMessageUpdated(Message msg)
		{
			if (MessageUpdated != null)
				RaiseEvent(nameof(MessageUpdated), () => MessageUpdated(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageReadRemotely;
		private void RaiseMessageReadRemotely(Message msg)
		{
			if (MessageReadRemotely != null)
				RaiseEvent(nameof(MessageReadRemotely), () => MessageReadRemotely(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageSent;
		private void RaiseMessageSent(Message msg)
		{
			if (MessageSent != null)
				RaiseEvent(nameof(MessageSent), () => MessageSent(this, new MessageEventArgs(msg)));
		}

		/// <summary> Returns the message with the specified id, or null if none was found. </summary>
		public Message GetMessage(string id) => _messages[id];

		/// <summary> Sends a message to the provided channel. To include a mention, see the Mention static helper class. </summary>
		public Task<Message[]> SendMessage(Channel channel, string text)
			=> SendMessage(channel, text, MentionHelper.GetUserIds(text), false);
		/// <summary> Sends a message to the provided channel. To include a mention, see the Mention static helper class. </summary>
		public Task<Message[]> SendMessage(string channelId, string text)
			=> SendMessage(_channels[channelId], text, MentionHelper.GetUserIds(text), false);
		private async Task<Message[]> SendMessage(Channel channel, string text, IEnumerable<object> mentionedUsers = null, bool isTextToSpeech = false)
		{
			CheckReady();
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (text == null) throw new ArgumentNullException(nameof(text));
			var mentionedUserIds = CollectionHelper.FlattenUsers(mentionedUsers);

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
					var currentUser = msg.User;
					msg.Update(new MessageInfo
					{
						Content = blockText,
						Timestamp = DateTime.UtcNow,
						Author = new UserReference { Avatar = currentUser.AvatarId, Discriminator = currentUser.Discriminator, Id = CurrentUserId, Username = currentUser.Name },
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
					var model = await _api.SendMessage(channel.Id, blockText, mentionedUserIds, nonce, isTextToSpeech).ConfigureAwait(false);
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
		public Task EditMessage(Message message, string text = null, IEnumerable<object> mentionedUsers = null)
			=> EditMessage(message?.ChannelId, message?.Id, text, mentionedUsers);
		/// <summary> Edits the provided message, changing only non-null attributes. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public Task EditMessage(Channel channel, string messageId, string text = null, IEnumerable<object> mentionedUsers = null)
			=> EditMessage(channel?.Id, messageId, text, mentionedUsers);
		/// <summary> Edits the provided message, changing only non-null attributes. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public async Task EditMessage(string channelId, string messageId, string text = null, IEnumerable<object> mentionedUsers = null)
		{
			CheckReady();
			if (channelId == null) throw new ArgumentNullException(nameof(channelId));
			if (messageId == null) throw new ArgumentNullException(nameof(messageId));
			var mentionedUserIds = CollectionHelper.FlattenUsers(mentionedUsers);

			if (text != null && text.Length > MaxMessageSize)
				text = text.Substring(0, MaxMessageSize);

			var model = await _api.EditMessage(messageId, channelId, text, mentionedUserIds).ConfigureAwait(false);
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
								/*if (channel.IsPrivate)
								{
									var user = msg.User;
									if (user != null)
										user.UpdateActivity(msg.EditedTimestamp ?? msg.Timestamp);
								}
								else*/
								if (!channel.IsPrivate)
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

		private Task MessageQueueLoop()
		{
			var cancelToken = CancelToken;
			int interval = Config.MessageQueueInterval;

			return Task.Run(async () =>
			{
				Message msg;
				while (!cancelToken.IsCancellationRequested)
				{
					while (_pendingMessages.TryDequeue(out msg))
					{
						bool hasFailed = false;
						SendMessageResponse response = null;
						try
						{
							response = await _api.SendMessage(msg.ChannelId, msg.RawText, msg.MentionIds, msg.Nonce, msg.IsTTS).ConfigureAwait(false);
						}
						catch (WebException) { break; }
						catch (HttpException) { hasFailed = true; }

						if (!hasFailed)
						{
							_messages.Remap(msg.Id, response.Id);
							msg.Id = response.Id;
							msg.Update(response);
						}
						msg.IsQueued = false;
						msg.HasFailed = hasFailed;
						RaiseMessageSent(msg);
					}
					await Task.Delay(interval).ConfigureAwait(false);
				}
			});
		}
		private string GenerateNonce()
		{
			lock (_rand)
				return _rand.Next().ToString();
		}
	}
}