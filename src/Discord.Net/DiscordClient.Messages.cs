using Discord.API;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Messages : AsyncCollection<Message>
	{
		private bool _isEnabled;

		public Messages(DiscordClient client, object writerLock, bool isEnabled)
			: base(client, writerLock)
		{
			_isEnabled = isEnabled;
        }

		public Message GetOrAdd(string id, string channelId, string userId)
		{
			if (_isEnabled)
				return GetOrAdd(id, () => new Message(_client, id, channelId, userId));
			else
			{
				var msg = new Message(_client, id, channelId, userId);
				msg.Cache(); //Builds references
				return msg;
            }
        }
	}

	public class MessageEventArgs : EventArgs
	{
		public Message Message { get; }
		public User User => Message.User;
		public Channel Channel => Message.Channel;
		public Server Server => Message.Server;

		public MessageEventArgs(Message msg) { Message = msg; }
	}

	public partial class DiscordClient
	{
		public const int MaxMessageSize = 2000;

		public event EventHandler<MessageEventArgs> MessageReceived;
		private void RaiseMessageReceived(Message msg)
		{
			if (MessageReceived != null)
				RaiseEvent(nameof(MessageReceived), () => MessageReceived(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageSent;
		private void RaiseMessageSent(Message msg)
		{
			if (MessageSent != null)
				RaiseEvent(nameof(MessageSent), () => MessageSent(this, new MessageEventArgs(msg)));
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
		
		internal Messages Messages => _messages;
		private readonly Messages _messages;

		/// <summary> Returns the message with the specified id, or null if none was found. </summary>
		public Message GetMessage(string id)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			CheckReady();

			return _messages[id];
		}

		/// <summary> Sends a message to the provided channel. To include a mention, see the Mention static helper class. </summary>
		public Task<Message> SendMessage(Channel channel, string text)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (text == null) throw new ArgumentNullException(nameof(text));
			CheckReady();

			return SendMessage(channel, text, false);
		}
		/// <summary> Sends a text-to-speech message to the provided channel. To include a mention, see the Mention static helper class. </summary>
		public Task<Message> SendTTSMessage(Channel channel, string text)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (text == null) throw new ArgumentNullException(nameof(text));
			CheckReady();

			return SendMessage(channel, text, false);
        }
		/// <summary> Sends a private message to the provided user. </summary>
		public async Task<Message> SendPrivateMessage(User user, string text)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (text == null) throw new ArgumentNullException(nameof(text));
            CheckReady();

			var channel = await CreatePMChannel(user).ConfigureAwait(false);
			return await SendMessage(channel, text).ConfigureAwait(false);
		}
		private async Task<Message> SendMessage(Channel channel, string text, bool isTextToSpeech)
		{
			Message msg;
			var server = channel.Server;

			if (Config.UseMessageQueue)
			{
				var nonce = GenerateNonce();
				msg = _messages.GetOrAdd("nonce_" + nonce, channel.Id, _userId);
                var currentUser = msg.User;
				msg.Update(new MessageInfo
				{
					Content = text,
                    Timestamp = DateTime.UtcNow,
					Author = new UserReference { Avatar = currentUser.AvatarId, Discriminator = currentUser.Discriminator, Id = _userId, Username = currentUser.Name },
					ChannelId = channel.Id,
					IsTextToSpeech = isTextToSpeech
				});
				msg.Nonce = nonce;
				msg.IsQueued = true;

				if (text.Length > MaxMessageSize)
					throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {MaxMessageSize} characters or less.");

				_pendingMessages.Enqueue(msg);
			}
			else
			{
				var mentionedUsers = new List<User>();
				if (!channel.IsPrivate)
					text = Mention.CleanUserMentions(this, server, text, mentionedUsers);

				if (text.Length > MaxMessageSize)
					throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {MaxMessageSize} characters or less.");

				var model = await _api.SendMessage(channel.Id, text, mentionedUsers.Select(x => x.Id), null, isTextToSpeech).ConfigureAwait(false);
				msg = _messages.GetOrAdd(model.Id, channel.Id, model.Author.Id);
				msg.Update(model);
				RaiseMessageSent(msg);
			}
			return msg;
		}


		/// <summary> Sends a file to the provided channel. </summary>
		public Task SendFile(Channel channel, string filePath)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (filePath == null) throw new ArgumentNullException(nameof(filePath));
			CheckReady();

			return _api.SendFile(channel.Id, filePath);
		}

		/// <summary> Edits the provided message, changing only non-null attributes. </summary>
		/// <remarks> While not required, it is recommended to include a mention reference in the text (see Mention.User). </remarks>
		public Task EditMessage(Message message, string text)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (text == null) throw new ArgumentNullException(nameof(text));
			CheckReady();

			var channel = message.Channel;
			var mentionedUsers = new List<User>();
			if (!channel.IsPrivate)
				text = Mention.CleanUserMentions(this, channel.Server, text, mentionedUsers);

			if (text.Length > MaxMessageSize)
				throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {MaxMessageSize} characters or less.");

			return _api.EditMessage(message.Id, message.Channel.Id, text, mentionedUsers.Select(x => x.Id));
		}

		/// <summary> Deletes the provided message. </summary>
		public Task DeleteMessage(Message message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			CheckReady();

			return DeleteMessageInternal(message);
        }
		public async Task DeleteMessages(IEnumerable<Message> messages)
		{
			if (messages == null) throw new ArgumentNullException(nameof(messages));
			CheckReady();

			foreach (var message in messages)
				await DeleteMessageInternal(message).ConfigureAwait(false);
		}
		private async Task DeleteMessageInternal(Message message)
		{
			try
			{
				await _api.DeleteMessage(message.Id, message.Channel.Id).ConfigureAwait(false);
				_messages.TryRemove(message.Id);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}


		/// <summary> Downloads last count messages from the server, returning all messages before or after relativeMessageId, if it's provided. </summary>
		public async Task<Message[]> DownloadMessages(Channel channel, int count, string relativeMessageId = null, RelativeDirection relativeDir = RelativeDirection.Before, bool cache = false)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (count < 0) throw new ArgumentNullException(nameof(count));
			CheckReady();

			if (count == 0) return new Message[0];
			if (channel != null && channel.Type == ChannelType.Text)
			{
				try
				{
					var msgs = await _api.GetMessages(channel.Id, count, relativeMessageId, relativeDir).ConfigureAwait(false);
                    var result = msgs.Select(x =>
					{
						Message msg = null;
						if (cache)
							msg = _messages.GetOrAdd(x.Id, x.ChannelId, x.Author.Id);
						else
						{
							msg = _messages[x.Id] ?? new Message(this, x.Id, x.ChannelId, x.Author.Id);
							msg.Update(x); //TODO: Look into updating when cache is true, but only if we actually generated a new message.
						}
						if (Config.TrackActivity)
						{
							if (!channel.IsPrivate)
							{
								var user = msg.User;
								if (user != null)
									user.UpdateActivity(msg.EditedTimestamp ?? msg.Timestamp);
							}
						}
						return msg;
					})
					.ToArray();
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.Forbidden){ } //Bad Permissions
			}
			return null;
		}

		private Task MessageQueueLoop()
		{
			var cancelToken = _cancelToken;
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
							response = await _api.SendMessage(msg.Channel.Id, msg.RawText, msg.MentionedUsers.Select(x => x.Id), msg.Nonce, msg.IsTTS).ConfigureAwait(false);
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