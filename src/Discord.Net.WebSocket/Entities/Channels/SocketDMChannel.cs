using Discord.Rest;
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
    public class SocketDMChannel : SocketChannel, IDMChannel, ISocketPrivateChannel, ISocketMessageChannel
    {
        private readonly MessageCache _messages;

        public SocketUser Recipient { get; private set; }

        public IReadOnlyCollection<SocketMessage> CachedMessages => _messages?.Messages ?? ImmutableArray.Create<SocketMessage>();
        public new IReadOnlyCollection<SocketUser> Users => ImmutableArray.Create(Discord.CurrentUser, Recipient);

        internal SocketDMChannel(DiscordSocketClient discord, ulong id, SocketGlobalUser recipient)
            : base(discord, id)
        {
            Recipient = recipient;
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
        }
        internal static SocketDMChannel Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            var entity = new SocketDMChannel(discord, model.Id, discord.GetOrCreateUser(state, model.Recipients.Value[0]));
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            Recipient.Update(state, model.Recipients.Value[0]);
        }

        public Task CloseAsync()
            => ChannelHelper.DeleteAsync(this, Discord);

        //Messages
        public SocketMessage GetCachedMessage(ulong id)
            => _messages?.Get(id);
        public async Task<IMessage> GetMessageAsync(ulong id)
        {
            IMessage msg = _messages?.Get(id);
            if (msg == null)
                msg = await ChannelHelper.GetMessageAsync(this, Discord, id);
            return msg;
        }
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, null, Direction.Before, limit, CacheMode.AllowDownload);
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessageId, dir, limit, CacheMode.AllowDownload);
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessage.Id, dir, limit, CacheMode.AllowDownload);
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, null, Direction.Before, limit);
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, fromMessageId, dir, limit);
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, fromMessage.Id, dir, limit);
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

        internal void AddMessage(SocketMessage msg)
            => _messages.Add(msg);
        internal SocketMessage RemoveMessage(ulong id)
            => _messages.Remove(id);

        //Users
        public new SocketUser GetUser(ulong id)
        {
            if (id == Recipient.Id)
                return Recipient;
            else if (id == Discord.CurrentUser.Id)
                return Discord.CurrentUser as SocketSelfUser;
            else
                return null;
        }

        public override string ToString() => $"@{Recipient}";
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
        internal new SocketDMChannel Clone() => MemberwiseClone() as SocketDMChannel;

        //SocketChannel
        internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;
        internal override SocketUser GetUserInternal(ulong id) => GetUser(id);

        //IDMChannel            
        IUser IDMChannel.Recipient => Recipient;

        //ISocketPrivateChannel
        IReadOnlyCollection<SocketUser> ISocketPrivateChannel.Recipients => ImmutableArray.Create(Recipient);

        //IPrivateChannel
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create<IUser>(Recipient);

        //IMessageChannel
        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id, CacheMode mode)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetMessageAsync(id);
            else
                return GetCachedMessage(id);
        }
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, null, Direction.Before, limit, mode);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit, CacheMode mode)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessageId, dir, limit, mode);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage fromMessage, Direction dir, int limit, CacheMode mode)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessage.Id, dir, limit, mode);
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
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode)
            => Task.FromResult<IUser>(GetUser(id));
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();
    }
}
