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

        public bool Available { get; private set; }
        public int MemberCount { get; private set; }
        public int DownloadedMemberCount { get; private set; }

        public bool HasAllMembers => _downloaderPromise.Task.IsCompleted;
        public Task DownloaderPromise => _downloaderPromise.Task;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public CachedGuildUser CurrentUser => GetUser(Discord.CurrentUser.Id);
        public IReadOnlyCollection<ICachedGuildChannel> Channels => _channels.Select(x => GetChannel(x)).ToReadOnlyCollection(_channels);
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
                    AddChannel(model.Channels[i], dataStore, channels);
            }
            _channels = channels;
            
            var members = new ConcurrentDictionary<ulong, CachedGuildUser>();
            if (model.Members != null)
            {
                DownloadedMemberCount = 0;
                for (int i = 0; i < model.Members.Length; i++)
                    AddUser(model.Members[i], dataStore, members);
                _downloaderPromise = new TaskCompletionSource<bool>();
                if (!model.Large)
                    _downloaderPromise.SetResult(true);
            }
            _members = members;

            var presences = new ConcurrentDictionary<ulong, Presence>();
            if (model.Presences != null)
            {
                for (int i = 0; i < model.Presences.Length; i++)
                {
                    var presence = model.Presences[i];
                    AddOrUpdatePresence(presence, presences);
                    //AddUser(presence, dataStore, members);
                }
            }
            _presences = presences;

            var voiceStates = new ConcurrentDictionary<ulong, VoiceState>();
            if (model.VoiceStates != null)
            {
                for (int i = 0; i < model.VoiceStates.Length; i++)
                    AddOrUpdateVoiceState(model.VoiceStates[i], dataStore, voiceStates);
            }
            _voiceStates = voiceStates;
        }

        public override Task<IGuildChannel> GetChannelAsync(ulong id) => Task.FromResult<IGuildChannel>(GetChannel(id));
        public override Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync() => Task.FromResult<IReadOnlyCollection<IGuildChannel>>(Channels);
        public void AddChannel(ChannelModel model, DataStore dataStore, ConcurrentHashSet<ulong> channels = null)
        {
            var channel = ToChannel(model);
            (channels ?? _channels).TryAdd(model.Id);
            dataStore.AddChannel(channel);
        }
        public ICachedGuildChannel GetChannel(ulong id)
        {
            return Discord.DataStore.GetChannel(id) as ICachedGuildChannel;
        }
        public ICachedGuildChannel RemoveChannel(ulong id)
        {
            _channels.TryRemove(id);
            return Discord.DataStore.RemoveChannel(id) as ICachedGuildChannel;
        }

        public Presence AddOrUpdatePresence(PresenceModel model, ConcurrentDictionary<ulong, Presence> presences = null)
        {
            var game = model.Game != null ? new Game(model.Game) : (Game?)null;
            var presence = new Presence(model.Status, game);
            (presences ?? _presences)[model.User.Id] = presence;
            return presence;
        }
        public Presence? GetPresence(ulong id)
        {
            Presence presence;
            if (_presences.TryGetValue(id, out presence))
                return presence;
            return null;
        }
        public Presence? RemovePresence(ulong id)
        {
            Presence presence;
            if (_presences.TryRemove(id, out presence))
                return presence;
            return null;
        }

        public Role AddRole(RoleModel model, ConcurrentDictionary<ulong, Role> roles = null)
        {
            var role = new Role(this, model);
            (roles ?? _roles)[model.Id] = role;
            return role;
        }
        public Role RemoveRole(ulong id)
        {
            Role role;
            if (_roles.TryRemove(id, out role))
                return role;
            return null;
        }

        public VoiceState AddOrUpdateVoiceState(VoiceStateModel model, DataStore dataStore, ConcurrentDictionary<ulong, VoiceState> voiceStates = null)
        {
            var voiceChannel = dataStore.GetChannel(model.ChannelId.Value) as CachedVoiceChannel;
            var voiceState = new VoiceState(voiceChannel, model.SessionId, model.SelfMute, model.SelfDeaf, model.Suppress);
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

        public override Task<IGuildUser> GetUserAsync(ulong id) => Task.FromResult<IGuildUser>(GetUser(id));
        public override Task<IGuildUser> GetCurrentUserAsync() 
            => Task.FromResult<IGuildUser>(CurrentUser);
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync() 
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members);
        //TODO: Is there a better way of exposing pagination?
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(int limit, int offset) 
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members.OrderBy(x => x.Id).Skip(offset).Take(limit).ToImmutableArray());
        public CachedGuildUser AddUser(MemberModel model, DataStore dataStore, ConcurrentDictionary<ulong, CachedGuildUser> members = null)
        {
            var user = Discord.GetOrAddUser(model.User, dataStore);
            members = members ?? _members;

            CachedGuildUser member;
            if (members.TryGetValue(model.User.Id, out member))
                member.Update(model, UpdateSource.WebSocket);
            else
            {
                member = new CachedGuildUser(this, user, model);
                members[user.Id] = member;
                user.AddRef();
                DownloadedMemberCount++;
            }
            return member;
        }
        public CachedGuildUser AddUser(PresenceModel model, DataStore dataStore, ConcurrentDictionary<ulong, CachedGuildUser> members = null)
        {
            members = members ?? _members;

            CachedGuildUser member;
            if (members.TryGetValue(model.User.Id, out member))
                member.Update(model, UpdateSource.WebSocket);
            else
            {
                var user = Discord.GetOrAddUser(model.User, dataStore);
                member = new CachedGuildUser(this, user, model);
                members[user.Id] = member;
                user.AddRef();
                DownloadedMemberCount++;
            }            
            return member;
        }
        public CachedGuildUser GetUser(ulong id)
        {
            CachedGuildUser member;
            if (_members.TryGetValue(id, out member))
                return member;
            return null;
        }
        public CachedGuildUser RemoveUser(ulong id)
        {
            CachedGuildUser member;
            if (_members.TryRemove(id, out member))
                return member;
            return null;
        }
        public async Task DownloadMembersAsync()
        {
            if (!HasAllMembers)
                await Discord.ApiClient.SendRequestMembersAsync(new ulong[] { Id }).ConfigureAwait(false);
            await _downloaderPromise.Task.ConfigureAwait(false);
        }
        public void CompleteDownloadMembers()
        {
            _downloaderPromise.TrySetResult(true);
        }

        public CachedGuild Clone() => MemberwiseClone() as CachedGuild;

        new internal ICachedGuildChannel ToChannel(ChannelModel model)
        {
            switch (model.Type.Value)
            {
                case ChannelType.Text:
                    return new CachedTextChannel(this, model);
                case ChannelType.Voice:
                    return new CachedVoiceChannel(this, model);
                default:
                    throw new InvalidOperationException($"Unknown channel type: {model.Type.Value}");
            }
        }
    }
}
