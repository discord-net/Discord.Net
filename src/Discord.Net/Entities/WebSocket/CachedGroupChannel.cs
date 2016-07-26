using Discord.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;
using UserModel = Discord.API.User;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord
{
    internal class CachedGroupChannel : GroupChannel, IGroupChannel, ICachedChannel, ICachedMessageChannel, ICachedPrivateChannel
    {
        bool IEntity<ulong>.IsAttached => true;

        private readonly MessageManager _messages;
        private ConcurrentDictionary<ulong, VoiceState> _voiceStates;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public IReadOnlyCollection<ICachedUser> Members 
            => _users.Select(x => x.Value as ICachedUser).Concat(ImmutableArray.Create(Discord.CurrentUser)).ToReadOnlyCollection(() => _users.Count + 1);
        public new IReadOnlyCollection<ICachedUser> Recipients => _users.Select(x => x.Value as ICachedUser).ToReadOnlyCollection(_users);

        public CachedGroupChannel(DiscordSocketClient discord, Model model)
            : base(discord, model)
        {
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
            else
                _messages = new MessageManager(Discord, this);
            _voiceStates = new ConcurrentDictionary<ulong, VoiceState>(1, 5);
        }
        public override void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            base.Update(model, source);
        }

        internal void UpdateUsers(UserModel[] models, UpdateSource source, DataStore dataStore)
        {
            var users = new ConcurrentDictionary<ulong, GroupUser>(1, models.Length);
            for (int i = 0; i < models.Length; i++)
            {
                var globalUser = Discord.GetOrAddUser(models[i], dataStore);
                users[models[i].Id] = new CachedGroupUser(this, globalUser);
            }
            _users = users;
        }
        internal override void UpdateUsers(UserModel[] models, UpdateSource source)
            => UpdateUsers(models, source, Discord.DataStore);

        public CachedGroupUser AddUser(UserModel model, DataStore dataStore)
        {
            GroupUser user;
            if (_users.TryGetValue(model.Id, out user))
                return user as CachedGroupUser;
            else
            {
                var globalUser = Discord.GetOrAddUser(model, dataStore);
                var privateUser = new CachedGroupUser(this, globalUser);
                _users[privateUser.Id] = privateUser;
                return privateUser;
            }
        }
        public ICachedUser GetUser(ulong id)
        {
            GroupUser user;
            if (_users.TryGetValue(id, out user))
                return user as CachedGroupUser;
            if (id == Discord.CurrentUser.Id)
                return Discord.CurrentUser;
            return null;
        }
        public CachedGroupUser RemoveUser(ulong id)
        {
            GroupUser user;
            if (_users.TryRemove(id, out user))
                return user as CachedGroupUser;
            return null;
        }

        public VoiceState AddOrUpdateVoiceState(VoiceStateModel model, DataStore dataStore, ConcurrentDictionary<ulong, VoiceState> voiceStates = null)
        {
            var voiceChannel = dataStore.GetChannel(model.ChannelId.Value) as CachedVoiceChannel;
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
        public CachedMessage AddMessage(ICachedUser author, MessageModel model)
        {
            var msg = new CachedMessage(this, author, model);
            _messages.Add(msg);
            return msg;
        }
        public CachedMessage GetMessage(ulong id)
        {
            return _messages.Get(id);
        }
        public CachedMessage RemoveMessage(ulong id)
        {
            return _messages.Remove(id);
        }

        public CachedDMChannel Clone() => MemberwiseClone() as CachedDMChannel;

        IMessage IMessageChannel.GetCachedMessage(ulong id) => GetMessage(id);
        ICachedUser ICachedMessageChannel.GetUser(ulong id, bool skipCheck) => GetUser(id);
        ICachedChannel ICachedChannel.Clone() => Clone();
    }
}
