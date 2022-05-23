using Discord.API.Gateway;
using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
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
using StickerModel = Discord.API.Sticker;
using EventModel = Discord.API.GuildScheduledEvent;
using System.IO;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based guild object.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGuild : SocketEntity<ulong>, IGuild, IDisposable
    {
        #region SocketGuild
#pragma warning disable IDISP002, IDISP006
        private readonly SemaphoreSlim _audioLock;
        private TaskCompletionSource<bool> _syncPromise, _downloaderPromise;
        private TaskCompletionSource<AudioClient> _audioConnectPromise;
        private ConcurrentDictionary<ulong, SocketGuildChannel> _channels;
        private ConcurrentDictionary<ulong, SocketGuildUser> _members;
        private ConcurrentDictionary<ulong, SocketRole> _roles;
        private ConcurrentDictionary<ulong, SocketVoiceState> _voiceStates;
        private ConcurrentDictionary<ulong, SocketCustomSticker> _stickers;
        private ConcurrentDictionary<ulong, SocketGuildEvent> _events;
        private ImmutableArray<GuildEmote> _emotes;

        private AudioClient _audioClient;
        private VoiceStateUpdateParams _voiceStateUpdateParams;
#pragma warning restore IDISP002, IDISP006

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public int AFKTimeout { get; private set; }
        /// <inheritdoc />
        public bool IsWidgetEnabled { get; private set; }
        /// <inheritdoc />
        public VerificationLevel VerificationLevel { get; private set; }
        /// <inheritdoc />
        public MfaLevel MfaLevel { get; private set; }
        /// <inheritdoc />
        public DefaultMessageNotifications DefaultMessageNotifications { get; private set; }
        /// <inheritdoc />
        public ExplicitContentFilterLevel ExplicitContentFilter { get; private set; }
        /// <summary>
        ///     Gets the number of members.
        /// </summary>
        /// <remarks>
        ///     This property retrieves the number of members returned by Discord.
        ///     <note type="tip">
        ///     <para>
        ///         Due to how this property is returned by Discord instead of relying on the WebSocket cache, the
        ///         number here is the most accurate in terms of counting the number of users within this guild.
        ///     </para>
        ///     <para>
        ///         Use this instead of enumerating the count of the
        ///         <see cref="Discord.WebSocket.SocketGuild.Users" /> collection, as you may see discrepancy
        ///         between that and this property.
        ///     </para>
        ///     </note>
        /// </remarks>
        public int MemberCount { get; internal set; }
        /// <summary> Gets the number of members downloaded to the local guild cache. </summary>
        public int DownloadedMemberCount { get; private set; }
        internal bool IsAvailable { get; private set; }
        /// <summary> Indicates whether the client is connected to this guild. </summary>
        public bool IsConnected { get; internal set; }
        /// <inheritdoc />
        public ulong? ApplicationId { get; internal set; }

        internal ulong? AFKChannelId { get; private set; }
        internal ulong? WidgetChannelId { get; private set; }
        internal ulong? SystemChannelId { get; private set; }
        internal ulong? RulesChannelId { get; private set; }
        internal ulong? PublicUpdatesChannelId { get; private set; }
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
        public string DiscoverySplashId { get; private set; }
        /// <inheritdoc />
        public PremiumTier PremiumTier { get; private set; }
        /// <inheritdoc />
        public string BannerId { get; private set; }
        /// <inheritdoc />
        public string VanityURLCode { get; private set; }
        /// <inheritdoc />
        public SystemChannelMessageDeny SystemChannelFlags { get; private set; }
        /// <inheritdoc />
        public string Description { get; private set; }
        /// <inheritdoc />
        public int PremiumSubscriptionCount { get; private set; }
        /// <inheritdoc />
        public string PreferredLocale { get; private set; }
        /// <inheritdoc />
        public int? MaxPresences { get; private set; }
        /// <inheritdoc />
        public int? MaxMembers { get; private set; }
        /// <inheritdoc />
        public int? MaxVideoChannelUsers { get; private set; }
        /// <inheritdoc />
        public NsfwLevel NsfwLevel { get; private set; }
        /// <inheritdoc />
        public CultureInfo PreferredCulture { get; private set; }
        /// <inheritdoc />
        public bool IsBoostProgressBarEnabled { get; private set; }
        /// <inheritdoc />
        public GuildFeatures Features { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string IconUrl => CDN.GetGuildIconUrl(Id, IconId);
        /// <inheritdoc />
        public string SplashUrl => CDN.GetGuildSplashUrl(Id, SplashId);
        /// <inheritdoc />
        public string DiscoverySplashUrl => CDN.GetGuildDiscoverySplashUrl(Id, DiscoverySplashId);
        /// <inheritdoc />
        public string BannerUrl => CDN.GetGuildBannerUrl(Id, BannerId, ImageFormat.Auto);
        /// <summary> Indicates whether the client has all the members downloaded to the local guild cache. </summary>
        public bool HasAllMembers => MemberCount <= DownloadedMemberCount;// _downloaderPromise.Task.IsCompleted;
        /// <summary> Indicates whether the guild cache is synced to this guild. </summary>
        public bool IsSynced => _syncPromise.Task.IsCompleted;
        public Task SyncPromise => _syncPromise.Task;
        public Task DownloaderPromise => _downloaderPromise.Task;
        /// <summary>
        ///     Gets the <see cref="IAudioClient" /> associated with this guild.
        /// </summary>
        public IAudioClient AudioClient => _audioClient;
        /// <summary>
        ///     Gets the default channel in this guild.
        /// </summary>
        /// <remarks>
        ///     This property retrieves the first viewable text channel for this guild.
        ///     <note type="warning">
        ///         This channel does not guarantee the user can send message to it, as it only looks for the first viewable
        ///         text channel.
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A <see cref="SocketTextChannel"/> representing the first viewable channel that the user has access to.
        /// </returns>
        public SocketTextChannel DefaultChannel => TextChannels
            .Where(c => CurrentUser.GetPermissions(c).ViewChannel && c is not IThreadChannel)
            .OrderBy(c => c.Position)
            .FirstOrDefault();
        /// <summary>
        ///     Gets the AFK voice channel in this guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="SocketVoiceChannel" /> that the AFK users will be moved to after they have idled for too
        ///     long; <see langword="null"/> if none is set.
        /// </returns>
        public SocketVoiceChannel AFKChannel
        {
            get
            {
                var id = AFKChannelId;
                return id.HasValue ? GetVoiceChannel(id.Value) : null;
            }
        }
        /// <inheritdoc/>
        public int MaxBitrate
        {
            get
            {
                return PremiumTier switch
                {
                    PremiumTier.Tier1 => 128000,
                    PremiumTier.Tier2 => 256000,
                    PremiumTier.Tier3 => 384000,
                    _ => 96000,
                };
            }
        }
        /// <inheritdoc/>
        public ulong MaxUploadLimit
            => GuildHelper.GetUploadLimit(this);
        /// <summary>
        ///     Gets the widget channel (i.e. the channel set in the guild's widget settings) in this guild.
        /// </summary>
        /// <returns>
        ///     A channel set within the server's widget settings; <see langword="null"/> if none is set.
        /// </returns>
        public SocketGuildChannel WidgetChannel
        {
            get
            {
                var id = WidgetChannelId;
                return id.HasValue ? GetChannel(id.Value) : null;
            }
        }
        /// <summary>
        ///     Gets the system channel where randomized welcome messages are sent in this guild.
        /// </summary>
        /// <returns>
        ///     A text channel where randomized welcome messages will be sent to; <see langword="null"/> if none is set.
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
        ///     Gets the channel with the guild rules.
        /// </summary>
        /// <returns>
        ///     A text channel with the guild rules; <see langword="null"/> if none is set.
        /// </returns>
        public SocketTextChannel RulesChannel
        {
            get
            {
                var id = RulesChannelId;
                return id.HasValue ? GetTextChannel(id.Value) : null;
            }
        }
        /// <summary>
        ///     Gets the channel where admins and moderators of Community guilds receive
        ///     notices from Discord.
        /// </summary>
        /// <returns>
        ///     A text channel where admins and moderators of Community guilds receive
        ///     notices from Discord; <see langword="null"/> if none is set.
        /// </returns>
        public SocketTextChannel PublicUpdatesChannel
        {
            get
            {
                var id = PublicUpdatesChannelId;
                return id.HasValue ? GetTextChannel(id.Value) : null;
            }
        }
        /// <summary>
        ///     Gets a collection of all text channels in this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of message channels found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketTextChannel> TextChannels
            => Channels.OfType<SocketTextChannel>().ToImmutableArray();
        /// <summary>
        ///     Gets a collection of all voice channels in this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of voice channels found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketVoiceChannel> VoiceChannels
            => Channels.OfType<SocketVoiceChannel>().ToImmutableArray();
        /// <summary>
        ///     Gets a collection of all stage channels in this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of stage channels found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketStageChannel> StageChannels
            => Channels.OfType<SocketStageChannel>().ToImmutableArray();
        /// <summary>
        ///     Gets a collection of all category channels in this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of category channels found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketCategoryChannel> CategoryChannels
            => Channels.OfType<SocketCategoryChannel>().ToImmutableArray();
        /// <summary>
        ///     Gets a collection of all thread channels in this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of thread channels found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketThreadChannel> ThreadChannels
            => Channels.OfType<SocketThreadChannel>().ToImmutableArray();
        /// <summary>
        ///     Gets the current logged-in user.
        /// </summary>
        public SocketGuildUser CurrentUser => _members.TryGetValue(Discord.CurrentUser.Id, out SocketGuildUser member) ? member : null;
        /// <summary>
        ///     Gets the built-in role containing all users in this guild.
        /// </summary>
        /// <returns>
        ///     A role object that represents an <c>@everyone</c> role in this guild.
        /// </returns>
        public SocketRole EveryoneRole => GetRole(Id);
        /// <summary>
        ///     Gets a collection of all channels in this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of generic channels found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketGuildChannel> Channels
        {
            get
            {
                var channels = _channels;
                var state = Discord.State;
                return channels.Select(x => x.Value).Where(x => x != null).ToReadOnlyCollection(channels);
            }
        }
        /// <inheritdoc />
        public IReadOnlyCollection<GuildEmote> Emotes => _emotes;
        /// <summary>
        ///     Gets a collection of all custom stickers for this guild.
        /// </summary>
        public IReadOnlyCollection<SocketCustomSticker> Stickers
            => _stickers.Select(x => x.Value).ToImmutableArray();
        /// <summary>
        ///     Gets a collection of users in this guild.
        /// </summary>
        /// <remarks>
        ///     This property retrieves all users found within this guild.
        ///     <note type="warning">
        ///         <para>
        ///             This property may not always return all the members for large guilds (i.e. guilds containing
        ///             100+ users). If you are simply looking to get the number of users present in this guild,
        ///             consider using <see cref="MemberCount"/> instead.
        ///         </para>
        ///         <para>
        ///             Otherwise, you may need to enable <see cref="DiscordSocketConfig.AlwaysDownloadUsers"/> to fetch
        ///             the full user list upon startup, or use <see cref="DownloadUsersAsync"/> to manually download
        ///             the users.
        ///         </para>
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A collection of guild users found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketGuildUser> Users => _members.ToReadOnlyCollection();
        /// <summary>
        ///     Gets a collection of all roles in this guild.
        /// </summary>
        /// <returns>
        ///     A read-only collection of roles found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketRole> Roles => _roles.ToReadOnlyCollection();

        /// <summary>
        ///     Gets a collection of all events within this guild.
        /// </summary>
        /// <remarks>
        ///     This field is based off of caching alone, since there is no events returned on the guild model.
        /// </remarks>
        /// <returns>
        ///     A read-only collection of guild events found within this guild.
        /// </returns>
        public IReadOnlyCollection<SocketGuildEvent> Events => _events.ToReadOnlyCollection();

        internal SocketGuild(DiscordSocketClient client, ulong id)
            : base(client, id)
        {
            _audioLock = new SemaphoreSlim(1, 1);
            _emotes = ImmutableArray.Create<GuildEmote>();
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
                if(_events == null)
                    _events = new ConcurrentDictionary<ulong, SocketGuildEvent>();
                if (_channels == null)
                    _channels = new ConcurrentDictionary<ulong, SocketGuildChannel>();
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

            var channels = new ConcurrentDictionary<ulong, SocketGuildChannel>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.Channels.Length * 1.05));
            {
                for (int i = 0; i < model.Channels.Length; i++)
                {
                    var channel = SocketGuildChannel.Create(this, state, model.Channels[i]);
                    state.AddChannel(channel);
                    channels.TryAdd(channel.Id, channel);
                }

                for(int i = 0; i < model.Threads.Length; i++)
                {
                    var threadChannel = SocketThreadChannel.Create(this, state, model.Threads[i]);
                    state.AddChannel(threadChannel);
                    channels.TryAdd(threadChannel.Id, threadChannel);
                }
            }

            _channels = channels;

            var members = new ConcurrentDictionary<ulong, SocketGuildUser>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.Members.Length * 1.05));
            {
                for (int i = 0; i < model.Members.Length; i++)
                {
                    var member = SocketGuildUser.Create(this, state, model.Members[i]);
                    if (members.TryAdd(member.Id, member))
                        member.GlobalUser.AddRef();
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

            var events = new ConcurrentDictionary<ulong, SocketGuildEvent>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.GuildScheduledEvents.Length * 1.05));
            {
                for (int i = 0; i < model.GuildScheduledEvents.Length; i++)
                {
                    var guildEvent = SocketGuildEvent.Create(Discord, this, model.GuildScheduledEvents[i]);
                    events.TryAdd(guildEvent.Id, guildEvent);
                }
            }
            _events = events;


            _syncPromise = new TaskCompletionSource<bool>();
            _downloaderPromise = new TaskCompletionSource<bool>();
            var _ = _syncPromise.TrySetResultAsync(true);
            /*if (!model.Large)
                _ = _downloaderPromise.TrySetResultAsync(true);*/
        }
        internal void Update(ClientState state, Model model)
        {
            AFKChannelId = model.AFKChannelId;
            if (model.WidgetChannelId.IsSpecified)
                WidgetChannelId = model.WidgetChannelId.Value;
            SystemChannelId = model.SystemChannelId;
            RulesChannelId = model.RulesChannelId;
            PublicUpdatesChannelId = model.PublicUpdatesChannelId;
            AFKTimeout = model.AFKTimeout;
            if (model.WidgetEnabled.IsSpecified)
                IsWidgetEnabled = model.WidgetEnabled.Value;
            IconId = model.Icon;
            Name = model.Name;
            OwnerId = model.OwnerId;
            VoiceRegionId = model.Region;
            SplashId = model.Splash;
            DiscoverySplashId = model.DiscoverySplash;
            VerificationLevel = model.VerificationLevel;
            MfaLevel = model.MfaLevel;
            DefaultMessageNotifications = model.DefaultMessageNotifications;
            ExplicitContentFilter = model.ExplicitContentFilter;
            ApplicationId = model.ApplicationId;
            PremiumTier = model.PremiumTier;
            VanityURLCode = model.VanityURLCode;
            BannerId = model.Banner;
            SystemChannelFlags = model.SystemChannelFlags;
            Description = model.Description;
            PremiumSubscriptionCount = model.PremiumSubscriptionCount.GetValueOrDefault();
            NsfwLevel = model.NsfwLevel;
            if (model.MaxPresences.IsSpecified)
                MaxPresences = model.MaxPresences.Value ?? 25000;
            if (model.MaxMembers.IsSpecified)
                MaxMembers = model.MaxMembers.Value;
            if (model.MaxVideoChannelUsers.IsSpecified)
                MaxVideoChannelUsers = model.MaxVideoChannelUsers.Value;
            PreferredLocale = model.PreferredLocale;
            PreferredCulture = PreferredLocale == null ? null : new CultureInfo(PreferredLocale);
            if (model.IsBoostProgressBarEnabled.IsSpecified)
                IsBoostProgressBarEnabled = model.IsBoostProgressBarEnabled.Value;
            if (model.Emojis != null)
            {
                var emojis = ImmutableArray.CreateBuilder<GuildEmote>(model.Emojis.Length);
                for (int i = 0; i < model.Emojis.Length; i++)
                    emojis.Add(model.Emojis[i].ToEntity());
                _emotes = emojis.ToImmutable();
            }
            else
                _emotes = ImmutableArray.Create<GuildEmote>();

            Features = model.Features;

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

            if (model.Stickers != null)
            {
                var stickers = new ConcurrentDictionary<ulong, SocketCustomSticker>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(model.Stickers.Length * 1.05));
                for (int i = 0; i < model.Stickers.Length; i++)
                {
                    var sticker = model.Stickers[i];
                    if (sticker.User.IsSpecified)
                        AddOrUpdateUser(sticker.User.Value);

                    var entity = SocketCustomSticker.Create(Discord, sticker, this, sticker.User.IsSpecified ? sticker.User.Value.Id : null);

                    stickers.TryAdd(sticker.Id, entity);
                }

                _stickers = stickers;
            }
            else
                _stickers = new ConcurrentDictionary<ulong, SocketCustomSticker>(ConcurrentHashSet.DefaultConcurrencyLevel, 7);
        }
        /*internal void Update(ClientState state, GuildSyncModel model) //TODO remove? userbot related
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
            //if (!model.Large)
            //    _ = _downloaderPromise.TrySetResultAsync(true);
        }*/

        internal void Update(ClientState state, EmojiUpdateModel model)
        {
            var emotes = ImmutableArray.CreateBuilder<GuildEmote>(model.Emojis.Length);
            for (int i = 0; i < model.Emojis.Length; i++)
                emotes.Add(model.Emojis[i].ToEntity());
            _emotes = emotes.ToImmutable();
        }
        #endregion

        #region General
        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => GuildHelper.DeleteAsync(this, Discord, options);

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        public Task ModifyAsync(Action<GuildProperties> func, RequestOptions options = null)
            => GuildHelper.ModifyAsync(this, Discord, func, options);

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        public Task ModifyWidgetAsync(Action<GuildWidgetProperties> func, RequestOptions options = null)
            => GuildHelper.ModifyWidgetAsync(this, Discord, func, options);
        /// <inheritdoc />
        public Task ReorderChannelsAsync(IEnumerable<ReorderChannelProperties> args, RequestOptions options = null)
            => GuildHelper.ReorderChannelsAsync(this, Discord, args, options);
        /// <inheritdoc />
        public Task ReorderRolesAsync(IEnumerable<ReorderRoleProperties> args, RequestOptions options = null)
            => GuildHelper.ReorderRolesAsync(this, Discord, args, options);

        /// <inheritdoc />
        public Task LeaveAsync(RequestOptions options = null)
            => GuildHelper.LeaveAsync(this, Discord, options);
        #endregion

        #region Bans

        /// <inheritdoc cref="IGuild.GetBansAsync(int, RequestOptions)" />
        public IAsyncEnumerable<IReadOnlyCollection<RestBan>> GetBansAsync(int limit = DiscordConfig.MaxBansPerBatch, RequestOptions options = null)
            => GuildHelper.GetBansAsync(this, Discord, null, Direction.Before, limit, options);

        /// <inheritdoc cref="IGuild.GetBansAsync(ulong, Direction, int, RequestOptions)" />
        public IAsyncEnumerable<IReadOnlyCollection<RestBan>> GetBansAsync(ulong fromUserId, Direction dir, int limit = DiscordConfig.MaxBansPerBatch, RequestOptions options = null)
            => GuildHelper.GetBansAsync(this, Discord, fromUserId, dir, limit, options);

        /// <inheritdoc cref="IGuild.GetBansAsync(IUser, Direction, int, RequestOptions)" />
        public IAsyncEnumerable<IReadOnlyCollection<RestBan>> GetBansAsync(IUser fromUser, Direction dir, int limit = DiscordConfig.MaxBansPerBatch, RequestOptions options = null)
            => GuildHelper.GetBansAsync(this, Discord, fromUser.Id, dir, limit, options);

        /// <summary>
        ///     Gets a ban object for a banned user.
        /// </summary>
        /// <param name="user">The banned user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
        ///     contains the user information and the reason for the ban; <see langword="null"/> if the ban entry cannot be found.
        /// </returns>
        public Task<RestBan> GetBanAsync(IUser user, RequestOptions options = null)
            => GuildHelper.GetBanAsync(this, Discord, user.Id, options);
        /// <summary>
        ///     Gets a ban object for a banned user.
        /// </summary>
        /// <param name="userId">The snowflake identifier for the banned user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
        ///     contains the user information and the reason for the ban; <see langword="null"/> if the ban entry cannot be found.
        /// </returns>
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
        #endregion

        #region Channels
        /// <summary>
        ///     Gets a channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the channel.</param>
        /// <returns>
        ///     A generic channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
        /// </returns>
        public SocketGuildChannel GetChannel(ulong id)
        {
            var channel = Discord.State.GetChannel(id) as SocketGuildChannel;
            if (channel?.Guild.Id == Id)
                return channel;
            return null;
        }
        /// <summary>
        ///     Gets a text channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the text channel.</param>
        /// <returns>
        ///     A text channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
        /// </returns>
        public SocketTextChannel GetTextChannel(ulong id)
            => GetChannel(id) as SocketTextChannel;
        /// <summary>
        ///     Gets a thread in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the thread.</param>
        /// <returns>
        ///     A thread channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
        /// </returns>
        public SocketThreadChannel GetThreadChannel(ulong id)
            => GetChannel(id) as SocketThreadChannel;
        /// <summary>
        ///     Gets a forum channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the forum channel.</param>
        /// <returns>
        ///     A forum channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
        /// </returns>
        public SocketForumChannel GetForumChannel(ulong id)
            => GetChannel(id) as SocketForumChannel;
        /// <summary>
        ///     Gets a voice channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the voice channel.</param>
        /// <returns>
        ///     A voice channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
        /// </returns>
        public SocketVoiceChannel GetVoiceChannel(ulong id)
            => GetChannel(id) as SocketVoiceChannel;
        /// <summary>
        ///     Gets a stage channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the stage channel.</param>
        /// <returns>
        ///     A stage channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
        /// </returns>
        public SocketStageChannel GetStageChannel(ulong id)
            => GetChannel(id) as SocketStageChannel;
        /// <summary>
        ///     Gets a category channel in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the category channel.</param>
        /// <returns>
        ///     A category channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
        /// </returns>
        public SocketCategoryChannel GetCategoryChannel(ulong id)
            => GetChannel(id) as SocketCategoryChannel;

        /// <summary>
        ///     Creates a new text channel in this guild.
        /// </summary>
        /// <example>
        ///     The following example creates a new text channel under an existing category named <c>Wumpus</c> with a set topic.
        ///     <code language="cs">
        ///     var categories = await guild.GetCategoriesAsync();
        ///     var targetCategory = categories.FirstOrDefault(x => x.Name == "wumpus");
        ///     if (targetCategory == null) return;
        ///     await Context.Guild.CreateTextChannelAsync(name, x =>
        ///     {
        ///         x.CategoryId = targetCategory.Id;
        ///         x.Topic = $"This channel was created at {DateTimeOffset.UtcNow} by {user}.";
        ///     });
        ///     </code>
        /// </example>
        /// <param name="name">The new name for the text channel.</param>
        /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     text channel.
        /// </returns>
        public Task<RestTextChannel> CreateTextChannelAsync(string name, Action<TextChannelProperties> func = null, RequestOptions options = null)
            => GuildHelper.CreateTextChannelAsync(this, Discord, name, options, func);
        /// <summary>
        ///     Creates a new voice channel in this guild.
        /// </summary>
        /// <param name="name">The new name for the voice channel.</param>
        /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     voice channel.
        /// </returns>
        public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func = null, RequestOptions options = null)
            => GuildHelper.CreateVoiceChannelAsync(this, Discord, name, options, func);

        /// <summary>
        ///     Creates a new stage channel in this guild.
        /// </summary>
        /// <param name="name">The new name for the stage channel.</param>
        /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     stage channel.
        /// </returns>
        public Task<RestStageChannel> CreateStageChannelAsync(string name, Action<VoiceChannelProperties> func = null, RequestOptions options = null)
            => GuildHelper.CreateStageChannelAsync(this, Discord, name, options, func);
        /// <summary>
        ///     Creates a new channel category in this guild.
        /// </summary>
        /// <param name="name">The new name for the category.</param>
        /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     category channel.
        /// </returns>
        public Task<RestCategoryChannel> CreateCategoryChannelAsync(string name, Action<GuildChannelProperties> func = null, RequestOptions options = null)
            => GuildHelper.CreateCategoryChannelAsync(this, Discord, name, options, func);

        internal SocketGuildChannel AddChannel(ClientState state, ChannelModel model)
        {
            var channel = SocketGuildChannel.Create(this, state, model);
            _channels.TryAdd(model.Id, channel);
            state.AddChannel(channel);
            return channel;
        }

        internal SocketGuildChannel AddOrUpdateChannel(ClientState state, ChannelModel model)
        {
            if (_channels.TryGetValue(model.Id, out SocketGuildChannel channel))
                channel.Update(Discord.State, model);
            else
            {
                channel = SocketGuildChannel.Create(this, Discord.State, model);
                _channels[channel.Id] = channel;
                state.AddChannel(channel);
            }
            return channel;
        }

        internal SocketGuildChannel RemoveChannel(ClientState state, ulong id)
        {
            if (_channels.TryRemove(id, out var _))
                return state.RemoveChannel(id) as SocketGuildChannel;
            return null;
        }
        internal void PurgeChannelCache(ClientState state)
        {
            foreach (var channelId in _channels)
                state.RemoveChannel(channelId.Key);

            _channels.Clear();
        }
        #endregion

        #region Voice Regions
        /// <summary>
        ///     Gets a collection of all the voice regions this guild can access.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     voice regions the guild can access.
        /// </returns>
        public Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null)
            => GuildHelper.GetVoiceRegionsAsync(this, Discord, options);
        #endregion

        #region Integrations
        public Task<IReadOnlyCollection<RestIntegration>> GetIntegrationsAsync(RequestOptions options = null)
            => GuildHelper.GetIntegrationsAsync(this, Discord, options);
        public Task DeleteIntegrationAsync(ulong id, RequestOptions options = null)
            => GuildHelper.DeleteIntegrationAsync(this, Discord, id, options);
        #endregion

        #region Interactions
        /// <summary>
        ///     Deletes all application commands in the current guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous delete operation.
        /// </returns>
        public Task DeleteApplicationCommandsAsync(RequestOptions options = null)
            => InteractionHelper.DeleteAllGuildCommandsAsync(Discord, Id, options);

        /// <summary>
        ///     Gets a collection of slash commands created by the current user in this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     slash commands created by the current user.
        /// </returns>
        public async Task<IReadOnlyCollection<SocketApplicationCommand>> GetApplicationCommandsAsync(RequestOptions options = null)
        {
            var commands = (await Discord.ApiClient.GetGuildApplicationCommandsAsync(Id, options)).Select(x => SocketApplicationCommand.Create(Discord, x, Id));

            foreach (var command in commands)
            {
                Discord.State.AddCommand(command);
            }

            return commands.ToImmutableArray();
        }

        /// <summary>
        ///     Gets an application command within this guild with the specified id.
        /// </summary>
        /// <param name="id">The id of the application command to get.</param>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A ValueTask that represents the asynchronous get operation. The task result contains a <see cref="IApplicationCommand"/>
        ///     if found, otherwise <see langword="null"/>.
        /// </returns>
        public async ValueTask<SocketApplicationCommand> GetApplicationCommandAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            var command = Discord.State.GetCommand(id);

            if (command != null)
                return command;

            if (mode == CacheMode.CacheOnly)
                return null;

            var model = await Discord.ApiClient.GetGlobalApplicationCommandAsync(id, options);

            if (model == null)
                return null;

            command = SocketApplicationCommand.Create(Discord, model, Id);

            Discord.State.AddCommand(command);

            return command;
        }

        /// <summary>
        ///     Creates an application command within this guild.
        /// </summary>
        /// <param name="properties">The properties to use when creating the command.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the command that was created.
        /// </returns>
        public async Task<SocketApplicationCommand> CreateApplicationCommandAsync(ApplicationCommandProperties properties, RequestOptions options = null)
        {
            var model = await InteractionHelper.CreateGuildCommandAsync(Discord, Id, properties, options);

            var entity = Discord.State.GetOrAddCommand(model.Id, (id) => SocketApplicationCommand.Create(Discord, model));

            entity.Update(model);

            return entity;
        }

        /// <summary>
        ///     Overwrites the application commands within this guild.
        /// </summary>
        /// <param name="properties">A collection of properties to use when creating the commands.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains a collection of commands that was created.
        /// </returns>
        public async Task<IReadOnlyCollection<SocketApplicationCommand>> BulkOverwriteApplicationCommandAsync(ApplicationCommandProperties[] properties,
            RequestOptions options = null)
        {
            var models = await InteractionHelper.BulkOverwriteGuildCommandsAsync(Discord, Id, properties, options);

            var entities = models.Select(x => SocketApplicationCommand.Create(Discord, x));

            Discord.State.PurgeCommands(x => !x.IsGlobalCommand && x.Guild.Id == Id);

            foreach(var entity in entities)
            {
                Discord.State.AddCommand(entity);
            }

            return entities.ToImmutableArray();
        }
        #endregion

        #region Invites
        /// <summary>
        ///     Gets a collection of all invites in this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
        ///     invite metadata, each representing information for an invite found within this guild.
        /// </returns>
        public Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => GuildHelper.GetInvitesAsync(this, Discord, options);
        /// <summary>
        ///     Gets the vanity invite URL of this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the partial metadata of
        ///     the vanity invite found within this guild; <see langword="null"/> if none is found.
        /// </returns>
        public Task<RestInviteMetadata> GetVanityInviteAsync(RequestOptions options = null)
            => GuildHelper.GetVanityInviteAsync(this, Discord, options);
        #endregion

        #region Roles
        /// <summary>
        ///     Gets a role in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the role.</param>
        /// <returns>
        ///     A role that is associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
        /// </returns>
        public SocketRole GetRole(ulong id)
        {
            if (_roles.TryGetValue(id, out SocketRole value))
                return value;
            return null;
        }

        /// <summary>
        ///     Creates a new role with the provided name.
        /// </summary>
        /// <param name="name">The new name for the role.</param>
        /// <param name="permissions">The guild permission that the role should possess.</param>
        /// <param name="color">The color of the role.</param>
        /// <param name="isHoisted">Whether the role is separated from others on the sidebar.</param>
        /// <param name="isMentionable">Whether the role can be mentioned.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     role.
        /// </returns>
        public Task<RestRole> CreateRoleAsync(string name, GuildPermissions? permissions = default(GuildPermissions?), Color? color = default(Color?),
            bool isHoisted = false, bool isMentionable = false, RequestOptions options = null)
            => GuildHelper.CreateRoleAsync(this, Discord, name, permissions, color, isHoisted, isMentionable, options);
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

        internal SocketRole AddOrUpdateRole(RoleModel model)
        {
            if (_roles.TryGetValue(model.Id, out SocketRole role))
                _roles[model.Id].Update(Discord.State, model);
            else
                role = AddRole(model);

            return role;
        }

        internal SocketCustomSticker AddSticker(StickerModel model)
        {
            if (model.User.IsSpecified)
                AddOrUpdateUser(model.User.Value);

            var sticker = SocketCustomSticker.Create(Discord, model, this, model.User.IsSpecified ? model.User.Value.Id : null);
            _stickers[model.Id] = sticker;
            return sticker;
        }

        internal SocketCustomSticker AddOrUpdateSticker(StickerModel model)
        {
            if (_stickers.TryGetValue(model.Id, out SocketCustomSticker sticker))
                _stickers[model.Id].Update(model);
            else
                sticker = AddSticker(model);

            return sticker;
        }

        internal SocketCustomSticker RemoveSticker(ulong id)
        {
            if (_stickers.TryRemove(id, out SocketCustomSticker sticker))
                return sticker;
            return null;
        }
        #endregion

        #region Users
        /// <inheritdoc />
        public Task<RestGuildUser> AddGuildUserAsync(ulong id, string accessToken, Action<AddGuildUserProperties> func = null, RequestOptions options = null)
            => GuildHelper.AddGuildUserAsync(this, Discord, id, accessToken, func, options);

        /// <summary>
        ///     Gets a user from this guild.
        /// </summary>
        /// <remarks>
        ///     This method retrieves a user found within this guild.
        ///     <note>
        ///         This may return <see langword="null"/> in the WebSocket implementation due to incomplete user collection in
        ///         large guilds.
        ///     </note>
        /// </remarks>
        /// <param name="id">The snowflake identifier of the user.</param>
        /// <returns>
        ///     A guild user associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
        /// </returns>
        public SocketGuildUser GetUser(ulong id)
        {
            if (_members.TryGetValue(id, out SocketGuildUser member))
                return member;
            return null;
        }
        /// <inheritdoc />
        public Task<int> PruneUsersAsync(int days = 30, bool simulate = false, RequestOptions options = null, IEnumerable<ulong> includeRoleIds = null)
            => GuildHelper.PruneUsersAsync(this, Discord, days, simulate, options, includeRoleIds);

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
        ///     Purges this guild's user cache.
        /// </summary>
        public void PurgeUserCache() => PurgeUserCache(_ => true);
        /// <summary>
        ///     Purges this guild's user cache.
        /// </summary>
        /// <param name="predicate">The predicate used to select which users to clear.</param>
        public void PurgeUserCache(Func<SocketGuildUser, bool> predicate)
        {
            var membersToPurge = Users.Where(x => predicate.Invoke(x) && x?.Id != Discord.CurrentUser.Id);
            var membersToKeep = Users.Where(x => !predicate.Invoke(x) || x?.Id == Discord.CurrentUser.Id);

            foreach (var member in membersToPurge)
                if(_members.TryRemove(member.Id, out _))
                    member.GlobalUser.RemoveRef(Discord);

            foreach (var member in membersToKeep)
                _members.TryAdd(member.Id, member);

            _downloaderPromise = new TaskCompletionSource<bool>();
            DownloadedMemberCount = _members.Count;
        }

        /// <summary>
        ///     Gets a collection of all users in this guild.
        /// </summary>
        /// <remarks>
        ///     <para>This method retrieves all users found within this guild throught REST.</para>
        ///     <para>Users returned by this method are not cached.</para>
        /// </remarks>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a collection of guild
        ///     users found within this guild.
        /// </returns>
        public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(RequestOptions options = null)
        {
            if (HasAllMembers)
                return ImmutableArray.Create(Users).ToAsyncEnumerable<IReadOnlyCollection<IGuildUser>>();
            return GuildHelper.GetUsersAsync(this, Discord, null, null, options);
        }

        /// <inheritdoc />
        public async Task DownloadUsersAsync()
        {
            await Discord.DownloadUsersAsync(new[] { this }).ConfigureAwait(false);
        }
        internal void CompleteDownloadUsers()
        {
            _downloaderPromise.TrySetResultAsync(true);
        }

        /// <summary>
        ///     Gets a collection of users in this guild that the name or nickname starts with the
        ///     provided <see cref="string"/> at <paramref name="query"/>.
        /// </summary>
        /// <remarks>
        ///     The <paramref name="limit"/> can not be higher than <see cref="DiscordConfig.MaxUsersPerBatch"/>.
        /// </remarks>
        /// <param name="query">The partial name or nickname to search.</param>
        /// <param name="limit">The maximum number of users to be gotten.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a collection of guild
        ///     users that the name or nickname starts with the provided <see cref="string"/> at <paramref name="query"/>.
        /// </returns>
        public Task<IReadOnlyCollection<RestGuildUser>> SearchUsersAsync(string query, int limit = DiscordConfig.MaxUsersPerBatch, RequestOptions options = null)
            => GuildHelper.SearchUsersAsync(this, Discord, query, limit, options);
        #endregion

        #region Guild Events

        /// <summary>
        ///     Gets an event in this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the event.</param>
        /// <returns>
        ///     An event that is associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
        /// </returns>
        public SocketGuildEvent GetEvent(ulong id)
        {
            if (_events.TryGetValue(id, out SocketGuildEvent value))
                return value;
            return null;
        }

        internal SocketGuildEvent RemoveEvent(ulong id)
        {
            if (_events.TryRemove(id, out SocketGuildEvent value))
                return value;
            return null;
        }

        internal SocketGuildEvent AddOrUpdateEvent(EventModel model)
        {
            if (_events.TryGetValue(model.Id, out SocketGuildEvent value))
                value.Update(model);
            else
            {
                value = SocketGuildEvent.Create(Discord, this, model);
                _events[model.Id] = value;
            }
            return value;
        }

        /// <summary>
        ///     Gets an event within this guild.
        /// </summary>
        /// <param name="id">The snowflake identifier for the event.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation.
        /// </returns>
        public Task<RestGuildEvent> GetEventAsync(ulong id, RequestOptions options = null)
            => GuildHelper.GetGuildEventAsync(Discord, id, this, options);

        /// <summary>
        ///     Gets all active events within this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation.
        /// </returns>
        public Task<IReadOnlyCollection<RestGuildEvent>> GetEventsAsync(RequestOptions options = null)
            => GuildHelper.GetGuildEventsAsync(Discord, this, options);

        /// <summary>
        ///     Creates an event within this guild.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="privacyLevel">The privacy level of the event.</param>
        /// <param name="startTime">The start time of the event.</param>
        /// <param name="type">The type of the event.</param>
        /// <param name="description">The description of the event.</param>
        /// <param name="endTime">The end time of the event.</param>
        /// <param name="channelId">
        ///     The channel id of the event.
        ///     <remarks>
        ///     The event must have a type of <see cref="GuildScheduledEventType.Stage"/> or <see cref="GuildScheduledEventType.Voice"/>
        ///     in order to use this property.
        ///     </remarks>
        /// </param>
        /// <param name="location">The location of the event; links are supported</param>
        /// <param name="coverImage">The optional banner image for the event.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous create operation.
        /// </returns>
        public Task<RestGuildEvent> CreateEventAsync(
            string name,
            DateTimeOffset startTime,
            GuildScheduledEventType type,
            GuildScheduledEventPrivacyLevel privacyLevel = GuildScheduledEventPrivacyLevel.Private,
            string description = null,
            DateTimeOffset? endTime = null,
            ulong? channelId = null,
            string location = null,
            Image? coverImage = null,
            RequestOptions options = null)
        {
            // requirements taken from https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-permissions-requirements
            switch (type)
            {
                case GuildScheduledEventType.Stage:
                    CurrentUser.GuildPermissions.Ensure(GuildPermission.ManageEvents | GuildPermission.ManageChannels | GuildPermission.MuteMembers | GuildPermission.MoveMembers);
                break;
                case GuildScheduledEventType.Voice:
                    CurrentUser.GuildPermissions.Ensure(GuildPermission.ManageEvents | GuildPermission.ViewChannel | GuildPermission.Connect);
                    break;
                case GuildScheduledEventType.External:
                    CurrentUser.GuildPermissions.Ensure(GuildPermission.ManageEvents);
                    break;
            }

            return GuildHelper.CreateGuildEventAsync(Discord, this, name, privacyLevel, startTime, type, description, endTime, channelId, location, coverImage, options);
        }


        #endregion

        #region Audit logs
        /// <summary>
        ///     Gets the specified number of audit log entries for this guild.
        /// </summary>
        /// <param name="limit">The number of audit log entries to fetch.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="beforeId">The audit log entry ID to filter entries before.</param>
        /// <param name="actionType">The type of actions to filter.</param>
        /// <param name="userId">The user ID to filter entries for.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of the requested audit log entries.
        /// </returns>
        public IAsyncEnumerable<IReadOnlyCollection<RestAuditLogEntry>> GetAuditLogsAsync(int limit, RequestOptions options = null, ulong? beforeId = null, ulong? userId = null, ActionType? actionType = null)
            => GuildHelper.GetAuditLogsAsync(this, Discord, beforeId, limit, options, userId: userId, actionType: actionType);
        #endregion

        #region Webhooks
        /// <summary>
        ///     Gets a webhook found within this guild.
        /// </summary>
        /// <param name="id">The identifier for the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the webhook with the
        ///     specified <paramref name="id"/>; <see langword="null"/> if none is found.
        /// </returns>
        public Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => GuildHelper.GetWebhookAsync(this, Discord, id, options);
        /// <summary>
        ///     Gets a collection of all webhook from this guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of webhooks found within the guild.
        /// </returns>
        public Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
            => GuildHelper.GetWebhooksAsync(this, Discord, options);
        #endregion

        #region Emotes
        /// <inheritdoc />
        public Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions options = null)
            => GuildHelper.GetEmotesAsync(this, Discord, options);
        /// <inheritdoc />
        public Task<GuildEmote> GetEmoteAsync(ulong id, RequestOptions options = null)
            => GuildHelper.GetEmoteAsync(this, Discord, id, options);
        /// <inheritdoc />
        public Task<GuildEmote> CreateEmoteAsync(string name, Image image, Optional<IEnumerable<IRole>> roles = default(Optional<IEnumerable<IRole>>), RequestOptions options = null)
            => GuildHelper.CreateEmoteAsync(this, Discord, name, image, roles, options);
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        public Task<GuildEmote> ModifyEmoteAsync(GuildEmote emote, Action<EmoteProperties> func, RequestOptions options = null)
            => GuildHelper.ModifyEmoteAsync(this, Discord, emote.Id, func, options);
        /// <inheritdoc />
        public Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null)
            => GuildHelper.DeleteEmoteAsync(this, Discord, emote.Id, options);

        /// <summary>
        /// Moves the user to the voice channel.
        /// </summary>
        /// <param name="user">The user to move.</param>
        /// <param name="targetChannel">the channel where the user gets moved to.</param>
        /// <returns>A task that represents the asynchronous operation for moving a user.</returns>
        public Task MoveAsync(IGuildUser user, IVoiceChannel targetChannel)
            => user.ModifyAsync(x => x.Channel = new Optional<IVoiceChannel>(targetChannel));

        /// <summary>
        /// Disconnects the user from its current voice channel
        /// </summary>
        /// <param name="user">The user to disconnect.</param>
        /// <returns>A task that represents the asynchronous operation for disconnecting a user.</returns>
        async Task IGuild.DisconnectAsync(IGuildUser user) => await user.ModifyAsync(x => x.Channel = new Optional<IVoiceChannel>());
        #endregion

        #region Stickers
        /// <summary>
        ///     Gets a specific sticker within this guild.
        /// </summary>
        /// <param name="id">The id of the sticker to get.</param>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the sticker found with the
        ///     specified <paramref name="id"/>; <see langword="null" /> if none is found.
        /// </returns>
        public async ValueTask<SocketCustomSticker> GetStickerAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            var sticker = _stickers?.FirstOrDefault(x => x.Key == id);

            if (sticker?.Value != null)
                return sticker?.Value;

            if (mode == CacheMode.CacheOnly)
                return null;

            var model = await Discord.ApiClient.GetGuildStickerAsync(Id, id, options).ConfigureAwait(false);

            if (model == null)
                return null;

            return AddOrUpdateSticker(model);
        }
        /// <summary>
        ///     Gets a specific sticker within this guild.
        /// </summary>
        /// <param name="id">The id of the sticker to get.</param>
        /// <returns>A sticker, if none is found then <see langword="null"/>.</returns>
        public SocketCustomSticker GetSticker(ulong id)
            => GetStickerAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult();
        /// <summary>
        ///     Gets a collection of all stickers within this guild.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of stickers found within the guild.
        /// </returns>
        public async ValueTask<IReadOnlyCollection<SocketCustomSticker>> GetStickersAsync(CacheMode mode = CacheMode.AllowDownload,
            RequestOptions options = null)
        {
            if (Stickers.Count > 0)
                return Stickers;

            if (mode == CacheMode.CacheOnly)
                return ImmutableArray.Create<SocketCustomSticker>();

            var models = await Discord.ApiClient.ListGuildStickersAsync(Id, options).ConfigureAwait(false);

            List<SocketCustomSticker> stickers = new();

            foreach (var model in models)
            {
                stickers.Add(AddOrUpdateSticker(model));
            }

            return stickers;
        }
        /// <summary>
        ///     Creates a new sticker in this guild.
        /// </summary>
        /// <param name="name">The name of the sticker.</param>
        /// <param name="description">The description of the sticker.</param>
        /// <param name="tags">The tags of the sticker.</param>
        /// <param name="image">The image of the new emote.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the created sticker.
        /// </returns>
        public async Task<SocketCustomSticker> CreateStickerAsync(string name, string description, IEnumerable<string> tags, Image image,
            RequestOptions options = null)
        {
            var model = await GuildHelper.CreateStickerAsync(Discord, this, name, description, tags, image, options).ConfigureAwait(false);

            return AddOrUpdateSticker(model);
        }
        /// <summary>
        ///     Creates a new sticker in this guild
        /// </summary>
        /// <param name="name">The name of the sticker.</param>
        /// <param name="description">The description of the sticker.</param>
        /// <param name="tags">The tags of the sticker.</param>
        /// <param name="path">The path of the file to upload.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the created sticker.
        /// </returns>
        public Task<SocketCustomSticker> CreateStickerAsync(string name, string description, IEnumerable<string> tags, string path,
            RequestOptions options = null)
        {
            var fs = File.OpenRead(path);
            return CreateStickerAsync(name, description, tags, fs, Path.GetFileName(fs.Name), options);
        }
        /// <summary>
        ///     Creates a new sticker in this guild
        /// </summary>
        /// <param name="name">The name of the sticker.</param>
        /// <param name="description">The description of the sticker.</param>
        /// <param name="tags">The tags of the sticker.</param>
        /// <param name="stream">The stream containing the file data.</param>
        /// <param name="filename">The name of the file <b>with</b> the extension, ex: image.png.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the created sticker.
        /// </returns>
        public async Task<SocketCustomSticker> CreateStickerAsync(string name, string description, IEnumerable<string> tags, Stream stream,
            string filename, RequestOptions options = null)
        {
            var model = await GuildHelper.CreateStickerAsync(Discord, this, name, description, tags, stream, filename, options).ConfigureAwait(false);

            return AddOrUpdateSticker(model);
        }
        /// <summary>
        ///     Deletes a sticker within this guild.
        /// </summary>
        /// <param name="sticker">The sticker to delete.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation.
        /// </returns>
        public Task DeleteStickerAsync(SocketCustomSticker sticker, RequestOptions options = null)
            => sticker.DeleteAsync(options);
        #endregion

        #region Voice States
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
        #endregion

        #region Audio
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

                _voiceStateUpdateParams = new VoiceStateUpdateParams
                {
                    GuildId = Id,
                    ChannelId = channelId,
                    SelfDeaf = selfDeaf,
                    SelfMute = selfMute
                };

                if (external)
                {
#pragma warning disable IDISP001
                    var _ = promise.TrySetResultAsync(null);
                    await Discord.ApiClient.SendVoiceStateUpdateAsync(_voiceStateUpdateParams).ConfigureAwait(false);
                    return null;
#pragma warning restore IDISP001
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
#pragma warning disable IDISP001
                        var _ = promise.TrySetResultAsync(_audioClient);
#pragma warning restore IDISP001
                        return Task.Delay(0);
                    };
