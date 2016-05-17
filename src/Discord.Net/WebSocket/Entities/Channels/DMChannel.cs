using Discord.API.Rest;
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
    public class DMChannel : Channel, IDMChannel
    {
        private readonly MessageCache _messages;
        
        internal DiscordClient Discord { get; }

        /// <inheritdoc />
        public User Recipient { get; private set; }
        
        /// <inheritdoc />
        public new IEnumerable<User> Users => ImmutableArray.Create(Discord.CurrentUser, Recipient);
        public IEnumerable<Message> CachedMessages => _messages.Messages;

        internal DMChannel(DiscordClient discord, User recipient, Model model)
            : base(model.Id)
        {
            Discord = discord;
            Recipient = recipient;
            _messages = new MessageCache(Discord, this);

            Update(model);
        }
        private void Update(Model model)
        {
            Recipient.Update(model.Recipient);
        }

        protected override User GetUserInternal(ulong id)
        {
            if (id == Recipient.Id)
                return Recipient;
            else if (id == Discord.CurrentUser.Id)
                return Discord.CurrentUser;
            else
                return null;
        }
        protected override IEnumerable<User> GetUsersInternal()
        {
            return Users;
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

        /// <inheritdoc />
        public async Task<Message> SendMessage(string text, bool isTTS = false)
        {
            var args = new CreateMessageParams { Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.CreateDMMessage(Id, args).ConfigureAwait(false);
            return new Message(this, GetUser(model.Id), model);
        }
        /// <inheritdoc />
        public async Task<Message> SendFile(string filePath, string text = null, bool isTTS = false)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
            {
                var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
                var model = await Discord.ApiClient.UploadDMFile(Id, file, args).ConfigureAwait(false);
                return new Message(this, GetUser(model.Id), model);
            }
        }
        /// <inheritdoc />
        public async Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false)
        {
            var args = new UploadFileParams { Filename = filename, Content = text, IsTTS = isTTS };
            var model = await Discord.ApiClient.UploadDMFile(Id, stream, args).ConfigureAwait(false);
            return new Message(this, GetUser(model.Id), model);
        }

        /// <inheritdoc />
        public async Task DeleteMessages(IEnumerable<IMessage> messages)
        {
            await Discord.ApiClient.DeleteDMMessages(Id, new DeleteMessagesParam { MessageIds = messages.Select(x => x.Id) }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task TriggerTyping()
        {
            await Discord.ApiClient.TriggerTypingIndicator(Id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task Close()
        {
            await Discord.ApiClient.DeleteChannel(Id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override string ToString() => '@' + Recipient.ToString();
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";

        IUser IDMChannel.Recipient => Recipient;
        IEnumerable<IMessage> IMessageChannel.CachedMessages => CachedMessages;

        Task<IEnumerable<IUser>> IChannel.GetUsers()
            => Task.FromResult<IEnumerable<IUser>>(Users);
        Task<IEnumerable<IUser>> IChannel.GetUsers(int limit, int offset)
            => Task.FromResult<IEnumerable<IUser>>(Users.Skip(offset).Take(limit));
        Task<IUser> IChannel.GetUser(ulong id)
            => Task.FromResult<IUser>(GetUser(id));
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
        Task IUpdateable.Update() 
            => Task.CompletedTask;
    }
}
