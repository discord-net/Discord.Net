using Discord.API;
using Discord.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discord
{
	internal sealed class Messages : AsyncCollection<long, Message>
	{
		private bool _isEnabled;

		public Messages(DiscordClient client, object writerLock, bool isEnabled)
			: base(client, writerLock)
		{
			_isEnabled = isEnabled;
        }
		
		public Message GetOrAdd(long id, long channelId, long userId)
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
		public void Import(Dictionary<long, Message> messages)
			=> base.Import(messages);
	}

    internal class MessageQueueItem
    {
        public readonly Message Message;
        public readonly string Text;
        public readonly long[] MentionedUsers;
        public MessageQueueItem(Message msg, string text, long[] userIds)
        {
            Message = msg;
            Text = text;
            MentionedUsers = userIds;
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
				EventHelper.Raise(_logger, nameof(MessageReceived), () => MessageReceived(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageSent;
		private void RaiseMessageSent(Message msg)
		{
			if (MessageSent != null)
				EventHelper.Raise(_logger, nameof(MessageSent), () => MessageSent(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageDeleted;
		private void RaiseMessageDeleted(Message msg)
		{
			if (MessageDeleted != null)
				EventHelper.Raise(_logger, nameof(MessageDeleted), () => MessageDeleted(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageUpdated;
		private void RaiseMessageUpdated(Message msg)
		{
			if (MessageUpdated != null)
				EventHelper.Raise(_logger, nameof(MessageUpdated), () => MessageUpdated(this, new MessageEventArgs(msg)));
		}
		public event EventHandler<MessageEventArgs> MessageAcknowledged;
		private void RaiseMessageAcknowledged(Message msg)
		{
			if (MessageAcknowledged != null)
				EventHelper.Raise(_logger, nameof(MessageAcknowledged), () => MessageAcknowledged(this, new MessageEventArgs(msg)));
		}
		
		internal Messages Messages => _messages;
        private readonly Random _nonceRand;
        private readonly Messages _messages;
        private readonly JsonSerializer _messageImporter;
        private readonly ConcurrentQueue<MessageQueueItem> _pendingMessages;

        /// <summary> Returns the message with the specified id, or null if none was found. </summary>
        public Message GetMessage(long id)
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

            var model = await _api.SendFile(channel.Id, filename, stream).ConfigureAwait(false);
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
				msg = _messages.GetOrAdd(nonce, channel.Id, _userId.Value);
                var currentUser = msg.User;
                msg.Update(new MessageInfo
                {
                    Content = text,
                    Timestamp = DateTime.UtcNow,
                    Author = new UserReference { Avatar = currentUser.AvatarId, Discriminator = currentUser.Discriminator, Id = _userId.Value, Username = currentUser.Name },
                    ChannelId = channel.Id,
                    Nonce = IdConvert.ToString(nonce),
					IsTextToSpeech = isTextToSpeech
				});
				msg.State = MessageState.Queued;
                
				_pendingMessages.Enqueue(new MessageQueueItem(msg, text, mentionedUsers.Select(x => x.Id).ToArray()));
            }
			else
			{
                var model = await _api.SendMessage(channel.Id, text, mentionedUsers.Select(x => x.Id), null, isTextToSpeech).ConfigureAwait(false);
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

			if (text.Length > MaxMessageSize)
				throw new ArgumentOutOfRangeException(nameof(text), $"Message must be {MaxMessageSize} characters or less.");
            
            if (Config.UseMessageQueue)
                _pendingMessages.Enqueue(new MessageQueueItem(message, text, mentionedUsers.Select(x => x.Id).ToArray()));
            else
                await _api.EditMessage(message.Id, message.Channel.Id, text, mentionedUsers.Select(x => x.Id)).ConfigureAwait(false);
		}

		/// <summary> Deletes the provided message. </summary>
		public async Task DeleteMessage(Message message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			CheckReady();

			try
			{
				await _api.DeleteMessage(message.Id, message.Channel.Id).ConfigureAwait(false);
			}
			catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
		}
		public async Task DeleteMessages(IEnumerable<Message> messages)
		{
			if (messages == null) throw new ArgumentNullException(nameof(messages));
			CheckReady();

			foreach (var message in messages)
			{
				try
				{
					await _api.DeleteMessage(message.Id, message.Channel.Id).ConfigureAwait(false);
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
			}
		}

		/// <summary> Downloads last count messages from the server, returning all messages before or after relativeMessageId, if it's provided. </summary>
		public async Task<Message[]> DownloadMessages(Channel channel, int count, long? relativeMessageId = null, RelativeDirection relativeDir = RelativeDirection.Before, bool useCache = true)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			if (count < 0) throw new ArgumentNullException(nameof(count));
			CheckReady();

			bool trackActivity = Config.TrackActivity;

			if (count == 0) return new Message[0];
			if (channel != null && channel.Type == ChannelType.Text)
			{
				try
				{
					var msgs = await _api.GetMessages(channel.Id, count, relativeMessageId, relativeDir).ConfigureAwait(false);
                    return msgs.Select(x =>
					{
						Message msg = null;
						if (useCache)
						{
							msg = _messages.GetOrAdd(x.Id, x.ChannelId, x.Author.Id);
							if (trackActivity)
							{
								var user = msg.User;
								if (user != null)
									user.UpdateActivity(msg.EditedTimestamp ?? msg.Timestamp);
							}
						}
						else
							msg = /*_messages[x.Id] ??*/ new Message(this, x.Id, x.ChannelId, x.Author.Id);
						msg.Update(x);
						return msg;
					})
					.ToArray();
				}
				catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.Forbidden){ } //Bad Permissions
			}
			return new Message[0];
		}
		
		/// <summary> Deserializes messages from JSON format and imports them into the message cache.</summary>
		public IEnumerable<Message> ImportMessages(Channel channel, string json)
		{
			if (json == null) throw new ArgumentNullException(nameof(json));
			
			var dic = JArray.Parse(json)
				.Select(x =>
				{
					var msg = new Message(this, 
						x["Id"].Value<long>(),
						channel.Id,
						x["UserId"].Value<long>());

					var reader = x.CreateReader();
					_messageImporter.Populate(reader, msg);
					msg.Text = Mention.Resolve(msg, msg.RawText);
					return msg;
				})
				.ToDictionary(x => x.Id);
			_messages.Import(dic);
			return dic.Values;
		}

		/// <summary> Serializes the message cache for a given channel to JSON.</summary>
		public string ExportMessages(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));

			return JsonConvert.SerializeObject(channel.Messages);
		}

		private Task MessageQueueAsync()
		{
			var cancelToken = _cancelToken;
			int interval = Config.MessageQueueInterval;

			return Task.Run(async () =>
			{
				MessageQueueItem queuedMessage;

				while (!cancelToken.IsCancellationRequested)
				{
					while (_pendingMessages.TryDequeue(out queuedMessage))
					{
						SendMessageResponse response = null;
                        var msg = queuedMessage.Message;
                        try
                        {
                            response = await _api.SendMessage(
                                    msg.Channel.Id,
                                    queuedMessage.Text,
                                    queuedMessage.MentionedUsers,
                                    IdConvert.ToString(msg.Id), //Nonce
                                    msg.IsTTS)
                                .ConfigureAwait(false);
                        }
                        catch (WebException) { break; }
                        catch (HttpException) { msg.State = MessageState.Failed; }
                        
						RaiseMessageSent(msg);
                    }
                    await Task.Delay(interval).ConfigureAwait(false);
				}
			});
		}
		private long GenerateNonce()
		{
			lock (_nonceRand)
				return -_nonceRand.Next(1, int.MaxValue - 1);
		}
	}
}