#pragma warning disable IDISP003
                    _audioClient = audioClient;
#pragma warning restore IDISP003
                }

                await Discord.ApiClient.SendVoiceStateUpdateAsync(_voiceStateUpdateParams).ConfigureAwait(false);
            }
            catch
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
            catch
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
            _audioClient?.Dispose();
            _audioClient = null;
            _voiceStateUpdateParams = null;
        }

        internal async Task ModifyAudioAsync(ulong channelId, Action<AudioChannelProperties> func, RequestOptions options)
        {
            await _audioLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ModifyAudioInternalAsync(channelId, func, options).ConfigureAwait(false);
            }
            finally
            {
                _audioLock.Release();
            }
        }

        private async Task ModifyAudioInternalAsync(ulong channelId, Action<AudioChannelProperties> func, RequestOptions options)
        {
            if (_voiceStateUpdateParams == null || _voiceStateUpdateParams.ChannelId != channelId)
                throw new InvalidOperationException("Cannot modify properties of not connected audio channel");

            var props = new AudioChannelProperties();
            func(props);

            if (props.SelfDeaf.IsSpecified)
                _voiceStateUpdateParams.SelfDeaf = props.SelfDeaf.Value;
            if (props.SelfMute.IsSpecified)
                _voiceStateUpdateParams.SelfMute = props.SelfMute.Value;

            await Discord.ApiClient.SendVoiceStateUpdateAsync(_voiceStateUpdateParams, options).ConfigureAwait(false);
        }

        internal async Task FinishConnectAudio(string url, string token)
        {
            //TODO: Mem Leak: Disconnected/Connected handlers aren't cleaned up
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
        /// <returns>
        ///     A string that resolves to <see cref="Discord.WebSocket.SocketGuild.Name"/>.
        /// </returns>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
        internal SocketGuild Clone() => MemberwiseClone() as SocketGuild;
        #endregion

        #region  IGuild
        /// <inheritdoc />
        ulong? IGuild.AFKChannelId => AFKChannelId;
        /// <inheritdoc />
        IAudioClient IGuild.AudioClient => AudioClient;
        /// <inheritdoc />
        bool IGuild.Available => true;
        /// <inheritdoc />
        ulong? IGuild.WidgetChannelId => WidgetChannelId;
        /// <inheritdoc />
        ulong? IGuild.SystemChannelId => SystemChannelId;
        /// <inheritdoc />
        ulong? IGuild.RulesChannelId => RulesChannelId;
        /// <inheritdoc />
        ulong? IGuild.PublicUpdatesChannelId => PublicUpdatesChannelId;
        /// <inheritdoc />
        IRole IGuild.EveryoneRole => EveryoneRole;
        /// <inheritdoc />
        IReadOnlyCollection<IRole> IGuild.Roles => Roles;
        /// <inheritdoc />
        int? IGuild.ApproximateMemberCount => null;
        /// <inheritdoc />
        int? IGuild.ApproximatePresenceCount => null;
        /// <inheritdoc />
        IReadOnlyCollection<ICustomSticker> IGuild.Stickers => Stickers;
        /// <inheritdoc />
        async Task<IGuildScheduledEvent> IGuild.CreateEventAsync(string name, DateTimeOffset startTime, GuildScheduledEventType type, GuildScheduledEventPrivacyLevel privacyLevel, string description, DateTimeOffset? endTime, ulong? channelId, string location, Image? coverImage, RequestOptions options)
            => await CreateEventAsync(name, startTime, type, privacyLevel, description, endTime, channelId, location, coverImage, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IGuildScheduledEvent> IGuild.GetEventAsync(ulong id, RequestOptions options)
            => await GetEventAsync(id, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IGuildScheduledEvent>> IGuild.GetEventsAsync(RequestOptions options)
            => await GetEventsAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(int limit, RequestOptions options)
            => GetBansAsync(limit, options);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(ulong fromUserId, Direction dir, int limit, RequestOptions options)
            => GetBansAsync(fromUserId, dir, limit, options);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(IUser fromUser, Direction dir, int limit, RequestOptions options)
            => GetBansAsync(fromUser, dir, limit, options);
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
        Task<IThreadChannel> IGuild.GetThreadChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IThreadChannel>(GetThreadChannel(id));
        /// <inheritdoc />
        Task<IReadOnlyCollection<IThreadChannel>> IGuild.GetThreadChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IThreadChannel>>(ThreadChannels);
        /// <inheritdoc />
        Task<IReadOnlyCollection<IVoiceChannel>> IGuild.GetVoiceChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IVoiceChannel>>(VoiceChannels);
        /// <inheritdoc />
        Task<IReadOnlyCollection<ICategoryChannel>> IGuild.GetCategoriesAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<ICategoryChannel>>(CategoryChannels);
        /// <inheritdoc />
        Task<IVoiceChannel> IGuild.GetVoiceChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IVoiceChannel>(GetVoiceChannel(id));
        /// <inheritdoc />
        Task<IStageChannel> IGuild.GetStageChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IStageChannel>(GetStageChannel(id));
        /// <inheritdoc />
        Task<IReadOnlyCollection<IStageChannel>> IGuild.GetStageChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IStageChannel>>(StageChannels);
        /// <inheritdoc />
        Task<IVoiceChannel> IGuild.GetAFKChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IVoiceChannel>(AFKChannel);
        /// <inheritdoc />
        Task<ITextChannel> IGuild.GetDefaultChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<ITextChannel>(DefaultChannel);
        /// <inheritdoc />
        Task<IGuildChannel> IGuild.GetWidgetChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildChannel>(WidgetChannel);
        /// <inheritdoc />
        Task<ITextChannel> IGuild.GetSystemChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<ITextChannel>(SystemChannel);
        /// <inheritdoc />
        Task<ITextChannel> IGuild.GetRulesChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<ITextChannel>(RulesChannel);
        /// <inheritdoc />
        Task<ITextChannel> IGuild.GetPublicUpdatesChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<ITextChannel>(PublicUpdatesChannel);
        /// <inheritdoc />
        async Task<ITextChannel> IGuild.CreateTextChannelAsync(string name, Action<TextChannelProperties> func, RequestOptions options)
            => await CreateTextChannelAsync(name, func, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func, RequestOptions options)
            => await CreateVoiceChannelAsync(name, func, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IStageChannel> IGuild.CreateStageChannelAsync(string name, Action<VoiceChannelProperties> func, RequestOptions options)
            => await CreateStageChannelAsync(name, func, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<ICategoryChannel> IGuild.CreateCategoryAsync(string name, Action<GuildChannelProperties> func, RequestOptions options)
            => await CreateCategoryChannelAsync(name, func, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IVoiceRegion>> IGuild.GetVoiceRegionsAsync(RequestOptions options)
            => await GetVoiceRegionsAsync(options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IIntegration>> IGuild.GetIntegrationsAsync(RequestOptions options)
            => await GetIntegrationsAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task IGuild.DeleteIntegrationAsync(ulong id, RequestOptions options)
            => await DeleteIntegrationAsync(id, options).ConfigureAwait(false);

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
            => await CreateRoleAsync(name, permissions, color, isHoisted, false, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IRole> IGuild.CreateRoleAsync(string name, GuildPermissions? permissions, Color? color, bool isHoisted, bool isMentionable, RequestOptions options)
            => await CreateRoleAsync(name, permissions, color, isHoisted, isMentionable, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IGuildUser>> IGuild.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload && !HasAllMembers)
                return (await GetUsersAsync(options).FlattenAsync().ConfigureAwait(false)).ToImmutableArray();
            else
                return Users;
        }

        /// <inheritdoc />
        async Task<IGuildUser> IGuild.AddGuildUserAsync(ulong userId, string accessToken, Action<AddGuildUserProperties> func, RequestOptions options)
            => await AddGuildUserAsync(userId, accessToken, func, options);
        /// <inheritdoc />
        async Task<IGuildUser> IGuild.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            var user = GetUser(id);
            if (user is not null || mode == CacheMode.CacheOnly)
                return user;

            return await GuildHelper.GetUserAsync(this, Discord, id, options).ConfigureAwait(false);
        }

        /// <inheritdoc />
        Task<IGuildUser> IGuild.GetCurrentUserAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(CurrentUser);
        /// <inheritdoc />
        Task<IGuildUser> IGuild.GetOwnerAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(Owner);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IGuildUser>> IGuild.SearchUsersAsync(string query, int limit, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await SearchUsersAsync(query, limit, options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IGuildUser>();
        }

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IAuditLogEntry>> IGuild.GetAuditLogsAsync(int limit, CacheMode cacheMode, RequestOptions options,
            ulong? beforeId, ulong? userId, ActionType? actionType)
        {
            if (cacheMode == CacheMode.AllowDownload)
                return (await GetAuditLogsAsync(limit, options, beforeId: beforeId, userId: userId, actionType: actionType).FlattenAsync().ConfigureAwait(false)).ToImmutableArray();
            else
                return ImmutableArray.Create<IAuditLogEntry>();
        }

        /// <inheritdoc />
        async Task<IWebhook> IGuild.GetWebhookAsync(ulong id, RequestOptions options)
            => await GetWebhookAsync(id, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IWebhook>> IGuild.GetWebhooksAsync(RequestOptions options)
            => await GetWebhooksAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IApplicationCommand>> IGuild.GetApplicationCommandsAsync (RequestOptions options)
            => await GetApplicationCommandsAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<ICustomSticker> IGuild.CreateStickerAsync(string name, string description, IEnumerable<string> tags, Image image, RequestOptions options)
            => await CreateStickerAsync(name, description, tags, image, options);
        /// <inheritdoc />
        async Task<ICustomSticker> IGuild.CreateStickerAsync(string name, string description, IEnumerable<string> tags, Stream stream, string filename, RequestOptions options)
            => await CreateStickerAsync(name, description, tags, stream, filename, options);
        /// <inheritdoc />
        async Task<ICustomSticker> IGuild.CreateStickerAsync(string name, string description, IEnumerable<string> tags, string path, RequestOptions options)
            => await CreateStickerAsync(name, description, tags, path, options);
        /// <inheritdoc />
        async Task<ICustomSticker> IGuild.GetStickerAsync(ulong id, CacheMode mode, RequestOptions options)
            => await GetStickerAsync(id, mode, options);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<ICustomSticker>> IGuild.GetStickersAsync(CacheMode mode, RequestOptions options)
            => await GetStickersAsync(mode, options);
        /// <inheritdoc />
        Task IGuild.DeleteStickerAsync(ICustomSticker sticker, RequestOptions options)
            => DeleteStickerAsync(_stickers[sticker.Id], options);
        /// <inheritdoc />
        async Task<IApplicationCommand> IGuild.GetApplicationCommandAsync(ulong id, CacheMode mode, RequestOptions options)
            => await GetApplicationCommandAsync(id, mode, options);
        /// <inheritdoc />
        async Task<IApplicationCommand> IGuild.CreateApplicationCommandAsync(ApplicationCommandProperties properties, RequestOptions options)
            => await CreateApplicationCommandAsync(properties, options);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IApplicationCommand>> IGuild.BulkOverwriteApplicationCommandsAsync(ApplicationCommandProperties[] properties,
            RequestOptions options)
            => await BulkOverwriteApplicationCommandAsync(properties, options);

        void IDisposable.Dispose()
        {
            DisconnectAudioAsync().GetAwaiter().GetResult();
            _audioLock?.Dispose();
            _audioClient?.Dispose();
        }
        #endregion
    }
}
