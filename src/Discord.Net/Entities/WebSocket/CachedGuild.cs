using Discord.Audio;
using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChannelModel = Discord.API.Channel;
using EmojiUpdateModel = Discord.API.Gateway.GuildEmojiUpdateEvent;
using ExtendedModel = Discord.API.Gateway.ExtendedGuild;
using GuildSyncModel = Discord.API.Gateway.GuildSyncEvent;
using MemberModel = Discord.API.GuildMember;
using Model = Discord.API.Guild;
using PresenceModel = Discord.API.Presence;
using RoleModel = Discord.API.Role;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord
{
    internal class CachedGuild : Guild, ICachedEntity<ulong>, IGuild, IUserGuild
    {
        private readonly SemaphoreSlim _audioLock;
        private TaskCompletionSource<bool> _syncPromise, _downloaderPromise;
        private ConcurrentHashSet<ulong> _channels;
        private ConcurrentDictionary<ulong, CachedGuildUser> _members;
        private ConcurrentDictionary<ulong, VoiceState> _voiceStates;

        public bool Available { get; private set; }
        public int MemberCount { get; private set; }
        public int DownloadedMemberCount { get; private set; }
        public AudioClient AudioClient { get; private set; }

        public bool HasAllMembers => _downloaderPromise.Task.IsCompleted;
        public Task SyncPromise => _syncPromise.Task;
        public Task DownloaderPromise => _downloaderPromise.Task;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public CachedGuildUser CurrentUser => GetUser(Discord.CurrentUser.Id);
        public IReadOnlyCollection<ICachedGuildChannel> Channels
        {
            get
            {
                var channels = _channels;
                var store = Discord.DataStore;
                return channels.Select(x => store.GetChannel(x) as ICachedGuildChannel).Where(x => x != null).ToReadOnlyCollection(channels);
            }
        }
        public IReadOnlyCollection<CachedGuildUser> Members => _members.ToReadOnlyCollection();
        public IEnumerable<KeyValuePair<ulong, VoiceState>> VoiceStates => _voiceStates;
        
        public CachedGuild(DiscordSocketClient discord, ExtendedModel model, DataStore dataStore) : base(discord, model)
        {
            _audioLock = new SemaphoreSlim(1, 1);
            _syncPromise = new TaskCompletionSource<bool>();
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
                if (_roles == null)
                    _roles = new ConcurrentDictionary<ulong, Role>();
                if (Emojis == null)
                    Emojis = ImmutableArray.Create<Emoji>();
                if (Features == null)
                    Features = ImmutableArray.Create<string>();
                return;
            }

            base.Update(model as Model, source);

            MemberCount = model.MemberCount;
            
            var channels = new ConcurrentHashSet<ulong>(1, (int)(model.Channels.Length * 1.05));
            {
                for (int i = 0; i < model.Channels.Length; i++)
                    AddChannel(model.Channels[i], dataStore, channels);
            }
            _channels = channels;
            
            var members = new ConcurrentDictionary<ulong, CachedGuildUser>(1, (int)(model.Presences.Length * 1.05));
            {
                DownloadedMemberCount = 0;
                for (int i = 0; i < model.Members.Length; i++)
                    AddUser(model.Members[i], dataStore, members);
                if (Discord.ApiClient.AuthTokenType != TokenType.User)
                {
                    _syncPromise.TrySetResult(true);
                    if (!model.Large)
                        _downloaderPromise.TrySetResult(true);
                }

                for (int i = 0; i < model.Presences.Length; i++)
                    AddOrUpdateUser(model.Presences[i], dataStore, members);
            }
            _members = members;
            
            var voiceStates = new ConcurrentDictionary<ulong, VoiceState>(1, (int)(model.VoiceStates.Length * 1.05));
            {
                for (int i = 0; i < model.VoiceStates.Length; i++)
                    AddOrUpdateVoiceState(model.VoiceStates[i], dataStore, voiceStates);
            }
            _voiceStates = voiceStates;
        }
        public void Update(GuildSyncModel model, UpdateSource source, DataStore dataStore)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            var members = new ConcurrentDictionary<ulong, CachedGuildUser>(1, (int)(model.Presences.Length * 1.05));
            {
                DownloadedMemberCount = 0;
                for (int i = 0; i < model.Members.Length; i++)
                    AddUser(model.Members[i], dataStore, members);
                _syncPromise.TrySetResult(true);
                if (!model.Large)
                    _downloaderPromise.TrySetResult(true);

                for (int i = 0; i < model.Presences.Length; i++)
                    AddOrUpdateUser(model.Presences[i], dataStore, members);
            }
            _members = members;
        }

        public void Update(EmojiUpdateModel model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;
            
            var emojis = ImmutableArray.CreateBuilder<Emoji>(model.Emojis.Length);
            for (int i = 0; i < model.Emojis.Length; i++)
                emojis.Add(new Emoji(model.Emojis[i]));
            Emojis = emojis.ToImmutableArray();
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
        public CachedGuildUser AddOrUpdateUser(PresenceModel model, DataStore dataStore, ConcurrentDictionary<ulong, CachedGuildUser> members = null)
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
        public override async Task DownloadUsersAsync()
        {
            await Discord.DownloadUsersAsync(new [] { this });
        }
        public void CompleteDownloadMembers()
        {
            _downloaderPromise.TrySetResult(true);
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

        public async Task ConnectAudio(int id, string url, string token)
        {
            AudioClient audioClient;
            await _audioLock.WaitAsync().ConfigureAwait(false);
            var voiceState = GetVoiceState(CurrentUser.Id).Value;
            try
            {
                audioClient = AudioClient;
                if (audioClient == null)
                {
                    audioClient = new AudioClient(this, id);
                    audioClient.Disconnected += async ex =>
                    {
                        await _audioLock.WaitAsync().ConfigureAwait(false);
                        try
                        {
                            if (ex != null)
                            {
                                //Reconnect if we still have channel info.
                                //TODO: Is this threadsafe? Could channel data be deleted before we access it?
                                var voiceState2 = GetVoiceState(CurrentUser.Id);
                                if (voiceState2.HasValue)
                                {
                                    var voiceChannelId = voiceState2.Value.VoiceChannel?.Id;
                                    if (voiceChannelId != null)
                                        await Discord.ApiClient.SendVoiceStateUpdateAsync(Id, voiceChannelId, voiceState2.Value.IsSelfDeafened, voiceState2.Value.IsSelfMuted);
                                }
                            }
                            else
                            {
                                try { AudioClient.Dispose(); } catch { }
                                AudioClient = null;
                            }
                        }
                        finally
                        {
                            _audioLock.Release();
                        }
                    };
                    AudioClient = audioClient;
                }
            }
            finally
            {
                _audioLock.Release();
            }
            await audioClient.ConnectAsync(url, CurrentUser.Id, voiceState.VoiceSessionId, token).ConfigureAwait(false);
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

        bool IUserGuild.IsOwner => OwnerId == Discord.CurrentUser.Id;
        GuildPermissions IUserGuild.Permissions => CurrentUser.GuildPermissions;
        IAudioClient IGuild.AudioClient => AudioClient;
    }
}
