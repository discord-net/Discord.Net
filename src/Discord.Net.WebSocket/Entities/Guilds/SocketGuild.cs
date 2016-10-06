using Discord.API.Rest;
using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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

namespace Discord.WebSocket
{
    public class SocketGuild : SocketEntity<ulong>, IGuild
    {
        private readonly SemaphoreSlim _audioLock;
        private TaskCompletionSource<bool> _syncPromise, _downloaderPromise;
        private TaskCompletionSource<AudioClient> _audioConnectPromise;
        private ConcurrentHashSet<ulong> _channels;
        private ConcurrentDictionary<ulong, SocketGuildUser> _members;
        private ConcurrentDictionary<ulong, SocketRole> _roles;
        private ConcurrentDictionary<ulong, SocketVoiceState> _voiceStates;
        private ImmutableArray<Emoji> _emojis;
        private ImmutableArray<string> _features;
        internal bool _available;

        public string Name { get; private set; }
        public int AFKTimeout { get; private set; }
        public bool IsEmbeddable { get; private set; }
        public VerificationLevel VerificationLevel { get; private set; }
        public MfaLevel MfaLevel { get; private set; }
        public DefaultMessageNotifications DefaultMessageNotifications { get; private set; }
        public int MemberCount { get; set; }
        public int DownloadedMemberCount { get; private set; }
        public AudioClient AudioClient { get; private set; }

        public ulong? AFKChannelId { get; private set; }
        public ulong? EmbedChannelId { get; private set; }
        public ulong OwnerId { get; private set; }
        public string VoiceRegionId { get; private set; }
        public string IconId { get; private set; }
        public string SplashId { get; private set; }

        public ulong DefaultChannelId => Id;
        public string IconUrl => API.CDN.GetGuildIconUrl(Id, IconId);
        public string SplashUrl => API.CDN.GetGuildSplashUrl(Id, SplashId);
        public bool HasAllMembers => _downloaderPromise.Task.IsCompleted;
        public bool IsSynced => _syncPromise.Task.IsCompleted;
        public Task SyncPromise => _syncPromise.Task;
        public Task DownloaderPromise => _downloaderPromise.Task;

        public SocketRole EveryoneRole => GetRole(Id);
        public IReadOnlyCollection<SocketGuildChannel> Channels
        {
            get
            {
                var channels = _channels;
                var state = Discord.State;
                return channels.Select(x => state.GetChannel(x) as SocketGuildChannel).Where(x => x != null).ToReadOnlyCollection(channels);
            }
        }
        public IReadOnlyCollection<Emoji> Emojis => _emojis;
        public IReadOnlyCollection<string> Features => _features;
        public IReadOnlyCollection<SocketGuildUser> Users => _members.ToReadOnlyCollection();
        public IReadOnlyCollection<SocketRole> Roles => _roles.ToReadOnlyCollection();
        public IReadOnlyCollection<SocketVoiceState> VoiceStates => _voiceStates.ToReadOnlyCollection();

        internal SocketGuild(DiscordSocketClient client, ulong id)
            : base(client, id)
        {
            _audioLock = new SemaphoreSlim(1, 1);
            _emojis = ImmutableArray.Create<Emoji>();
            _features = ImmutableArray.Create<string>();
        }
        internal static SocketGuild Create(DiscordSocketClient discord, ClientState state, ExtendedModel model)
        {
            var entity = new SocketGuild(discord, model.Id);
            entity.Update(state, model);
            return entity;
        }
        internal void Update(ClientState state, ExtendedModel model)
        {
            _available = !(model.Unavailable ?? false);
            if (!_available)
            {
                if (_channels == null)
                    _channels = new ConcurrentHashSet<ulong>();
                if (_members == null)
                    _members = new ConcurrentDictionary<ulong, SocketGuildUser>();
                if (_roles == null)
                    _roles = new ConcurrentDictionary<ulong, SocketRole>();
                /*if (Emojis == null)
                    _emojis = ImmutableArray.Create<Emoji>();
                if (Features == null)
                    _features = ImmutableArray.Create<string>();*/
                return;
            }

            Update(state, model as Model);

            var channels = new ConcurrentHashSet<ulong>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.Channels.Length * 1.05));
            {
                for (int i = 0; i < model.Channels.Length; i++)
                {
                    var channel = SocketGuildChannel.Create(this, state, model.Channels[i]);
                    state.AddChannel(channel);
                    channels.TryAdd(channel.Id);
                }
            }
            _channels = channels;

