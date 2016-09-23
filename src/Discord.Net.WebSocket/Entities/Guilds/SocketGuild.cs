using Discord.Audio;
using Discord.Rest;
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

namespace Discord.WebSocket
{
    internal class SocketGuild : Guild, IGuild, IUserGuild
    {
        internal override bool IsAttached => true;

        private readonly SemaphoreSlim _audioLock;
        private TaskCompletionSource<bool> _syncPromise, _downloaderPromise;
        private TaskCompletionSource<AudioClient> _audioConnectPromise;
        private ConcurrentHashSet<ulong> _channels;
        private ConcurrentDictionary<ulong, SocketGuildUser> _members;
        private ConcurrentDictionary<ulong, VoiceState> _voiceStates;
        internal bool _available;

        public bool Available => _available && Discord.ConnectionState == ConnectionState.Connected;
        public int MemberCount { get; set; }
        public int DownloadedMemberCount { get; private set; }
        public AudioClient AudioClient { get; private set; }

        public bool HasAllMembers => _downloaderPromise.Task.IsCompleted;
        public bool IsSynced => _syncPromise.Task.IsCompleted;
        public Task SyncPromise => _syncPromise.Task;
        public Task DownloaderPromise => _downloaderPromise.Task;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public SocketGuildUser CurrentUser => GetUser(Discord.CurrentUser.Id);
        public IReadOnlyCollection<ISocketGuildChannel> Channels
        {
            get
            {
                var channels = _channels;
                var store = Discord.DataStore;
                return channels.Select(x => store.GetChannel(x) as ISocketGuildChannel).Where(x => x != null).ToReadOnlyCollection(channels);
            }
        }
        public IReadOnlyCollection<SocketGuildUser> Members => _members.ToReadOnlyCollection();
        public IEnumerable<KeyValuePair<ulong, VoiceState>> VoiceStates => _voiceStates;
        
        public SocketGuild(DiscordSocketClient discord, ExtendedModel model, DataStore dataStore) : base(discord, model)
        {
            _audioLock = new SemaphoreSlim(1, 1);
            _syncPromise = new TaskCompletionSource<bool>();
            _downloaderPromise = new TaskCompletionSource<bool>();
            Update(model, dataStore);
        }

        public void Update(ExtendedModel model, DataStore dataStore)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            _available = !(model.Unavailable ?? false);
            if (!_available)
            {
                if (_channels == null)
                    _channels = new ConcurrentHashSet<ulong>();
                if (_members == null)
                    _members = new ConcurrentDictionary<ulong, SocketGuildUser>();
                if (_roles == null)
                    _roles = new ConcurrentDictionary<ulong, Role>();
                if (Emojis == null)
                    Emojis = ImmutableArray.Create<Emoji>();
                if (Features == null)
                    Features = ImmutableArray.Create<string>();
                return;
            }

            base.Update(model as Model, source);
                        
            var channels = new ConcurrentHashSet<ulong>(1, (int)(model.Channels.Length * 1.05));
            {
                for (int i = 0; i < model.Channels.Length; i++)
                    AddChannel(model.Channels[i], dataStore, channels);
            }
            _channels = channels;

            var members = new ConcurrentDictionary<ulong, SocketGuildUser>(1, (int)(model.Presences.Length * 1.05));
            {
                DownloadedMemberCount = 0;
                for (int i = 0; i < model.Members.Length; i++)
                    AddOrUpdateUser(model.Members[i], dataStore, members);
                if (Discord.ApiClient.AuthTokenType != TokenType.User)
                {
                    var _ = _syncPromise.TrySetResultAsync(true);
                    if (!model.Large)
                        _ = _downloaderPromise.TrySetResultAsync(true);
                }

                for (int i = 0; i < model.Presences.Length; i++)
                    AddOrUpdateUser(model.Presences[i], dataStore, members);
            }
            _members = members;
            MemberCount = model.MemberCount;

