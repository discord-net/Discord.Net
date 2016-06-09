using Discord.Data;
using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ChannelModel = Discord.API.Channel;
using ExtendedModel = Discord.API.Gateway.ExtendedGuild;
using MemberModel = Discord.API.GuildMember;
using Model = Discord.API.Guild;
using PresenceModel = Discord.API.Presence;
using RoleModel = Discord.API.Role;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord
{
    internal class CachedGuild : Guild, ICachedEntity<ulong>
    {
        private TaskCompletionSource<bool> _downloaderPromise;
        private ConcurrentHashSet<ulong> _channels;
        private ConcurrentDictionary<ulong, CachedGuildUser> _members;
        private ConcurrentDictionary<ulong, Presence> _presences;
        private ConcurrentDictionary<ulong, VoiceState> _voiceStates;

        public bool Available { get; private set; } //TODO: Add to IGuild
        public int MemberCount { get; private set; }
        public int DownloadedMemberCount { get; private set; }

        public bool HasAllMembers => _downloaderPromise.Task.IsCompleted;
        public Task DownloaderPromise => _downloaderPromise.Task;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public CachedGuildUser CurrentUser => GetCachedUser(Discord.CurrentUser.Id);
        public IReadOnlyCollection<ICachedGuildChannel> Channels => _channels.Select(x => GetCachedChannel(x)).ToReadOnlyCollection(_channels);
        public IReadOnlyCollection<CachedGuildUser> Members => _members.ToReadOnlyCollection();

        public CachedGuild(DiscordSocketClient discord, ExtendedModel model, DataStore dataStore) : base(discord, model)
        {
            _downloaderPromise = new TaskCompletionSource<bool>();
            Update(model, UpdateSource.Creation, dataStore);
        }

        public void Update(ExtendedModel model, UpdateSource source, DataStore dataStore)
        {
            if (source == UpdateSource.Rest && IsAttached) return;
            
            Available = !(model.Unavailable ?? false);
            if (!Available)
            {
                if (_channels == null)
                    _channels = new ConcurrentHashSet<ulong>();
                if (_members == null)
                    _members = new ConcurrentDictionary<ulong, CachedGuildUser>();
                if (_presences == null)
                    _presences = new ConcurrentDictionary<ulong, Presence>();
                if (_roles == null)
                    _roles = new ConcurrentDictionary<ulong, Role>();
                if (_voiceStates == null)
                    _voiceStates = new ConcurrentDictionary<ulong, VoiceState>();
                if (Emojis == null)
                    Emojis = ImmutableArray.Create<Emoji>();
                if (Features == null)
                    Features = ImmutableArray.Create<string>();
                return;
            }

            base.Update(model as Model, source);

            MemberCount = model.MemberCount;

            var channels = new ConcurrentHashSet<ulong>();
            if (model.Channels != null)
            {
                for (int i = 0; i < model.Channels.Length; i++)
                    AddCachedChannel(model.Channels[i], channels);
            }
            _channels = channels;

            var presences = new ConcurrentDictionary<ulong, Presence>();
            if (model.Presences != null)
            {
                for (int i = 0; i < model.Presences.Length; i++)
                    AddOrUpdateCachedPresence(model.Presences[i], presences);
            }
            _presences = presences;

            var members = new ConcurrentDictionary<ulong, CachedGuildUser>();
            if (model.Members != null)
            {
                for (int i = 0; i < model.Members.Length; i++)
                    AddCachedUser(model.Members[i], members, dataStore);
                _downloaderPromise = new TaskCompletionSource<bool>();
                DownloadedMemberCount = model.Members.Length;
                if (!model.Large)
                    _downloaderPromise.SetResult(true);
            }
            _members = members;

            var voiceStates = new ConcurrentDictionary<ulong, VoiceState>();
            if (model.VoiceStates != null)
            {
                for (int i = 0; i < model.VoiceStates.Length; i++)
                    AddOrUpdateCachedVoiceState(model.VoiceStates[i], _voiceStates);
            }
            _voiceStates = voiceStates;
        }

        public override Task<IGuildChannel> GetChannel(ulong id) => Task.FromResult<IGuildChannel>(GetCachedChannel(id));
        public override Task<IReadOnlyCollection<IGuildChannel>> GetChannels() => Task.FromResult<IReadOnlyCollection<IGuildChannel>>(Channels);
        public ICachedGuildChannel AddCachedChannel(ChannelModel model, ConcurrentHashSet<ulong> channels = null)
        {
            var channel = ToChannel(model);
            (channels ?? _channels).TryAdd(model.Id);
            return channel;
        }
        public ICachedGuildChannel GetCachedChannel(ulong id)
        {
            return Discord.DataStore.GetChannel(id) as ICachedGuildChannel;
        }
        public void RemoveCachedChannel(ulong id, ConcurrentHashSet<ulong> channels = null)
        {
            (channels ?? _channels).TryRemove(id);
        }

        public Presence AddOrUpdateCachedPresence(PresenceModel model, ConcurrentDictionary<ulong, Presence> presences = null)
        {
            var game = model.Game != null ? new Game(model.Game) : (Game?)null;
            var presence = new Presence(model.Status, game);
            (presences ?? _presences)[model.User.Id] = presence;
            return presence;
        }
        public Presence? GetCachedPresence(ulong id)
        {
            Presence presence;
            if (_presences.TryGetValue(id, out presence))
                return presence;
            return null;
        }
        public Presence? RemoveCachedPresence(ulong id)
        {
            Presence presence;
            if (_presences.TryRemove(id, out presence))
                return presence;
            return null;
        }

        public Role AddCachedRole(RoleModel model, ConcurrentDictionary<ulong, Role> roles = null)
        {
            var role = new Role(this, model);
            (roles ?? _roles)[model.Id] = role;
            return role;
        }
        public Role RemoveCachedRole(ulong id)
        {
            Role role;
            if (_roles.TryRemove(id, out role))
                return role;
            return null;
        }

        public VoiceState AddOrUpdateCachedVoiceState(VoiceStateModel model, ConcurrentDictionary<ulong, VoiceState> voiceStates = null)
        {
            var voiceChannel = GetCachedChannel(model.ChannelId.Value) as CachedVoiceChannel;
            var voiceState = new VoiceState(voiceChannel, model.SessionId, model.SelfMute, model.SelfDeaf, model.Suppress);
            (voiceStates ?? _voiceStates)[model.UserId] = voiceState;
            return voiceState;
        }
        public VoiceState? GetCachedVoiceState(ulong id)
        {
            VoiceState voiceState;
            if (_voiceStates.TryGetValue(id, out voiceState))
                return voiceState;
            return null;
        }
        public VoiceState? RemoveCachedVoiceState(ulong id)
        {
            VoiceState voiceState;
            if (_voiceStates.TryRemove(id, out voiceState))
                return voiceState;
            return null;
        }

        public override Task<IGuildUser> GetUser(ulong id) => Task.FromResult<IGuildUser>(GetCachedUser(id));
        public override Task<IGuildUser> GetCurrentUser() 
            => Task.FromResult<IGuildUser>(CurrentUser);
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers() 
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members);
        //TODO: Is there a better way of exposing pagination?
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers(int limit, int offset) 
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members.OrderBy(x => x.Id).Skip(offset).Take(limit).ToImmutableArray());
        public CachedGuildUser AddCachedUser(MemberModel model, ConcurrentDictionary<ulong, CachedGuildUser> members = null, DataStore dataStore = null)
        {
            var user = Discord.GetOrAddCachedUser(model.User);
            var member = new CachedGuildUser(this, user, model);
            (members ?? _members)[user.Id] = member;
            user.AddRef();
            DownloadedMemberCount++;
            return member;
        }
        public CachedGuildUser GetCachedUser(ulong id)
        {
            CachedGuildUser member;
            if (_members.TryGetValue(id, out member))
                return member;
            return null;
        }
        public CachedGuildUser RemoveCachedUser(ulong id)
        {
            CachedGuildUser member;
            if (_members.TryRemove(id, out member))
                return member;
            return null;
        }
        public async Task DownloadMembers()
        {
            if (!HasAllMembers)
                await Discord.ApiClient.SendRequestMembers(new ulong[] { Id }).ConfigureAwait(false);
            await _downloaderPromise.Task.ConfigureAwait(false);
        }
        public void CompleteDownloadMembers()
        {
            _downloaderPromise.TrySetResult(true);
        }

        public CachedGuild Clone() => MemberwiseClone() as CachedGuild;

        new internal ICachedGuildChannel ToChannel(ChannelModel model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return new CachedTextChannel(this, model);
                case ChannelType.Voice:
                    return new CachedVoiceChannel(this, model);
                default:
                    throw new InvalidOperationException($"Unknown channel type: {model.Type}");
            }
        }
    }
}