            var members = new ConcurrentDictionary<ulong, SocketGuildUser>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.Members.Length * 1.05));
            {
                for (int i = 0; i < model.Members.Length; i++)
                {
                    var member = SocketGuildUser.Create(this, state, model.Members[i]);
                    members.TryAdd(member.Id, member);
                }
                DownloadedMemberCount = members.Count;

                for (int i = 0; i < model.Presences.Length; i++)
                {
                    SocketGuildUser member;
                    if (members.TryGetValue(model.Presences[i].User.Id, out member))
                        member.Update(state, model.Presences[i]);
                    else
                        Debug.Assert(false);
                }
            }
            _members = members;
            MemberCount = model.MemberCount;

            var voiceStates = new ConcurrentDictionary<ulong, SocketVoiceState>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.VoiceStates.Length * 1.05));
            {
                for (int i = 0; i < model.VoiceStates.Length; i++)
                {
                    SocketVoiceChannel channel = null;
                    if (model.VoiceStates[i].ChannelId.HasValue)
                        channel = state.GetChannel(model.VoiceStates[i].ChannelId.Value) as SocketVoiceChannel;
                    var voiceState = SocketVoiceState.Create(channel, model.VoiceStates[i]);
                    voiceStates.TryAdd(model.VoiceStates[i].UserId, voiceState);
                }
            }
            _voiceStates = voiceStates;

            _syncPromise = new TaskCompletionSource<bool>();
            _downloaderPromise = new TaskCompletionSource<bool>();
            if (Discord.ApiClient.AuthTokenType != TokenType.User)
            {
                var _ = _syncPromise.TrySetResultAsync(true);
                if (!model.Large)
                    _ = _downloaderPromise.TrySetResultAsync(true);
            }
        }
        internal void Update(ClientState state, Model model)
        {
            AFKChannelId = model.AFKChannelId;
            EmbedChannelId = model.EmbedChannelId;
            AFKTimeout = model.AFKTimeout;
            IsEmbeddable = model.EmbedEnabled;
            IconId = model.Icon;
            Name = model.Name;
            OwnerId = model.OwnerId;
            VoiceRegionId = model.Region;
            SplashId = model.Splash;
            VerificationLevel = model.VerificationLevel;
            MfaLevel = model.MfaLevel;
            DefaultMessageNotifications = model.DefaultMessageNotifications;

            if (model.Emojis != null)
            {
                var emojis = ImmutableArray.CreateBuilder<Emoji>(model.Emojis.Length);
                for (int i = 0; i < model.Emojis.Length; i++)
                    emojis.Add(Emoji.Create(model.Emojis[i]));
                _emojis = emojis.ToImmutable();
            }
            else
                _emojis = ImmutableArray.Create<Emoji>();

            if (model.Features != null)
                _features = model.Features.ToImmutableArray();
            else
                _features = ImmutableArray.Create<string>();

            var roles = new ConcurrentDictionary<ulong, SocketRole>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.Roles.Length * 1.05));
            if (model.Roles != null)
            {
                for (int i = 0; i < model.Roles.Length; i++)
                {
                    var role = SocketRole.Create(this, state, model.Roles[i]);
                    roles.TryAdd(role.Id, role);
                }
            }
            _roles = roles;
        }
        internal void Update(ClientState state, GuildSyncModel model)
        {
            var members = new ConcurrentDictionary<ulong, SocketGuildUser>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.Members.Length * 1.05));
            {
                for (int i = 0; i < model.Members.Length; i++)
                {
                    var member = SocketGuildUser.Create(this, state, model.Members[i]);
                    members.TryAdd(member.Id, member);
                }
                DownloadedMemberCount = members.Count;

                for (int i = 0; i < model.Presences.Length; i++)
                {
                    SocketGuildUser member;
                    if (members.TryGetValue(model.Presences[i].User.Id, out member))
                        member.Update(state, model.Presences[i]);
                    else
                        Debug.Assert(false);
                }
            }
            _members = members;

            var _ = _syncPromise.TrySetResultAsync(true);
            if (!model.Large)
                _ = _downloaderPromise.TrySetResultAsync(true);
        }

        internal void Update(ClientState state, EmojiUpdateModel model)
        {
            var emojis = ImmutableArray.CreateBuilder<Emoji>(model.Emojis.Length);
            for (int i = 0; i < model.Emojis.Length; i++)
                emojis.Add(Emoji.Create(model.Emojis[i]));
            _emojis = emojis.ToImmutable();
        }

        //General
        public Task DeleteAsync(RequestOptions options)
            => GuildHelper.DeleteAsync(this, Discord, options);

        public Task ModifyAsync(Action<ModifyGuildParams> func, RequestOptions options = null)
            => GuildHelper.ModifyAsync(this, Discord, func, options);
        public Task ModifyEmbedAsync(Action<ModifyGuildEmbedParams> func, RequestOptions options = null)
            => GuildHelper.ModifyEmbedAsync(this, Discord, func, options);
        public Task ModifyChannelsAsync(IEnumerable<ModifyGuildChannelsParams> args, RequestOptions options = null)
            => GuildHelper.ModifyChannelsAsync(this, Discord, args, options);
        public Task ModifyRolesAsync(IEnumerable<ModifyGuildRolesParams> args, RequestOptions options = null)
            => GuildHelper.ModifyRolesAsync(this, Discord, args, options);

        public Task LeaveAsync(RequestOptions options = null)
            => GuildHelper.LeaveAsync(this, Discord, options);

        //Bans
        public Task<IReadOnlyCollection<RestBan>> GetBansAsync(RequestOptions options = null)
            => GuildHelper.GetBansAsync(this, Discord, options);

        public Task AddBanAsync(IUser user, int pruneDays = 0, RequestOptions options = null)
            => GuildHelper.AddBanAsync(this, Discord, user.Id, pruneDays, options);
        public Task AddBanAsync(ulong userId, int pruneDays = 0, RequestOptions options = null)
            => GuildHelper.AddBanAsync(this, Discord, userId, pruneDays, options);

        public Task RemoveBanAsync(IUser user, RequestOptions options = null)
            => GuildHelper.RemoveBanAsync(this, Discord, user.Id, options);
        public Task RemoveBanAsync(ulong userId, RequestOptions options = null)
            => GuildHelper.RemoveBanAsync(this, Discord, userId, options);

        //Channels
        public SocketGuildChannel GetChannel(ulong id)
        {
            var channel = Discord.State.GetChannel(id) as SocketGuildChannel;
            if (channel?.Guild.Id == Id)
                return channel;
            return null;
        }
        public Task<RestTextChannel> CreateTextChannelAsync(string name, RequestOptions options = null)
            => GuildHelper.CreateTextChannelAsync(this, Discord, name, options);
        public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name, RequestOptions options = null)
            => GuildHelper.CreateVoiceChannelAsync(this, Discord, name, options);
        internal SocketGuildChannel AddChannel(ClientState state, ChannelModel model)
        {
            var channel = SocketGuildChannel.Create(this, state, model);
            _channels.TryAdd(model.Id);
            state.AddChannel(channel);
            return channel;
        }
        internal SocketGuildChannel RemoveChannel(ClientState state, ulong id)
        {
            if (_channels.TryRemove(id))
                return state.RemoveChannel(id) as SocketGuildChannel;
            return null;
        }

        //Integrations
        public Task<IReadOnlyCollection<RestGuildIntegration>> GetIntegrationsAsync(RequestOptions options = null)
            => GuildHelper.GetIntegrationsAsync(this, Discord, options);
        public Task<RestGuildIntegration> CreateIntegrationAsync(ulong id, string type, RequestOptions options = null)
            => GuildHelper.CreateIntegrationAsync(this, Discord, id, type, options);

        //Invites
        public Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => GuildHelper.GetInvitesAsync(this, Discord, options);

        //Roles
        public SocketRole GetRole(ulong id)
        {
            SocketRole value;
            if (_roles.TryGetValue(id, out value))
                return value;
            return null;
        }
        public Task<RestRole> CreateRoleAsync(string name, GuildPermissions? permissions = default(GuildPermissions?), Color? color = default(Color?), 
            bool isHoisted = false, RequestOptions options = null)
            => GuildHelper.CreateRoleAsync(this, Discord, name, permissions, color, isHoisted, options);
        internal SocketRole AddRole(RoleModel model)
        {
            var role = SocketRole.Create(this, Discord.State, model);
            _roles[model.Id] = role;
            return role;
        }
        internal SocketRole RemoveRole(ulong id)
        {
            SocketRole role;
            if (_roles.TryRemove(id, out role))
                return role;
            return null;
        }

        //Users
        public SocketGuildUser GetUser(ulong id)
        {
            SocketGuildUser member;
            if (_members.TryGetValue(id, out member))
                return member;
            return null;
        }
        public SocketGuildUser GetCurrentUser()
        {
            SocketGuildUser member;
            if (_members.TryGetValue(Discord.CurrentUser.Id, out member))
                return member;
            return null;
        }
        public Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null)
            => GuildHelper.PruneUsersAsync(this, Discord, days, simulate, options);

        internal SocketGuildUser AddOrUpdateUser(MemberModel model)
        {
            SocketGuildUser member;
            if (_members.TryGetValue(model.User.Id, out member))
                member.Update(Discord.State, model);
            else
            {
                member = SocketGuildUser.Create(this, Discord.State, model);
                _members[member.Id] = member;
                DownloadedMemberCount++;
            }
            return member;
        }
        internal SocketGuildUser AddOrUpdateUser(PresenceModel model)
        {
            SocketGuildUser member;
            if (_members.TryGetValue(model.User.Id, out member))
                member.Update(Discord.State, model);
            else
            {
                member = SocketGuildUser.Create(this, Discord.State, model);
                _members[member.Id] = member;
                DownloadedMemberCount++;
            }
            return member;
        }
        internal SocketGuildUser RemoveUser(ulong id)
        {
            SocketGuildUser member;
            if (_members.TryRemove(id, out member))
            {
                DownloadedMemberCount--;
                return member;
            }
            member.GlobalUser.RemoveRef(Discord);
            return null;
        }

        public async Task DownloadUsersAsync()
        {
            await Discord.DownloadUsersAsync(new[] { this });
        }
        internal void CompleteDownloadUsers()
        {
            _downloaderPromise.TrySetResultAsync(true);
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

        //Audio
        public async Task DisconnectAudioAsync(AudioClient client = null)
        {
            await _audioLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectAudioInternalAsync(client).ConfigureAwait(false);
            }
            finally
            {
                _audioLock.Release();
            }
        }
        private async Task DisconnectAudioInternalAsync(AudioClient client = null)
        {
            var oldClient = AudioClient;
            if (oldClient != null)
            {
                if (client == null || oldClient == client)
                {
                    _audioConnectPromise?.TrySetCanceledAsync(); //Cancel any previous audio connection
                    _audioConnectPromise = null;
                }
                if (oldClient == client)
                {
                    AudioClient = null;
                    await oldClient.DisconnectAsync().ConfigureAwait(false);
                }
            }
        }
        internal async Task FinishConnectAudio(int id, string url, string token)
        {
            var voiceState = GetVoiceState(Discord.CurrentUser.Id).Value;

            await _audioLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (AudioClient == null)
                {
                    var audioClient = new AudioClient(this, id);
                    audioClient.Disconnected += async ex =>
                    {
                        await _audioLock.WaitAsync().ConfigureAwait(false);
                        try
                        {
                            if (AudioClient == audioClient) //Only reconnect if we're still assigned as this guild's audio client
                            {
                                if (ex != null)
                                {
                                    //Reconnect if we still have channel info.
                                    //TODO: Is this threadsafe? Could channel data be deleted before we access it?
                                    var voiceState2 = GetVoiceState(Discord.CurrentUser.Id);
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
                        }
                        finally
                        {
                            _audioLock.Release();
                        }
                    };
                    AudioClient = audioClient;
                }
                await AudioClient.ConnectAsync(url, Discord.CurrentUser.Id, voiceState.VoiceSessionId, token).ConfigureAwait(false);
                await _audioConnectPromise.TrySetResultAsync(AudioClient).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await DisconnectAudioAsync();
            }
            catch (Exception e)
            {
                await _audioConnectPromise.SetExceptionAsync(e).ConfigureAwait(false);
                await DisconnectAudioAsync();
            }
            finally
            {
                _audioLock.Release();
            }
        }
        internal async Task FinishJoinAudioChannel()
        {
            await _audioLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (AudioClient != null)
                    await _audioConnectPromise.TrySetResultAsync(AudioClient).ConfigureAwait(false);
            }
            finally
            {
                _audioLock.Release();
            }
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
        internal SocketGuild Clone() => MemberwiseClone() as SocketGuild;

        //IGuild
        bool IGuild.Available => true;
        IAudioClient IGuild.AudioClient => null;
        IRole IGuild.EveryoneRole => EveryoneRole;
        IReadOnlyCollection<IRole> IGuild.Roles => Roles;

        async Task<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(RequestOptions options)
            => await GetBansAsync(options);

        Task<IReadOnlyCollection<IGuildChannel>> IGuild.GetChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGuildChannel>>(Channels);
        Task<IGuildChannel> IGuild.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildChannel>(GetChannel(id));
        async Task<ITextChannel> IGuild.CreateTextChannelAsync(string name, RequestOptions options)
            => await CreateTextChannelAsync(name, options);
        async Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name, RequestOptions options)
            => await CreateVoiceChannelAsync(name, options);

        async Task<IReadOnlyCollection<IGuildIntegration>> IGuild.GetIntegrationsAsync(RequestOptions options)
            => await GetIntegrationsAsync(options);
        async Task<IGuildIntegration> IGuild.CreateIntegrationAsync(ulong id, string type, RequestOptions options)
            => await CreateIntegrationAsync(id, type, options);

        async Task<IReadOnlyCollection<IInviteMetadata>> IGuild.GetInvitesAsync(RequestOptions options)
            => await GetInvitesAsync(options);

        IRole IGuild.GetRole(ulong id)
            => GetRole(id);
        async Task<IRole> IGuild.CreateRoleAsync(string name, GuildPermissions? permissions, Color? color, bool isHoisted, RequestOptions options)
            => await CreateRoleAsync(name, permissions, color, isHoisted, options);

        Task<IReadOnlyCollection<IGuildUser>> IGuild.GetUsersAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Users);
        Task<IGuildUser> IGuild.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetUser(id));
        Task<IGuildUser> IGuild.GetCurrentUserAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetCurrentUser());
        Task IGuild.DownloadUsersAsync() { throw new NotSupportedException(); }
    }
}
