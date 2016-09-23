using Discord.Rest;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;
using UserModel = Discord.API.User;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    internal class SocketGroupChannel : IGroupChannel, ISocketChannel, ISocketMessageChannel, ISocketPrivateChannel
    {
        internal override bool IsAttached => true;

        private readonly MessageManager _messages;
        private ConcurrentDictionary<ulong, VoiceState> _voiceStates;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public IReadOnlyCollection<ISocketUser> Users 
            => _users.Select(x => x.Value as ISocketUser).Concat(ImmutableArray.Create(Discord.CurrentUser)).ToReadOnlyCollection(() => _users.Count + 1);
        public new IReadOnlyCollection<ISocketUser> Recipients => _users.Select(x => x.Value as ISocketUser).ToReadOnlyCollection(_users);

        public SocketGroupChannel(DiscordSocketClient discord, Model model)
            : base(discord, model)
        {
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
            else
                _messages = new MessageManager(Discord, this);
            _voiceStates = new ConcurrentDictionary<ulong, VoiceState>(1, 5);
        }
        public override void Update(Model model)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            base.Update(model, source);
        }

        internal void UpdateUsers(UserModel[] models, DataStore dataStore)
        {
            var users = new ConcurrentDictionary<ulong, GroupUser>(1, models.Length);
            for (int i = 0; i < models.Length; i++)
            {
                var globalUser = Discord.GetOrAddUser(models[i], dataStore);
                users[models[i].Id] = new SocketGroupUser(this, globalUser);
            }
            _users = users;
        }
        internal override void UpdateUsers(UserModel[] models)
            => UpdateUsers(models, source, Discord.DataStore);

        public SocketGroupUser AddUser(UserModel model, DataStore dataStore)
        {
            GroupUser user;
            if (_users.TryGetValue(model.Id, out user))
                return user as SocketGroupUser;
            else
            {
                var globalUser = Discord.GetOrAddUser(model, dataStore);
                var privateUser = new SocketGroupUser(this, globalUser);
                _users[privateUser.Id] = privateUser;
                return privateUser;
            }
        }
        public ISocketUser GetUser(ulong id)
        {
            GroupUser user;
            if (_users.TryGetValue(id, out user))
                return user as SocketGroupUser;
            if (id == Discord.CurrentUser.Id)
                return Discord.CurrentUser;
            return null;
        }
        public SocketGroupUser RemoveUser(ulong id)
        {
            GroupUser user;
            if (_users.TryRemove(id, out user))
                return user as SocketGroupUser;
            return null;
        }

        public VoiceState AddOrUpdateVoiceState(VoiceStateModel model, DataStore dataStore, ConcurrentDictionary<ulong, VoiceState> voiceStates = null)
        {
            var voiceChannel = dataStore.GetChannel(model.ChannelId.Value) as SocketVoiceChannel;
            var voiceState = new VoiceState(voiceChannel, model);
            (voiceStates ?? _voiceStates)[model.UserId] = voiceState;
            return voiceState;
        }
        public VoiceState? GetVoiceState(ulong id)
        {
            VoiceState voiceState;
            if (_voiceStates.TryGetValue(id, out voiceState))
                return voiceState;
            return null;
        }
        public VoiceState? RemoveVoiceState(ulong id)
        {
            VoiceState voiceState;
            if (_voiceStates.TryRemove(id, out voiceState))
                return voiceState;
            return null;
        }

        public override async Task<IMessage> GetMessageAsync(ulong id)
        {
            return await _messages.DownloadAsync(id).ConfigureAwait(false);
        }
        public override async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit)
        {
            return await _messages.DownloadAsync(null, Direction.Before, limit).ConfigureAwait(false);
        }
        public override async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
        {
            return await _messages.DownloadAsync(fromMessageId, dir, limit).ConfigureAwait(false);
        }
        public ISocketMessage CreateMessage(ISocketUser author, MessageModel model)
        {
            return _messages.Create(author, model);
        }
        public ISocketMessage AddMessage(ISocketUser author, MessageModel model)
        {
            var msg = _messages.Create(author, model);
            _messages.Add(msg);
            return msg;
        }
        public ISocketMessage GetMessage(ulong id)
        {
            return _messages.Get(id);
        }
        public ISocketMessage RemoveMessage(ulong id)
        {
            return _messages.Remove(id);
        }

        public SocketDMChannel Clone() => MemberwiseClone() as SocketDMChannel;

        IMessage IMessageChannel.GetCachedMessage(ulong id) => GetMessage(id);
        ISocketUser ISocketMessageChannel.GetUser(ulong id, bool skipCheck) => GetUser(id);
        ISocketChannel ISocketChannel.Clone() => Clone();
    }
}
