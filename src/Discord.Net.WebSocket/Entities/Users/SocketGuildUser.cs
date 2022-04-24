using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UserModel = Discord.IUserModel;
using MemberModel = Discord.IMemberModel;
using PresenceModel = Discord.IPresenceModel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based guild user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGuildUser : SocketUser, IGuildUser, ICached<MemberModel>, IDisposable
    {
        #region SocketGuildUser
        private long? _premiumSinceTicks;
        private long? _timedOutTicks;
        private long? _joinedAtTicks;
        private ImmutableArray<ulong> _roleIds;
        private ulong _guildId;

        /// <summary>
        ///     Gets the guild the user is in.
        /// </summary>
        public Lazy<SocketGuild> Guild { get; } // TODO: convert to LazyCached once guilds are cached.
        /// <summary>
        ///     Gets the ID of the guild that the user is in.
        /// </summary>
        public ulong GuildId => _guildId;
        /// <inheritdoc />
        public string DisplayName => Nickname ?? Username;
        /// <inheritdoc />
        public string Nickname { get; private set; }
        /// <inheritdoc/>
        public string DisplayAvatarId => GuildAvatarId ?? AvatarId;
        /// <inheritdoc/>
        public string GuildAvatarId { get; private set; }
        /// <inheritdoc />
        public override bool IsBot { get { return GlobalUser.Value.IsBot; } internal set { GlobalUser.Value.IsBot = value; } }
        /// <inheritdoc />
        public override string Username { get { return GlobalUser.Value.Username; } internal set { GlobalUser.Value.Username = value; } }
        /// <inheritdoc />
        public override ushort DiscriminatorValue { get { return GlobalUser.Value.DiscriminatorValue; } internal set { GlobalUser.Value.DiscriminatorValue = value; } }
        /// <inheritdoc />
        public override string AvatarId { get { return GlobalUser.Value.AvatarId; } internal set { GlobalUser.Value.AvatarId = value; } }

        /// <inheritdoc />
        public GuildPermissions GuildPermissions => new GuildPermissions(Permissions.ResolveGuild(Guild.Value, this));

        /// <inheritdoc />
        public override bool IsWebhook => false;
        /// <inheritdoc />
        public bool IsSelfDeafened => VoiceState?.IsSelfDeafened ?? false;
        /// <inheritdoc />
        public bool IsSelfMuted => VoiceState?.IsSelfMuted ?? false;
        /// <inheritdoc />
        public bool IsSuppressed => VoiceState?.IsSuppressed ?? false;
        /// <inheritdoc />
        public bool IsDeafened => VoiceState?.IsDeafened ?? false;
        /// <inheritdoc />
        public bool IsMuted => VoiceState?.IsMuted ?? false;
        /// <inheritdoc />
        public bool IsStreaming => VoiceState?.IsStreaming ?? false;
        /// <inheritdoc />
        public bool IsVideoing => VoiceState?.IsVideoing ?? false;
        /// <inheritdoc />
        public DateTimeOffset? RequestToSpeakTimestamp => VoiceState?.RequestToSpeakTimestamp ?? null;
        /// <inheritdoc />
        public bool? IsPending { get; private set; }


        /// <inheritdoc />
        public DateTimeOffset? JoinedAt => DateTimeUtils.FromTicks(_joinedAtTicks);
        /// <summary>
        ///     Returns a collection of roles that the user possesses.
        /// </summary>
        public IReadOnlyCollection<SocketRole> Roles
            => _roleIds.Select(id => Guild.Value.GetRole(id)).Where(x => x != null).ToReadOnlyCollection(() => _roleIds.Length);
        /// <summary>
        ///     Returns the voice channel the user is in, or <c>null</c> if none.
        /// </summary>
        public SocketVoiceChannel VoiceChannel => VoiceState?.VoiceChannel;
        /// <inheritdoc />
        public string VoiceSessionId => VoiceState?.VoiceSessionId ?? "";
        /// <summary>
        ///     Gets the voice connection status of the user if any.
        /// </summary>
        /// <returns>
        ///     A <see cref="SocketVoiceState" /> representing the user's voice status; <c>null</c> if the user is not
        ///     connected to a voice channel.
        /// </returns>
        public SocketVoiceState? VoiceState => Guild.Value.GetVoiceState(Id);
        public AudioInStream AudioStream => Guild.Value.GetAudioStream(Id);
        /// <inheritdoc />
        public DateTimeOffset? PremiumSince => DateTimeUtils.FromTicks(_premiumSinceTicks);
        /// <inheritdoc />
        public DateTimeOffset? TimedOutUntil
        {
            get
            {
                if (!_timedOutTicks.HasValue || _timedOutTicks.Value < 0)
                    return null;
                else
                    return DateTimeUtils.FromTicks(_timedOutTicks);
            }
        }

        /// <summary>
        ///     Returns the position of the user within the role hierarchy.
        /// </summary>
        /// <remarks>
        ///     The returned value equal to the position of the highest role the user has, or
        ///     <see cref="int.MaxValue"/> if user is the server owner.
        /// </remarks>
        public int Hierarchy
        {
            get
            {
                if (Guild.Value.OwnerId == Id)
                    return int.MaxValue;

                int maxPos = 0;
                for (int i = 0; i < _roleIds.Length; i++)
                {
                    var role = Guild.Value.GetRole(_roleIds[i]);
                    if (role != null && role.Position > maxPos)
                        maxPos = role.Position;
                }
                return maxPos;
            }
        }

        internal SocketGuildUser(ulong guildId, ulong userId, DiscordSocketClient client)
            : base(client, userId)
        {
            _guildId = guildId;
            Guild = new Lazy<SocketGuild>(() => client.StateManager.GetGuild(_guildId), System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }
        internal static SocketGuildUser Create(ulong guildId, DiscordSocketClient client, UserModel model)
        {
            var entity = new SocketGuildUser(guildId, model.Id, client);
            if (entity.Update(model))
                client.StateManager.GetMemberStore(guildId)?.AddOrUpdate(entity.ToModel());
            entity.UpdateRoles(Array.Empty<ulong>());
            return entity;
        }
        internal static SocketGuildUser Create(ulong guildId, DiscordSocketClient client, MemberModel model)
        {
            var entity = new SocketGuildUser(guildId, model.Id, client);
            entity.Update(model);
            client.StateManager.GetMemberStore(guildId)?.AddOrUpdate(model);
            return entity;
        }
        internal void Update(MemberModel model)
        {
            _joinedAtTicks = model.JoinedAt.UtcTicks;
            Nickname = model.Nickname;
            GuildAvatarId = model.GuildAvatar;
            UpdateRoles(model.Roles);
            if (model.PremiumSince.HasValue)
                _premiumSinceTicks = model.PremiumSince.Value.UtcTicks;
            if (model.CommunicationsDisabledUntil.HasValue)
                _timedOutTicks = model.CommunicationsDisabledUntil.Value.UtcTicks;
            IsPending = model.IsPending.GetValueOrDefault(false);
        }
        private void UpdateRoles(ulong[] roleIds)
        {
            var roles = ImmutableArray.CreateBuilder<ulong>(roleIds.Length + 1);
            roles.Add(_guildId);
            for (int i = 0; i < roleIds.Length; i++)
                roles.Add(roleIds[i]);
            _roleIds = roles.ToImmutable();
        }

        /// <inheritdoc />
        public Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null)
            => UserHelper.ModifyAsync(this, Discord, func, options);
        /// <inheritdoc />
        public Task KickAsync(string reason = null, RequestOptions options = null)
            => UserHelper.KickAsync(this, Discord, reason, options);
        /// <inheritdoc />
        public Task AddRoleAsync(ulong roleId, RequestOptions options = null)
            => AddRolesAsync(new[] { roleId }, options);
        /// <inheritdoc />
        public Task AddRoleAsync(IRole role, RequestOptions options = null)
            => AddRoleAsync(role.Id, options);
        /// <inheritdoc />
        public Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null)
            => UserHelper.AddRolesAsync(this, Discord, roleIds, options);
        /// <inheritdoc />
        public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
            => AddRolesAsync(roles.Select(x => x.Id), options);
        /// <inheritdoc />
        public Task RemoveRoleAsync(ulong roleId, RequestOptions options = null)
            => RemoveRolesAsync(new[] { roleId }, options);
        /// <inheritdoc />
        public Task RemoveRoleAsync(IRole role, RequestOptions options = null)
            => RemoveRoleAsync(role.Id, options);
        /// <inheritdoc />
        public Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null)
            => UserHelper.RemoveRolesAsync(this, Discord, roleIds, options);
        /// <inheritdoc />
        public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
            => RemoveRolesAsync(roles.Select(x => x.Id));
        /// <inheritdoc />
        public Task SetTimeOutAsync(TimeSpan span, RequestOptions options = null)
            => UserHelper.SetTimeoutAsync(this, Discord, span, options);
        /// <inheritdoc />
        public Task RemoveTimeOutAsync(RequestOptions options = null)
            => UserHelper.RemoveTimeOutAsync(this, Discord, options);
        /// <inheritdoc />
        public ChannelPermissions GetPermissions(IGuildChannel channel)
            => new ChannelPermissions(Permissions.ResolveChannel(Guild.Value, this, channel, GuildPermissions.RawValue));

        /// <inheritdoc />
        public string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => GuildAvatarId is not null
                ? GetGuildAvatarUrl(format, size)
                : GetAvatarUrl(format, size);

        /// <inheritdoc />
        public string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetGuildUserAvatarUrl(Id, _guildId, GuildAvatarId, size, format);

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Guild)";

        internal new SocketGuildUser Clone() => MemberwiseClone() as SocketGuildUser;

        #endregion

        #region IGuildUser
        /// <inheritdoc />
        IGuild IGuildUser.Guild => Guild.Value;
        /// <inheritdoc />
        ulong IGuildUser.GuildId => _guildId;
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => _roleIds;

        //IVoiceState
        /// <inheritdoc />
        IVoiceChannel IVoiceState.VoiceChannel => VoiceChannel;
        #endregion

        #region Cache

        private struct CacheModel : MemberModel
        {
            public ulong Id { get; set; }
            public string Nickname { get; set; }

            public string GuildAvatar { get; set; }

            public ulong[] Roles { get; set; }

            public DateTimeOffset JoinedAt { get; set; }

            public DateTimeOffset? PremiumSince { get; set; }

            public bool IsDeaf { get; set; }

            public bool IsMute { get; set; }

            public bool? IsPending { get; set; }

            public DateTimeOffset? CommunicationsDisabledUntil { get; set; }
        }
        internal new MemberModel ToModel()
            => ToModel<CacheModel>();

        internal new TModel ToModel<TModel>() where TModel : MemberModel, new()
        {
            return new TModel
            {
                Id = Id,
                CommunicationsDisabledUntil = TimedOutUntil,
                GuildAvatar = GuildAvatarId,
                IsDeaf = IsDeafened,
                IsMute = IsMuted,
                IsPending = IsPending,
                JoinedAt = JoinedAt ?? DateTimeOffset.UtcNow, // review: nullable joined at here? should our model reflect this?
                Nickname = Nickname,
                PremiumSince = PremiumSince,
                Roles = _roleIds.ToArray()
            };
        }

        MemberModel ICached<MemberModel>.ToModel()
            => ToModel();

        TResult ICached<MemberModel>.ToModel<TResult>()
            => ToModel<TResult>();

        void ICached<MemberModel>.Update(MemberModel model) => Update(model);

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            Discord.StateManager.GetMemberStore(_guildId)?.RemoveReference(Id);
        }
        ~SocketGuildUser() => Dispose();

        #endregion
    }
}
