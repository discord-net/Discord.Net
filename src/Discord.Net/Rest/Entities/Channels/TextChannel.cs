using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class TextChannel : GuildChannel, ITextChannel
    {
        /// <inheritdoc />
        public string Topic { get; private set; }

        /// <inheritdoc />
        public string Mention => MentionUtils.Mention(this);

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
            var model = await Discord.ApiClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
            Update(model);
        }

        /// <summary> Gets a user in this channel with the given id. </summary>
        public async Task<GuildUser> GetUser(ulong id)
        {
            var user = await Guild.GetUser(id).ConfigureAwait(false);
            if (user != null && Permissions.GetValue(Permissions.ResolveChannel(user, this, user.GuildPermissions.RawValue), ChannelPermission.ReadMessages))
                return user;
            return null;
        }
        /// <summary> Gets all users in this channel. </summary>
        public async Task<IEnumerable<GuildUser>> GetUsers()
        {
            var users = await Guild.GetUsers().ConfigureAwait(false);
            return users.Where(x => Permissions.GetValue(Permissions.ResolveChannel(x, this, x.GuildPermissions.RawValue), ChannelPermission.ReadMessages));
        }
        /// <summary> Gets a paginated collection of users in this channel. </summary>
        public async Task<IEnumerable<GuildUser>> GetUsers(int limit, int offset)
        {
            var users = await Guild.GetUsers(limit, offset).ConfigureAwait(false);
            return users.Where(x => Permissions.GetValue(Permissions.ResolveChannel(x, this, x.GuildPermissions.RawValue), ChannelPermission.ReadMessages));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Message>> GetMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessages(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, x));
        }
        /// <inheritdoc />
        public async Task<IEnumerable<Message>> GetMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            var args = new GetChannelMessagesParams { Limit = limit };
            var models = await Discord.ApiClient.GetChannelMessages(Id, args).ConfigureAwait(false);
            return models.Select(x => new Message(this, x));
        }

        /// <inheritdoc />
        public async Task<Message> SendMessage(string text, bool isTTS = false)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.CreateMessage(Guild.Id, Id, args).ConfigureAwait(false);
            return new Message(this, model);
        }
        /// <inheritdoc />
        public async Task<Message> SendFile(string filePath, string text = null, bool isTTS = false)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.ApiClient.UploadFile(Guild.Id, Id, file, args).ConfigureAwait(false);
                return new Message(this, model);
            }
        }
        /// <inheritdoc />
        public async Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false)
        {
            var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.UploadFile(Guild.Id, Id, stream, args).ConfigureAwait(false);
            return new Message(this, model);
        }

        /// <inheritdoc />
        public async Task DeleteMessages(IEnumerable<IMessage> messages)
        {
            await Discord.ApiClient.DeleteMessages(Guild.Id, Id, new DeleteMessagesParam { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task TriggerTyping()
        {
            await Discord.ApiClient.TriggerTypingIndicator(Id).ConfigureAwait(false);
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";


        protected override Task<GuildUser> GetUserInternal(ulong id) => GetUser(id);
        protected override Task<IEnumerable<GuildUser>> GetUsersInternal() => GetUsers();
        protected override Task<IEnumerable<GuildUser>> GetUsersInternal(int limit, int offset) => GetUsers(limit, offset);

        IEnumerable<IMessage> IMessageChannel.CachedMessages => Array.Empty<Message>();

        Task<IMessage> IMessageChannel.GetCachedMessage(ulong id)
            => Task.FromResult<IMessage>(null);
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