            var voiceStates = new ConcurrentDictionary<ulong, VoiceState>(1, (int)(model.VoiceStates.Length * 1.05));
            {
                for (int i = 0; i < model.VoiceStates.Length; i++)
                    AddOrUpdateVoiceState(model.VoiceStates[i], dataStore, voiceStates);
            }
            _voiceStates = voiceStates;
        }
        public void Update(GuildSyncModel model, DataStore dataStore)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            var members = new ConcurrentDictionary<ulong, SocketGuildUser>(1, (int)(model.Presences.Length * 1.05));
            {
                DownloadedMemberCount = 0;
                for (int i = 0; i < model.Members.Length; i++)
                    AddOrUpdateUser(model.Members[i], dataStore, members);
                var _ = _syncPromise.TrySetResultAsync(true);
                if (!model.Large)
                    _ = _downloaderPromise.TrySetResultAsync(true);

                for (int i = 0; i < model.Presences.Length; i++)
                    AddOrUpdateUser(model.Presences[i], dataStore, members);
            }
            _members = members;
        }

        public void Update(EmojiUpdateModel model)
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
        public ISocketGuildChannel GetChannel(ulong id)
        {
            return Discord.DataStore.GetChannel(id) as ISocketGuildChannel;
        }
        public ISocketGuildChannel RemoveChannel(ulong id)
        {
            _channels.TryRemove(id);
            return Discord.DataStore.RemoveChannel(id) as ISocketGuildChannel;
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
        public SocketGuildUser AddOrUpdateUser(MemberModel model, DataStore dataStore, ConcurrentDictionary<ulong, SocketGuildUser> members = null)
        {
            members = members ?? _members;

            SocketGuildUser member;
            if (members.TryGetValue(model.User.Id, out member))
                member.Update(model, UpdateSource.WebSocket);
            else
            {
                var user = Discord.GetOrAddUser(model.User, dataStore);
                member = new SocketGuildUser(this, user, model);
                members[user.Id] = member;
                DownloadedMemberCount++;
            }
            return member;
        }
        public SocketGuildUser AddOrUpdateUser(PresenceModel model, DataStore dataStore, ConcurrentDictionary<ulong, SocketGuildUser> members = null)
        {
            members = members ?? _members;

            SocketGuildUser member;
            if (members.TryGetValue(model.User.Id, out member))
                member.Update(model, UpdateSource.WebSocket);
            else
            {
                var user = Discord.GetOrAddUser(model.User, dataStore);
                member = new SocketGuildUser(this, user, model);
                members[user.Id] = member;
                DownloadedMemberCount++;
            }            
            return member;
        }
        public SocketGuildUser GetUser(ulong id)
        {
            SocketGuildUser member;
            if (_members.TryGetValue(id, out member))
                return member;
            return null;
        }
        public SocketGuildUser RemoveUser(ulong id)
        {
            SocketGuildUser member;
            if (_members.TryRemove(id, out member))
            {
                DownloadedMemberCount--;
                return member;
            }
            member.User.RemoveRef(Discord);
            return null;
        }
        public override async Task DownloadUsersAsync()
        {
            await Discord.DownloadUsersAsync(new [] { this });
        }
        public void CompleteDownloadMembers()
        {
            _downloaderPromise.TrySetResultAsync(true);
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

        public async Task<IAudioClient> ConnectAudioAsync(ulong channelId, bool selfDeaf, bool selfMute)
        {
            try
            {
                TaskCompletionSource<AudioClient> promise;

                await _audioLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    await DisconnectAudioInternalAsync().ConfigureAwait(false);
                    promise = new TaskCompletionSource<AudioClient>();
                    _audioConnectPromise = promise;
                    await Discord.ApiClient.SendVoiceStateUpdateAsync(Id, channelId, selfDeaf, selfMute).ConfigureAwait(false);
                }
                finally
                {
                    _audioLock.Release();
                }

                var timeoutTask = Task.Delay(15000);
                if (await Task.WhenAny(promise.Task, timeoutTask) == timeoutTask)
                    throw new TimeoutException();
                return await promise.Task.ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DisconnectAudioInternalAsync().ConfigureAwait(false);
                throw;
            }
        }
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
        public async Task FinishConnectAudio(int id, string url, string token)
        {
            var voiceState = GetVoiceState(CurrentUser.Id).Value;

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
                        }
                        finally
                        {
                            _audioLock.Release();
                        }
                    };
                    AudioClient = audioClient;
                }
                await AudioClient.ConnectAsync(url, CurrentUser.Id, voiceState.VoiceSessionId, token).ConfigureAwait(false);
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
        public async Task FinishJoinAudioChannel()
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

        public SocketGuild Clone() => MemberwiseClone() as SocketGuild;

        new internal ISocketGuildChannel ToChannel(ChannelModel model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return new SocketTextChannel(this, model);
                case ChannelType.Voice:
                    return new SocketVoiceChannel(this, model);
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
        }

        bool IUserGuild.IsOwner => OwnerId == Discord.CurrentUser.Id;
        GuildPermissions IUserGuild.Permissions => CurrentUser.GuildPermissions;
        IAudioClient IGuild.AudioClient => AudioClient;
    }
}
