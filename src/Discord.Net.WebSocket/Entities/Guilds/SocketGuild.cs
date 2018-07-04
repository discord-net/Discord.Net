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
using UserModel = Discord.API.User;
using VoiceStateModel = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based guild object.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGuild : SocketEntity<ulong>, IGuild
    {
        private readonly SemaphoreSlim _audioLock;
        private TaskCompletionSource<bool> _syncPromise, _downloaderPromise;
        private TaskCompletionSource<AudioClient> _audioConnectPromise;
        private ConcurrentHashSet<ulong> _channels;
        private ConcurrentDictionary<ulong, SocketGuildUser> _members;
        private ConcurrentDictionary<ulong, SocketRole> _roles;
        private ConcurrentDictionary<ulong, SocketVoiceState> _voiceStates;
        private ImmutableArray<GuildEmote> _emotes;
        private ImmutableArray<string> _features;
        private AudioClient _audioClient;

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public int AFKTimeout { get; private set; }
        /// <inheritdoc />
        public bool IsEmbeddable { get; private set; }
        /// <inheritdoc />
        public VerificationLevel VerificationLevel { get; private set; }
        /// <inheritdoc />
        public MfaLevel MfaLevel { get; private set; }
        /// <inheritdoc />
        public DefaultMessageNotifications DefaultMessageNotifications { get; private set; }
        /// <summary>
        ///     Gets the number of members.
        /// </summary>
        /// <remarks>
        ///     This property retrieves the number of members returned by Discord and is the most accurate. You may see
        ///     discrepancy between the <see cref="Users"/> collection and this property.
        /// </remarks>
        public int MemberCount { get; internal set; }
        /// <summary> Gets the number of members downloaded to the local guild cache. </summary>
        public int DownloadedMemberCount { get; private set; }
        internal bool IsAvailable { get; private set; }
        /// <summary> Indicates whether the client is connected to this guild. </summary>
        public bool IsConnected { get; internal set; }

        internal ulong? AFKChannelId { get; private set; }
        internal ulong? EmbedChannelId { get; private set; }
        internal ulong? SystemChannelId { get; private set; }
        /// <inheritdoc />
        public ulong OwnerId { get; private set; }
        /// <summary> Gets the user that owns this guild. </summary>
        public SocketGuildUser Owner => GetUser(OwnerId);
        /// <inheritdoc />
        public string VoiceRegionId { get; private set; }
        /// <inheritdoc />
        public string IconId { get; private set; }
        /// <inheritdoc />
        public string SplashId { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string IconUrl => CDN.GetGuildIconUrl(Id, IconId);
        /// <inheritdoc />
        public string SplashUrl => CDN.GetGuildSplashUrl(Id, SplashId);
        /// <summary> Indicates whether the client has all the members downloaded to the local guild cache. </summary>
        public bool HasAllMembers => MemberCount == DownloadedMemberCount;// _downloaderPromise.Task.IsCompleted;
        /// <summary> Indicates whether the guild cache is synced to this guild. </summary>
        public bool IsSynced => _syncPromise.Task.IsCompleted;
        public Task SyncPromise => _syncPromise.Task;
        public Task DownloaderPromise => _downloaderPromise.Task;
        /// <summary>
        ///     Gets the <see cref="IAudioClient" /> associated with this guild.
        /// </summary>
        public IAudioClient AudioClient => _audioClient;
        /// <summary>
        ///     Gets the first viewable text channel.
        /// </summary>
        /// <remarks>
        ///     This method gets the first viewable text channel for the logged-in user. This property does not
        ///     guarantee the user can send message to it.
        /// </remarks>
        public SocketTextChannel DefaultChannel => TextChannels
            .Where(c => CurrentUser.GetPermissions(c).ViewChannel)
            .OrderBy(c => c.Position)
            .FirstOrDefault();
        /// <summary>
        ///     Gets the AFK voice channel in this guild.
        /// </summary>
        /// <returns>
        ///     A voice channel set within this guild that AFK users get moved to; otherwise <c>null</c> if none is set.
        /// </returns>
        public SocketVoiceChannel AFKChannel
        {
            get
            {
                var id = AFKChannelId;
                return id.HasValue ? GetVoiceChannel(id.Value) : null;
            }
        }
        /// <summary>
        ///     Gets the embed channel set in the widget settings of this guild.
        /// </summary>
        /// <returns>
        ///     A channel set in the widget settings of this guild; otherwise <c>null</c> if none is set.
        /// </returns>
        public SocketGuildChannel EmbedChannel
        {
            get
            {
                var id = EmbedChannelId;
                return id.HasValue ? GetChannel(id.Value) : null;
            }
        }
        /// <summary>
        ///     Gets the channel where randomized welcome messages are sent.
        /// </summary>
        /// <returns>
        ///     A channel where randomized welcome messages are sent in this guild; otherwise <c>null</c> if none is
        ///     set.
        /// </returns>
        public SocketTextChannel SystemChannel
        {
            get
            {
                var id = SystemChannelId;
                return id.HasValue ? GetTextChannel(id.Value) : null;
            }
        }
        /// <summary>
        ///     Gets a collection of text channels present in this guild.
        /// </summary>
        /// <returns>
        ///     A collection of text channels in this guild.
        /// </returns>
        public IReadOnlyCollection<SocketTextChannel> TextChannels
            => Channels.Select(x => x as SocketTextChannel).Where(x => x != null).ToImmutableArray();
        /// <summary>
        ///     Gets a collection of voice channels present in this guild.
        /// </summary>
        public IReadOnlyCollection<SocketVoiceChannel> VoiceChannels
            => Channels.Select(x => x as SocketVoiceChannel).Where(x => x != null).ToImmutableArray();
        /// <summary>
        ///     Gets a collection of category channels present in this guild.
        /// </summary>
        public IReadOnlyCollection<SocketCategoryChannel> CategoryChannels
            => Channels.Select(x => x as SocketCategoryChannel).Where(x => x != null).ToImmutableArray();
        /// <summary>
        ///     Gets the current logged-in user.
        /// </summary>
        public SocketGuildUser CurrentUser => _members.TryGetValue(Discord.CurrentUser.Id, out SocketGuildUser member) ? member : null;
        /// <summary>
        ///     Gets the @everyone role in this guild.
        /// </summary>
        public SocketRole EveryoneRole => GetRole(Id);
        /// <summary>
        ///     Gets a collection of channels present in this guild.
        /// </summary>
        /// <returns>
        ///     Collection of channels.
        /// </returns>
        public IReadOnlyCollection<SocketGuildChannel> Channels
        {
            get
            {
                var channels = _channels;
                var state = Discord.State;
                return channels.Select(x => state.GetChannel(x) as SocketGuildChannel).Where(x => x != null).ToReadOnlyCollection(channels);
            }
        }
        /// <summary>
        ///     Gets a collection of emotes created in this guild.
        /// </summary>
        /// <returns>
        ///     Collection of emotes.
        /// </returns>
        public IReadOnlyCollection<GuildEmote> Emotes => _emotes;
        /// <summary>
        ///     Gets a collection of features enabled in this guild.
        /// </summary>
        /// <returns>
        ///     Collection of features in string.
        /// </returns>
        public IReadOnlyCollection<string> Features => _features;
        /// <summary>
        ///     Gets a collection of users in this guild.
        /// </summary>
        /// <remarks>
        /// This property may not always return all the members for large guilds (i.e. guilds containing 100+ users).
        /// You may need to enable <see cref="DiscordSocketConfig.AlwaysDownloadUsers"/> to fetch the full user list
        /// upon startup, or use <see cref="DownloadUsersAsync"/> to manually download the users.
        /// </remarks>
        /// <returns>
        ///     Collection of users.
        /// </returns>
        public IReadOnlyCollection<SocketGuildUser> Users => _members.ToReadOnlyCollection();
        /// <summary>
        ///     Gets a collection of roles in this guild.
        /// </summary>
        /// <returns>
        ///     Collection of roles.
        /// </returns>
        public IReadOnlyCollection<SocketRole> Roles => _roles.ToReadOnlyCollection();

        internal SocketGuild(DiscordSocketClient client, ulong id)
            : base(client, id)
        {
            _audioLock = new SemaphoreSlim(1, 1);
            _emotes = ImmutableArray.Create<GuildEmote>();
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
            IsAvailable = !(model.Unavailable ?? false);
            if (!IsAvailable)
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
                _syncPromise = new TaskCompletionSource<bool>();
                _downloaderPromise = new TaskCompletionSource<bool>();
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
                    if (members.TryGetValue(model.Presences[i].User.Id, out SocketGuildUser member))
                        member.Update(state, model.Presences[i], true);
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
            var _ = _syncPromise.TrySetResultAsync(true);
            /*if (!model.Large)
                _ = _downloaderPromise.TrySetResultAsync(true);*/
        }
        internal void Update(ClientState state, Model model)
        {
            AFKChannelId = model.AFKChannelId;
            EmbedChannelId = model.EmbedChannelId;
            SystemChannelId = model.SystemChannelId;
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
                var emojis = ImmutableArray.CreateBuilder<GuildEmote>(model.Emojis.Length);
                for (int i = 0; i < model.Emojis.Length; i++)
                    emojis.Add(model.Emojis[i].ToEntity());
                _emotes = emojis.ToImmutable();
            }
            else
                _emotes = ImmutableArray.Create<GuildEmote>();

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
                    if (members.TryGetValue(model.Presences[i].User.Id, out SocketGuildUser member))
                        member.Update(state, model.Presences[i], true);
                }
            }
            _members = members;

            var _ = _syncPromise.TrySetResultAsync(true);
            /*if (!model.Large)
                _ = _downloaderPromise.TrySetResultAsync(true);*/
        }

        internal void Update(ClientState state, EmojiUpdateModel model)
        {
            var emotes = ImmutableArray.CreateBuilder<GuildEmote>(model.Emojis.Length);
            for (int i = 0; i < model.Emojis.Length; i++)
                emotes.Add(model.Emojis[i].ToEntity());
            _emotes = emotes.ToImmutable();
        }

        //General
        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => GuildHelper.DeleteAsync(this, Discord, options);

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
        public Task ModifyAsync(Action<GuildProperties> func, RequestOptions options = null)
            => GuildHelper.ModifyAsync(this, Discord, func, options);

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
        public Task ModifyEmbedAsync(Action<GuildEmbedProperties> func, RequestOptions options = null)
            => GuildHelper.ModifyEmbedAsync(this, Discord, func, options);
        /// <inheritdoc />
        public Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions options = null)
            => GuildHelper.ReorderChannelsAsync(this, Discord, args, options);
        /// <inheritdoc />
        public Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions options = null)
            => GuildHelper.ReorderRolesAsync(this, Discord, args, options);

        /// <inheritdoc />
        public Task LeaveAsync(RequestOptions options = null)
            => GuildHelper.LeaveAsync(this, Discord, options);

        //Bans
        /// <summary>
        ///     Returns a collection of the banned users in this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A collection of bans.
        /// </returns>
        public Task<IReadOnlyCollection<RestBan>> GetBansAsync(RequestOptions options = null)
            => GuildHelper.GetBansAsync(this, Discord, options);
        public Task<RestBan> GetBanAsync(IUser user, RequestOptions options = null)
            => GuildHelper.GetBanAsync(this, Discord, user.Id, options);
        public Task<RestBan> GetBanAsync(ulong userId, RequestOptions options = null)
            => GuildHelper.GetBanAsync(this, Discord, userId, options);

        /// <inheritdoc />
        public Task AddBanAsync(IUser user, int pruneDays = 0, string reason = null, RequestOptions options = null)
            => GuildHelper.AddBanAsync(this, Discord, user.Id, pruneDays, reason, options);
        /// <inheritdoc />
        public Task AddBanAsync(ulong userId, int pruneDays = 0, string reason = null, RequestOptions options = null)
            => GuildHelper.AddBanAsync(this, Discord, userId, pruneDays, reason, options);

        /// <inheritdoc />
        public Task RemoveBanAsync(IUser user, RequestOptions options = null)
            => GuildHelper.RemoveBanAsync(this, Discord, user.Id, options);
        /// <inheritdoc />
        public Task RemoveBanAsync(ulong userId, RequestOptions options = null)
            => GuildHelper.RemoveBanAsync(this, Discord, userId, options);

        //Channels
        /// <summary>
        ///     Returns a guild channel with the provided ID.
        /// </summary>
        /// <param name="id">The channel ID.</param>
        /// <returns>
        ///     The guild channel associated with the ID.
        /// </returns>
        public SocketGuildChannel GetChannel(ulong id)
        {
            var channel = Discord.State.GetChannel(id) as SocketGuildChannel;
            if (channel?.Guild.Id == Id)
                return channel;
            return null;
        }
        /// <summary>
        ///     Returns a text channel with the provided ID.
        /// </summary>
        /// <param name="id">The channel ID.</param>
        /// <returns>
        ///     The text channel associated with the ID.
        /// </returns>
        public SocketTextChannel GetTextChannel(ulong id)
            => GetChannel(id) as SocketTextChannel;
        /// <summary>
        ///     Returns a voice channel with the provided ID.
        /// </summary>
        /// <param name="id">The channel ID.</param>
        /// <returns>
        ///     The voice channel associated with the ID.
        /// </returns>
        public SocketVoiceChannel GetVoiceChannel(ulong id)
            => GetChannel(id) as SocketVoiceChannel;

        /// <summary>
        ///     Creates a text channel with the provided name.
        /// </summary>
        /// <param name="name">The name of the new channel.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        /// <returns>
        ///     The created text channel.
        /// </returns>
        public Task<RestTextChannel> CreateTextChannelAsync(string name, Action<TextChannelProperties> func = null, RequestOptions options = null)
            => GuildHelper.CreateTextChannelAsync(this, Discord, name, options, func);
        /// <summary>
        ///     Creates a voice channel with the provided name.
        /// </summary>
        /// <param name="name">The name of the new channel.</param>
        /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        /// <returns>
        ///     The created voice channel.
        /// </returns>
        public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func = null, RequestOptions options = null)
            => GuildHelper.CreateVoiceChannelAsync(this, Discord, name, options, func);
        /// <summary>
        ///     Creates a category channel with the provided name.
        /// </summary>
        /// <param name="name">The name of the new channel.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        /// <returns>
        ///     The created category channel.
        /// </returns>
        public Task<RestCategoryChannel> CreateCategoryChannelAsync(string name, RequestOptions options = null)
            => GuildHelper.CreateCategoryChannelAsync(this, Discord, name, options);

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
        /// <summary>
        ///     Returns a collection of invites associated with this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of invites.
        /// </returns>
        public Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => GuildHelper.GetInvitesAsync(this, Discord, options);
        /// <summary>
        ///     Gets the vanity invite URL of this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A partial metadata of the vanity invite found within this guild.
        /// </returns>
        public Task<RestInviteMetadata> GetVanityInviteAsync(RequestOptions options = null)
            => GuildHelper.GetVanityInviteAsync(this, Discord, options);

        //Roles
        /// <summary>
        ///     Gets a role.
        /// </summary>
        /// <param name="id">The snowflake identifier of the role.</param>
        /// <returns>
        ///     The role associated with the snowflake identifier.
        /// </returns>
        public SocketRole GetRole(ulong id)
        {
            if (_roles.TryGetValue(id, out SocketRole value))
                return value;
            return null;
        }

        /// <summary>
        ///     Creates a role.
        /// </summary>
        /// <param name="name">The name of the new role.</param>
        /// <param name="permissions">
        /// The permissions that the new role possesses. Set to <c>null</c> to use the default permissions.
        /// </param>
        /// <param name="color">The color of the role. Set to <c>null</c> to use the default color.</param>
        /// <param name="isHoisted">Used to determine if users of this role are separated in the user list.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        /// <returns>
        ///     The created role.
        /// </returns>
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
            if (_roles.TryRemove(id, out SocketRole role))
                return role;
            return null;
        }

        //Users
        /// <summary>
        ///     Gets the user with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>
        ///     The user associated with the ID.
        /// </returns>
        public SocketGuildUser GetUser(ulong id)
        {
            if (_members.TryGetValue(id, out SocketGuildUser member))
                return member;
            return null;
        }
        /// <inheritdoc />
        public Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null)
            => GuildHelper.PruneUsersAsync(this, Discord, days, simulate, options);

        internal SocketGuildUser AddOrUpdateUser(UserModel model)
        {
            if (_members.TryGetValue(model.Id, out SocketGuildUser member))
                member.GlobalUser?.Update(Discord.State, model);
            else
            {
                member = SocketGuildUser.Create(this, Discord.State, model);
                member.GlobalUser.AddRef();
                _members[member.Id] = member;
                DownloadedMemberCount++;
            }
            return member;
        }
        internal SocketGuildUser AddOrUpdateUser(MemberModel model)
        {
            if (_members.TryGetValue(model.User.Id, out SocketGuildUser member))
                member.Update(Discord.State, model);
            else
            {
                member = SocketGuildUser.Create(this, Discord.State, model);
                member.GlobalUser.AddRef();
                _members[member.Id] = member;
                DownloadedMemberCount++;
            }
            return member;
        }
        internal SocketGuildUser AddOrUpdateUser(PresenceModel model)
        {
            if (_members.TryGetValue(model.User.Id, out SocketGuildUser member))
                member.Update(Discord.State, model, false);
            else
            {
                member = SocketGuildUser.Create(this, Discord.State, model);
                member.GlobalUser.AddRef();
                _members[member.Id] = member;
                DownloadedMemberCount++;
            }
            return member;
        }
        internal SocketGuildUser RemoveUser(ulong id)
        {
            if (_members.TryRemove(id, out SocketGuildUser member))
            {
                DownloadedMemberCount--;
                member.GlobalUser.RemoveRef(Discord);
                return member;
            }
            return null;
        }

        /// <summary>
        ///     Downloads the users of this guild to the WebSocket cache.
        /// </summary>
        public async Task DownloadUsersAsync()
        {
            await Discord.DownloadUsersAsync(new[] { this }).ConfigureAwait(false);
        }
        internal void CompleteDownloadUsers()
        {
            _downloaderPromise.TrySetResultAsync(true);
        }

        //Audit logs
        public IAsyncEnumerable<IReadOnlyCollection<RestAuditLogEntry>> GetAuditLogsAsync(int limit, RequestOptions options = null)
            => GuildHelper.GetAuditLogsAsync(this, Discord, null, limit, options);

        //Webhooks
        /// <summary>
        ///     Returns the webhook with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing the webhook associated with the ID.
        /// </returns>
        public Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => GuildHelper.GetWebhookAsync(this, Discord, id, options);
        /// <summary>
        ///     Gets a collection of webhooks that exist in the guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/> containing a collection of webhooks.
        /// </returns>
        public Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
            => GuildHelper.GetWebhooksAsync(this, Discord, options);

        //Emotes
        /// <inheritdoc />
        public Task<GuildEmote> GetEmoteAsync(ulong id, RequestOptions options = null)
            => GuildHelper.GetEmoteAsync(this, Discord, id, options);
        /// <inheritdoc />
        public Task<GuildEmote> CreateEmoteAsync(string name, Image image, Optional<IEnumerable<IRole>> roles = default(Optional<IEnumerable<IRole>>), RequestOptions options = null)
            => GuildHelper.CreateEmoteAsync(this, Discord, name, image, roles, options);
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
        public Task<GuildEmote> ModifyEmoteAsync(GuildEmote emote, Action<EmoteProperties> func, RequestOptions options = null)
            => GuildHelper.ModifyEmoteAsync(this, Discord, emote.Id, func, options);
        /// <inheritdoc />
        public Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null)
            => GuildHelper.DeleteEmoteAsync(this, Discord, emote.Id, options);

        //Voice States
        internal async Task<SocketVoiceState> AddOrUpdateVoiceStateAsync(ClientState state, VoiceStateModel model)
        {
            var voiceChannel = state.GetChannel(model.ChannelId.Value) as SocketVoiceChannel;
            var before = GetVoiceState(model.UserId) ?? SocketVoiceState.Default;
            var after = SocketVoiceState.Create(voiceChannel, model);
            _voiceStates[model.UserId] = after;

            if (_audioClient != null && before.VoiceChannel?.Id != after.VoiceChannel?.Id)
            {
                if (model.UserId == CurrentUser.Id)
                {
                    if (after.VoiceChannel != null && _audioClient.ChannelId != after.VoiceChannel?.Id)
                    {
                        _audioClient.ChannelId = after.VoiceChannel.Id;
                        await RepopulateAudioStreamsAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    await _audioClient.RemoveInputStreamAsync(model.UserId).ConfigureAwait(false); //User changed channels, end their stream
                    if (CurrentUser.VoiceChannel != null && after.VoiceChannel?.Id == CurrentUser.VoiceChannel?.Id)
                        await _audioClient.CreateInputStreamAsync(model.UserId).ConfigureAwait(false);
                }
            }

            return after;
        }
        internal SocketVoiceState? GetVoiceState(ulong id)
        {
            if (_voiceStates.TryGetValue(id, out SocketVoiceState voiceState))
                return voiceState;
            return null;
        }
        internal async Task<SocketVoiceState?> RemoveVoiceStateAsync(ulong id)
        {
            if (_voiceStates.TryRemove(id, out SocketVoiceState voiceState))
            {
                if (_audioClient != null)
                    await _audioClient.RemoveInputStreamAsync(id).ConfigureAwait(false); //User changed channels, end their stream
                return voiceState;
            }
            return null;
        }

        //Audio
        internal AudioInStream GetAudioStream(ulong userId)
        {
            return _audioClient?.GetInputStream(userId);
        }
        internal async Task<IAudioClient> ConnectAudioAsync(ulong channelId, bool selfDeaf, bool selfMute, bool external)
        {
            TaskCompletionSource<AudioClient> promise;

            await _audioLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectAudioInternalAsync().ConfigureAwait(false);
                promise = new TaskCompletionSource<AudioClient>();
                _audioConnectPromise = promise;

                if (external)
                {
                    var _ = promise.TrySetResultAsync(null);
                    await Discord.ApiClient.SendVoiceStateUpdateAsync(Id, channelId, selfDeaf, selfMute).ConfigureAwait(false);
                    return null;
                }

                if (_audioClient == null)
                {
                    var audioClient = new AudioClient(this, Discord.GetAudioId(), channelId);
                    audioClient.Disconnected += async ex =>
                    {
                        if (!promise.Task.IsCompleted)
                        {
                            try
                            { audioClient.Dispose(); }
                            catch { }
                            _audioClient = null;
                            if (ex != null)
                                await promise.TrySetExceptionAsync(ex);
                            else
                                await promise.TrySetCanceledAsync();
                            return;
                        }
                    };
                    audioClient.Connected += () =>
                    {
                        var _ = promise.TrySetResultAsync(_audioClient);
                        return Task.Delay(0);
                    };
                    _audioClient = audioClient;
                }

                await Discord.ApiClient.SendVoiceStateUpdateAsync(Id, channelId, selfDeaf, selfMute).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DisconnectAudioInternalAsync().ConfigureAwait(false);
                throw;
            }
            finally
            {
                _audioLock.Release();
            }

            try
            {
                var timeoutTask = Task.Delay(15000);
                if (await Task.WhenAny(promise.Task, timeoutTask).ConfigureAwait(false) == timeoutTask)
                    throw new TimeoutException();
                return await promise.Task.ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DisconnectAudioAsync().ConfigureAwait(false);
                throw;
            }
        }

        internal async Task DisconnectAudioAsync()
        {
            await _audioLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectAudioInternalAsync().ConfigureAwait(false);
            }
            finally
            {
                _audioLock.Release();
            }
        }
        private async Task DisconnectAudioInternalAsync()
        {
            _audioConnectPromise?.TrySetCanceledAsync(); //Cancel any previous audio connection
            _audioConnectPromise = null;
            if (_audioClient != null)
                await _audioClient.StopAsync().ConfigureAwait(false);
            await Discord.ApiClient.SendVoiceStateUpdateAsync(Id, null, false, false).ConfigureAwait(false);
            _audioClient = null;
        }
        internal async Task FinishConnectAudio(string url, string token)
        {
            //TODO: Mem Leak: Disconnected/Connected handlers arent cleaned up
            var voiceState = GetVoiceState(Discord.CurrentUser.Id).Value;

            await _audioLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_audioClient != null)
                {
                    await RepopulateAudioStreamsAsync().ConfigureAwait(false);
                    await _audioClient.StartAsync(url, Discord.CurrentUser.Id, voiceState.VoiceSessionId, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                await DisconnectAudioInternalAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _audioConnectPromise.SetExceptionAsync(e).ConfigureAwait(false);
                await DisconnectAudioInternalAsync().ConfigureAwait(false);
            }
            finally
            {
                _audioLock.Release();
            }
        }

        internal async Task RepopulateAudioStreamsAsync()
        {
            await _audioClient.ClearInputStreamsAsync().ConfigureAwait(false); //We changed channels, end all current streams
            if (CurrentUser.VoiceChannel != null)
            {
                foreach (var pair in _voiceStates)
                {
                    if (pair.Value.VoiceChannel?.Id == CurrentUser.VoiceChannel?.Id && pair.Key != CurrentUser.Id)
                        await _audioClient.CreateInputStreamAsync(pair.Key).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        ///     Gets the name of the guild.
        /// </summary>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
        internal SocketGuild Clone() => MemberwiseClone() as SocketGuild;

        //IGuild
        /// <inheritdoc />
        ulong? IGuild.AFKChannelId => AFKChannelId;
        /// <inheritdoc />
        IAudioClient IGuild.AudioClient => null;
        /// <inheritdoc />
        bool IGuild.Available => true;
        /// <inheritdoc />
        ulong IGuild.DefaultChannelId => DefaultChannel?.Id ?? 0;
        /// <inheritdoc />
        ulong? IGuild.EmbedChannelId => EmbedChannelId;
        /// <inheritdoc />
        ulong? IGuild.SystemChannelId => SystemChannelId;
        /// <inheritdoc />
        IRole IGuild.EveryoneRole => EveryoneRole;
        /// <inheritdoc />
        IReadOnlyCollection<IRole> IGuild.Roles => Roles;

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(RequestOptions options)
            => await GetBansAsync(options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IBan> IGuild.GetBanAsync(IUser user, RequestOptions options)
            => await GetBanAsync(user, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IBan> IGuild.GetBanAsync(ulong userId, RequestOptions options)
            => await GetBanAsync(userId, options).ConfigureAwait(false);

        /// <inheritdoc />
        Task<IReadOnlyCollection<IGuildChannel>> IGuild.GetChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGuildChannel>>(Channels);
        /// <inheritdoc />
        Task<IGuildChannel> IGuild.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildChannel>(GetChannel(id));
        /// <inheritdoc />
        Task<IReadOnlyCollection<ITextChannel>> IGuild.GetTextChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<ITextChannel>>(TextChannels);
        /// <inheritdoc />
        Task<ITextChannel> IGuild.GetTextChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<ITextChannel>(GetTextChannel(id));
        /// <inheritdoc />
        Task<IReadOnlyCollection<IVoiceChannel>> IGuild.GetVoiceChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IVoiceChannel>>(VoiceChannels);
        /// <inheritdoc />
        Task<IReadOnlyCollection<ICategoryChannel>> IGuild.GetCategoriesAsync(CacheMode mode , RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<ICategoryChannel>>(CategoryChannels);
        /// <inheritdoc />
        Task<IVoiceChannel> IGuild.GetVoiceChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IVoiceChannel>(GetVoiceChannel(id));
        /// <inheritdoc />
        Task<IVoiceChannel> IGuild.GetAFKChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IVoiceChannel>(AFKChannel);
        /// <inheritdoc />
        Task<ITextChannel> IGuild.GetDefaultChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<ITextChannel>(DefaultChannel);
        /// <inheritdoc />
        Task<IGuildChannel> IGuild.GetEmbedChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildChannel>(EmbedChannel);
        /// <inheritdoc />
        Task<ITextChannel> IGuild.GetSystemChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<ITextChannel>(SystemChannel);
        /// <inheritdoc />
        async Task<ITextChannel> IGuild.CreateTextChannelAsync(string name, Action<TextChannelProperties> func, RequestOptions options)
            => await CreateTextChannelAsync(name, func, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func, RequestOptions options)
            => await CreateVoiceChannelAsync(name, func, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<ICategoryChannel> IGuild.CreateCategoryAsync(string name, RequestOptions options)
            => await CreateCategoryChannelAsync(name, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IGuildIntegration>> IGuild.GetIntegrationsAsync(RequestOptions options)
            => await GetIntegrationsAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IGuildIntegration> IGuild.CreateIntegrationAsync(ulong id, string type, RequestOptions options)
            => await CreateIntegrationAsync(id, type, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IInviteMetadata>> IGuild.GetInvitesAsync(RequestOptions options)
            => await GetInvitesAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IInviteMetadata> IGuild.GetVanityInviteAsync(RequestOptions options)
            => await GetVanityInviteAsync(options).ConfigureAwait(false);

        /// <inheritdoc />
        IRole IGuild.GetRole(ulong id)
            => GetRole(id);
        /// <inheritdoc />
        async Task<IRole> IGuild.CreateRoleAsync(string name, GuildPermissions? permissions, Color? color, bool isHoisted, RequestOptions options)
            => await CreateRoleAsync(name, permissions, color, isHoisted, options).ConfigureAwait(false);

        /// <inheritdoc />
        Task<IReadOnlyCollection<IGuildUser>> IGuild.GetUsersAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Users);
        /// <inheritdoc />
        Task<IGuildUser> IGuild.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetUser(id));
        /// <inheritdoc />
        Task<IGuildUser> IGuild.GetCurrentUserAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(CurrentUser);
        /// <inheritdoc />
        Task<IGuildUser> IGuild.GetOwnerAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(Owner);

        async Task<IReadOnlyCollection<IAuditLogEntry>> IGuild.GetAuditLogAsync(int limit, CacheMode cacheMode, RequestOptions options)
        {
            if (cacheMode == CacheMode.AllowDownload)
                return (await GetAuditLogsAsync(limit, options).FlattenAsync().ConfigureAwait(false)).ToImmutableArray();
            else
                return ImmutableArray.Create<IAuditLogEntry>();
        }

        /// <inheritdoc />
        async Task<IWebhook> IGuild.GetWebhookAsync(ulong id, RequestOptions options)
            => await GetWebhookAsync(id, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IWebhook>> IGuild.GetWebhooksAsync(RequestOptions options)
            => await GetWebhooksAsync(options).ConfigureAwait(false);
    }
}
