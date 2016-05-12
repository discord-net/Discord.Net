using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    public class TextChannel : GuildChannel, ITextChannel
    {
        /// <inheritdoc />
        public string Topic { get; private set; }

        /// <inheritdoc />
        public string Mention => MentionHelper.Mention(this);

        internal TextChannel(Guild guild, Model model)
            : base(guild, model)
        {
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
            var model = await Discord.BaseClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
            Update(model);
        }

        protected override async Task<IEnumerable<GuildUser>> GetUsers()
        {
            var users = await Guild.GetUsers().ConfigureAwait(false);
            return users.Where(x => PermissionUtilities.GetValue(PermissionHelper.Resolve(x, this), ChannelPermission.ReadMessages));
        }

        /// <inheritdoc />
        public Task<Message> GetMessage(ulong id) { throw new NotSupportedException(); } //Not implemented
        /// <inheritdoc />
        public async Task<IEnumerable<Message>> GetMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.BaseClient.GetChannelMessages(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, x));
        }
        /// <inheritdoc />
        public async Task<IEnumerable<Message>> GetMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.BaseClient.GetChannelMessages(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, x));
        }

        /// <inheritdoc />
        public async Task<Message> SendMessage(string text, bool isTTS = false)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.BaseClient.CreateMessage(Guild.Id, Id, args).ConfigureAwait(false);
            return new Message(this, model);
        }
        /// <inheritdoc />
        public async Task<Message> SendFile(string filePath, string text = null, bool isTTS = false)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.BaseClient.UploadFile(Guild.Id, Id, file, args).ConfigureAwait(false);
                return new Message(this, model);
            }
        }
        /// <inheritdoc />
        public async Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false)
        {
            var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.BaseClient.UploadFile(Guild.Id, Id, stream, args).ConfigureAwait(false);
            return new Message(this, model);
        }

        /// <inheritdoc />
        public async Task DeleteMessages(IEnumerable<IMessage> messages)
        {
            await Discord.BaseClient.DeleteMessages(Guild.Id, Id, new DeleteMessagesParam { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task TriggerTyping()
        {
            await Discord.BaseClient.TriggerTypingIndicator(Id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override string ToString() => $"{base.ToString()} [Text]";

        async Task<IMessage> IMessageChannel.GetMessage(ulong id)
            => await GetMessage(id).ConfigureAwait(false);
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
