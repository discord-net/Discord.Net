using Discord.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;
using UserModel = Discord.API.User;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGroupChannel : SocketChannel, IGroupChannel
    {
        private readonly MessageCache _messages;

        private string _iconId;
        private ConcurrentDictionary<ulong, SocketGroupUser> _users;
        private ConcurrentDictionary<ulong, SocketVoiceState> _voiceStates;

        public string Name { get; private set; }

        public IReadOnlyCollection<SocketGroupUser> Users => _users.ToReadOnlyCollection();
        public IReadOnlyCollection<SocketGroupUser> Recipients
            => _users.Select(x => x.Value).Where(x => x.Id != Discord.CurrentUser.Id).ToReadOnlyCollection(() => _users.Count - 1);

        internal SocketGroupChannel(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
            _voiceStates = new ConcurrentDictionary<ulong, SocketVoiceState>(1, 5);
            _users = new ConcurrentDictionary<ulong, SocketGroupUser>(1, 5);
        }
        internal new static SocketGroupChannel Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketGroupChannel(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            if (model.Name.IsSpecified)
                Name = model.Name.Value;
            if (model.Icon.IsSpecified)
                _iconId = model.Icon.Value;

            if (model.Recipients.IsSpecified)
                UpdateUsers(model.Recipients.Value);
        }
        internal virtual void UpdateUsers(API.User[] models)
        {
            var users = new ConcurrentDictionary<ulong, SocketGroupUser>(1, (int)(models.Length * 1.05));
            for (int i = 0; i < models.Length; i++)
                users[models[i].Id] = SocketGroupUser.Create(Discord, models[i]);
            _users = users;
        }

        public async Task UpdateAsync()
            => Update(await ChannelHelper.GetAsync(this, Discord));
        public Task LeaveAsync()
            => ChannelHelper.DeleteAsync(this, Discord);

        public SocketGroupUser GetUser(ulong id)
        {
            SocketGroupUser user;
            if (_users.TryGetValue(id, out user))
                return user;
            return null;
        }

        public Task<RestMessage> GetMessageAsync(ulong id)
            => ChannelHelper.GetMessageAsync(this, Discord, id);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch)
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

        //IPrivateChannel
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => Recipients;

        //IMessageChannel
        IReadOnlyCollection<IMessage> IMessageChannel.CachedMessages => ImmutableArray.Create<IMessage>();

        IMessage IMessageChannel.GetCachedMessage(ulong id)
            => null;
        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id)
            => await GetMessageAsync(id);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit)
            => GetMessagesAsync(limit);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
            => GetMessagesAsync(fromMessageId, dir, limit);
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync()
            => await GetPinnedMessagesAsync();

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
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();
    }
}
