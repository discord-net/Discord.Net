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

        public Task CloseAsync(RequestOptions options = null)
            => ChannelHelper.DeleteAsync(this, Discord, options);

        //Messages
        public SocketMessage GetCachedMessage(ulong id)
            => _messages?.Get(id);
        public async Task<IMessage> GetMessageAsync(ulong id, RequestOptions options = null)
        {
            IMessage msg = _messages?.Get(id);
            if (msg == null)
                msg = await ChannelHelper.GetMessageAsync(this, Discord, id, options).ConfigureAwait(false);
            return msg;
        }
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, null, Direction.Before, limit, CacheMode.AllowDownload, options);
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessageId, dir, limit, CacheMode.AllowDownload, options);
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessage.Id, dir, limit, CacheMode.AllowDownload, options);
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, null, Direction.Before, limit);
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, fromMessageId, dir, limit);
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, fromMessage.Id, dir, limit);
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord, options);

        public Task<RestUserMessage> SendMessageAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS, embed, options);
#if NETSTANDARD1_3
        public Task<RestUserMessage> SendFileAsync(string filePath, string text, bool isTTS = false, RequestOptions options = null)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS, options);
#endif
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS = false, RequestOptions options = null)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS, options);

        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messages, options);

        public Task TriggerTypingAsync(RequestOptions options = null)
            => ChannelHelper.TriggerTypingAsync(this, Discord, options);
        public IDisposable EnterTypingState(RequestOptions options = null)
            => ChannelHelper.EnterTypingState(this, Discord, options);

        internal void AddMessage(SocketMessage msg)
            => _messages?.Add(msg);
        internal SocketMessage RemoveMessage(ulong id)
            => _messages?.Remove(id);

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
        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetMessageAsync(id, options).ConfigureAwait(false);
            else
                return GetCachedMessage(id);
        }
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, null, Direction.Before, limit, mode, options);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessageId, dir, limit, mode, options);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage fromMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessage.Id, dir, limit, mode, options);
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync(RequestOptions options)
            => await GetPinnedMessagesAsync(options).ConfigureAwait(false);
#if NETSTANDARD1_3
        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS, RequestOptions options)
            => await SendFileAsync(filePath, text, isTTS, options).ConfigureAwait(false);
#endif
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS, RequestOptions options)
            => await SendFileAsync(stream, filename, text, isTTS, options).ConfigureAwait(false);
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS, Embed embed, RequestOptions options)
            => await SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
        IDisposable IMessageChannel.EnterTypingState(RequestOptions options)
            => EnterTypingState(options);

        //IChannel        
        string IChannel.Name => $"@{Recipient}";

        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id));
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();
    }
}
