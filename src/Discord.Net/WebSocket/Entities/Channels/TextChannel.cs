using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class TextChannel : GuildChannel, ITextChannel
    {
        private readonly MessageCache _messages;

        /// <inheritdoc />
        public string Topic { get; private set; }

        /// <inheritdoc />
        public string Mention => MentionUtils.Mention(this);
        public override IEnumerable<GuildUser> Users
            => _permissions.Members.Where(x => x.Permissions.ReadMessages).Select(x => x.User).ToImmutableArray();
        public IEnumerable<Message> CachedMessages => _messages.Messages;

        internal TextChannel(Guild guild, Model model)
            : base(guild, model)
        {
            _messages = new MessageCache(Discord, this);
        }

        internal override void Update(Model model)
        {
            Topic = model.Topic;
            base.Update(model);
        }

        public async Task Modify(Action<ModifyTextChannelParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyTextChannelParams();
            func(args);
            await Discord.APIClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
        }

        /// <summary> Gets the message from this channel's cache with the given id, or null if none was found. </summary>
        public Message GetCachedMessage(ulong id)
        {
            return _messages.Get(id);
        }
        /// <summary> Gets the last N messages from this message channel. </summary>
        public async Task<IEnumerable<Message>> GetMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            return await _messages.Download(null, Direction.Before, limit).ConfigureAwait(false);
        }
        /// <summary> Gets a collection of messages in this channel. </summary>
        public async Task<IEnumerable<Message>> GetMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            return await _messages.Download(fromMessageId, dir, limit).ConfigureAwait(false);
        }

        public override GuildUser GetUser(ulong id)
        {
            var member = _permissions.Get(id);
            if (member != null && member.Value.Permissions.ReadMessages)
                return member.Value.User;
            return null;
        }

        /// <inheritdoc />
        public async Task<Message> SendMessage(string text, bool isTTS = false)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.APIClient.CreateMessage(Guild.Id, Id, args).ConfigureAwait(false);
            return new Message(this, model);
        }
        /// <inheritdoc />
        public async Task<Message> SendFile(string filePath, string text = null, bool isTTS = false)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.APIClient.UploadFile(Guild.Id, Id, file, args).ConfigureAwait(false);
                return new Message(this, model);
            }
        }
        /// <inheritdoc />
        public async Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false)
        {
            var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.APIClient.UploadFile(Guild.Id, Id, stream, args).ConfigureAwait(false);
            return new Message(this, model);
        }

        /// <inheritdoc />
        public async Task DeleteMessages(IEnumerable<IMessage> messages)
        {
            await Discord.APIClient.DeleteMessages(Guild.Id, Id, new DeleteMessagesParam { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task TriggerTyping()
        {
            await Discord.APIClient.TriggerTypingIndicator(Id).ConfigureAwait(false);
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";

        IEnumerable<IMessage> IMessageChannel.CachedMessages => CachedMessages;

        Task<IMessage> IMessageChannel.GetCachedMessage(ulong id)
            => Task.FromResult<IMessage>(GetCachedMessage(id));
        async Task<IEnumerable<IMessage>> IMessageChannel.GetMessages(int limit)
            => await GetMessages(limit).ConfigureAwait(false);
        async Task<IEnumerable<IMessage>> IMessageChannel.GetMessages(ulong fromMessageId, Direction dir, int limit)
            => await GetMessages(fromMessageId, dir, limit).ConfigureAwait(false);
        async Task<IMessage> IMessageChannel.SendMessage(string text, bool isTTS)
            => await SendMessage(text, isTTS).ConfigureAwait(false);
        async Task<IMessage> IMessageChannel.SendFile(string filePath, string text, bool isTTS)
            => await SendFile(filePath, text, isTTS).ConfigureAwait(false);
        async Task<IMessage> IMessageChannel.SendFile(Stream stream, string filename, string text, bool isTTS)
            => await SendFile(stream, filename, text, isTTS).ConfigureAwait(false);
        async Task IMessageChannel.TriggerTyping()
            => await TriggerTyping().ConfigureAwait(false);
    }
}
