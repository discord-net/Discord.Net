using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketDMChannel : SocketChannel, IDMChannel
    {
        private readonly MessageCache _messages;

        public SocketUser Recipient { get; private set; }

        public IReadOnlyCollection<SocketUser> Users => ImmutableArray.Create(Discord.CurrentUser, Recipient);

        internal SocketDMChannel(DiscordSocketClient discord, ulong id, ulong recipientId)
            : base(discord, id)
        {
            Recipient = new SocketUser(Discord, recipientId);
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
        }
        internal new static SocketDMChannel Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketDMChannel(discord, model.Id, model.Recipients.Value[0].Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Recipient.Update(model.Recipients.Value[0]);
        }

        public Task CloseAsync()
            => ChannelHelper.DeleteAsync(this, Discord);

        public SocketUser GetUser(ulong id)
        {
            if (id == Recipient.Id)
                return Recipient;
            else if (id == Discord.CurrentUser.Id)
                return Discord.CurrentUser as SocketSelfUser;
            else
                return null;
        }

        public SocketMessage GetMessage(ulong id)
            => _messages?.Get(id);
        public async Task<IMessage> GetMessageAsync(ulong id, bool allowDownload = true)
        {
            IMessage msg = _messages?.Get(id);
            if (msg == null && allowDownload)
                msg = await ChannelHelper.GetMessageAsync(this, Discord, id);
            return msg;
        }
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, limit: limit);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessageId, dir, limit);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessage.Id, dir, limit);
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync()
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord);

        public Task<RestUserMessage> SendMessageAsync(string text, bool isTTS)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS);
        public Task<RestUserMessage> SendFileAsync(string filePath, string text, bool isTTS)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS);
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS);

        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messages);

        public IDisposable EnterTypingState()
            => ChannelHelper.EnterTypingState(this, Discord);

        internal SocketMessage AddMessage(SocketUser author, MessageModel model)
        {
            var msg = SocketMessage.Create(Discord, author, model);
            _messages.Add(msg);
            return msg;
        }
        internal SocketMessage RemoveMessage(ulong id)
        {
            return _messages.Remove(id);
        }

        public SocketDMChannel Clone() => MemberwiseClone() as SocketDMChannel;

        public override string ToString() => $"@{Recipient}";
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";

        //IDMChannel            
        IUser IDMChannel.Recipient => Recipient;

        //IPrivateChannel
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create<IUser>(Recipient);

        //IMessageChannel
        IReadOnlyCollection<IMessage> IMessageChannel.CachedMessages => ImmutableArray.Create<IMessage>();
        IMessage IMessageChannel.GetCachedMessage(ulong id) => null;

        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id)
            => await GetMessageAsync(id);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit)
            => GetMessagesAsync(limit);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
            => GetMessagesAsync(fromMessageId, dir, limit);
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync()
            => await GetPinnedMessagesAsync().ConfigureAwait(false);
        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS)
            => await SendFileAsync(filePath, text, isTTS);
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS)
            => await SendFileAsync(stream, filename, text, isTTS);
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS)
            => await SendMessageAsync(text, isTTS);
        IDisposable IMessageChannel.EnterTypingState()
            => EnterTypingState();

        //IChannel
        IReadOnlyCollection<IUser> IChannel.CachedUsers => Users;

        IUser IChannel.GetCachedUser(ulong id)
            => GetUser(id);
        Task<IUser> IChannel.GetUserAsync(ulong id)
            => Task.FromResult<IUser>(GetUser(id));
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync()
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>().ToAsyncEnumerable();
    }
}
