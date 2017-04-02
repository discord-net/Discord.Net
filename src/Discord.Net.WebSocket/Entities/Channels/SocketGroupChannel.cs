using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;
using UserModel = Discord.API.User;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGroupChannel : SocketChannel, IGroupChannel, ISocketPrivateChannel, ISocketMessageChannel, ISocketAudioChannel
    {
        private readonly MessageCache _messages;

        private string _iconId;
        private ConcurrentDictionary<ulong, SocketGroupUser> _users;
        private ConcurrentDictionary<ulong, SocketVoiceState> _voiceStates;

        public string Name { get; private set; }

        public IReadOnlyCollection<SocketMessage> CachedMessages => _messages?.Messages ?? ImmutableArray.Create<SocketMessage>();
        public new IReadOnlyCollection<SocketGroupUser> Users => _users.ToReadOnlyCollection();
        public IReadOnlyCollection<SocketGroupUser> Recipients
            => _users.Select(x => x.Value).Where(x => x.Id != Discord.CurrentUser.Id).ToReadOnlyCollection(() => _users.Count - 1);

        internal SocketGroupChannel(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
            _voiceStates = new ConcurrentDictionary<ulong, SocketVoiceState>(ConcurrentHashSet.DefaultConcurrencyLevel, 5);
            _users = new ConcurrentDictionary<ulong, SocketGroupUser>(ConcurrentHashSet.DefaultConcurrencyLevel, 5);
        }
        internal static SocketGroupChannel Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            var entity = new SocketGroupChannel(discord, model.Id);
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            if (model.Name.IsSpecified)
                Name = model.Name.Value;
            if (model.Icon.IsSpecified)
                _iconId = model.Icon.Value;

            if (model.Recipients.IsSpecified)
                UpdateUsers(state, model.Recipients.Value);
        }
        private void UpdateUsers(ClientState state, UserModel[] models)
        {
            var users = new ConcurrentDictionary<ulong, SocketGroupUser>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(models.Length * 1.05));
            for (int i = 0; i < models.Length; i++)
                users[models[i].Id] = SocketGroupUser.Create(this, state, models[i]);
            _users = users;
        }
        
        public Task LeaveAsync(RequestOptions options = null)
            => ChannelHelper.DeleteAsync(this, Discord, options);

        public Task<IAudioClient> ConnectAsync()
        {
            throw new NotSupportedException("Voice is not yet supported for group channels.");
        }

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
        public new SocketGroupUser GetUser(ulong id)
        {
            SocketGroupUser user;
            if (_users.TryGetValue(id, out user))
                return user;
            return null;
        }
        internal SocketGroupUser GetOrAddUser(UserModel model)
        {
            SocketGroupUser user;
            if (_users.TryGetValue(model.Id, out user))
                return user as SocketGroupUser;
            else
            {
                var privateUser = SocketGroupUser.Create(this, Discord.State, model);
                _users[privateUser.Id] = privateUser;
                return privateUser;
            }
        }
        internal SocketGroupUser RemoveUser(ulong id)
        {
            SocketGroupUser user;
            if (_users.TryRemove(id, out user))
            {
                user.GlobalUser.RemoveRef(Discord);
                return user as SocketGroupUser;
            }
            return null;
        }

        //Voice States
        internal SocketVoiceState AddOrUpdateVoiceState(ClientState state, VoiceStateModel model)
        {
            var voiceChannel = state.GetChannel(model.ChannelId.Value) as SocketVoiceChannel;
            var voiceState = SocketVoiceState.Create(voiceChannel, model);
            _voiceStates[model.UserId] = voiceState;
            return voiceState;
        }
        internal SocketVoiceState? GetVoiceState(ulong id)
        {
            SocketVoiceState voiceState;
            if (_voiceStates.TryGetValue(id, out voiceState))
                return voiceState;
            return null;
        }
        internal SocketVoiceState? RemoveVoiceState(ulong id)
        {
            SocketVoiceState voiceState;
            if (_voiceStates.TryRemove(id, out voiceState))
                return voiceState;
            return null;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}, Group)";
        internal new SocketGroupChannel Clone() => MemberwiseClone() as SocketGroupChannel;

        //SocketChannel
        internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;
        internal override SocketUser GetUserInternal(ulong id) => GetUser(id);

        //ISocketPrivateChannel
        IReadOnlyCollection<SocketUser> ISocketPrivateChannel.Recipients => Recipients;

        //IPrivateChannel
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => Recipients;

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

        //IAudioChannel
        Task<IAudioClient> IAudioChannel.ConnectAsync(Action<IAudioClient> configAction) { throw new NotSupportedException(); }

        //IChannel        
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id));
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();
    }
}
