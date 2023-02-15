using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MemberModel = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;
using UserModel = Discord.API.User;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based guild user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGuildUser : SocketUser, IGuildUser
    {
        #region SocketGuildUser
        private long? _premiumSinceTicks;
        private long? _timedOutTicks;
        private long? _joinedAtTicks;
        private ImmutableArray<ulong> _roleIds;

        internal override SocketGlobalUser GlobalUser { get; set; }
        /// <summary>
        ///     Gets the guild the user is in.
        /// </summary>
        public SocketGuild Guild { get; }
        /// <inheritdoc />
        public string DisplayName => Nickname ?? Username;
        /// <inheritdoc />
        public string Nickname { get; private set; }
        /// <inheritdoc/>
        public string DisplayAvatarId => GuildAvatarId ?? AvatarId;
        /// <inheritdoc/>
        public string GuildAvatarId { get; private set; }
        /// <inheritdoc />
        public override bool IsBot { get { return GlobalUser.IsBot; } internal set { GlobalUser.IsBot = value; } }
        /// <inheritdoc />
        public override string Username { get { return GlobalUser.Username; } internal set { GlobalUser.Username = value; } }
        /// <inheritdoc />
        public override ushort DiscriminatorValue { get { return GlobalUser.DiscriminatorValue; } internal set { GlobalUser.DiscriminatorValue = value; } }
        /// <inheritdoc />
        public override string AvatarId { get { return GlobalUser.AvatarId; } internal set { GlobalUser.AvatarId = value; } }

        /// <inheritdoc />
        public GuildPermissions GuildPermissions => new GuildPermissions(Permissions.ResolveGuild(Guild, this));
        internal override SocketPresence Presence { get; set; }

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
        public GuildUserFlags Flags { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset? JoinedAt => DateTimeUtils.FromTicks(_joinedAtTicks);
        /// <summary>
        ///     Returns a collection of roles that the user possesses.
        /// </summary>
        public IReadOnlyCollection<SocketRole> Roles
            => _roleIds.Select(id => Guild.GetRole(id)).Where(x => x != null).ToReadOnlyCollection(() => _roleIds.Length);
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
        public SocketVoiceState? VoiceState => Guild.GetVoiceState(Id);
        public AudioInStream AudioStream => Guild.GetAudioStream(Id);
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
                if (Guild.OwnerId == Id)
                    return int.MaxValue;

                int maxPos = 0;
                for (int i = 0; i < _roleIds.Length; i++)
                {
                    var role = Guild.GetRole(_roleIds[i]);
                    if (role != null && role.Position > maxPos)
                        maxPos = role.Position;
                }
                return maxPos;
            }
        }

        internal SocketGuildUser(SocketGuild guild, SocketGlobalUser globalUser)
            : base(guild.Discord, globalUser.Id)
        {
            Guild = guild;
            GlobalUser = globalUser;
        }
        internal static SocketGuildUser Create(SocketGuild guild, ClientState state, UserModel model)
        {
            var entity = new SocketGuildUser(guild, guild.Discord.GetOrCreateUser(state, model));
            entity.Update(state, model);
            entity.UpdateRoles(new ulong[0]);
            return entity;
        }
        internal static SocketGuildUser Create(SocketGuild guild, ClientState state, MemberModel model)
        {
            var entity = new SocketGuildUser(guild, guild.Discord.GetOrCreateUser(state, model.User));
            entity.Update(state, model);
            if (!model.Roles.IsSpecified)
                entity.UpdateRoles(new ulong[0]);
            return entity;
        }
        internal static SocketGuildUser Create(SocketGuild guild, ClientState state, PresenceModel model)
        {
            var entity = new SocketGuildUser(guild, guild.Discord.GetOrCreateUser(state, model.User));
            entity.Update(state, model, false);
            if (!model.Roles.IsSpecified)
                entity.UpdateRoles(new ulong[0]);
            return entity;
        }
        internal void Update(ClientState state, MemberModel model)
        {
            base.Update(state, model.User);
            if (model.JoinedAt.IsSpecified)
                _joinedAtTicks = model.JoinedAt.Value.UtcTicks;
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;
            if (model.Avatar.IsSpecified)
                GuildAvatarId = model.Avatar.Value;
            if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles.Value);
            if (model.PremiumSince.IsSpecified)
                _premiumSinceTicks = model.PremiumSince.Value?.UtcTicks;
            if (model.TimedOutUntil.IsSpecified)
                _timedOutTicks = model.TimedOutUntil.Value?.UtcTicks;
            if (model.Pending.IsSpecified)
                IsPending = model.Pending.Value;

            Flags = model.Flags;
        }
        internal void Update(ClientState state, PresenceModel model, bool updatePresence)
        {
            if (updatePresence)
            {
                Update(model);
            }
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;
            if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles.Value);
            if (model.PremiumSince.IsSpecified)
                _premiumSinceTicks = model.PremiumSince.Value?.UtcTicks;
        }

        internal override void Update(PresenceModel model)
        {
            Presence ??= new SocketPresence();

            Presence.Update(model);
            GlobalUser.Update(model);
        }

        private void UpdateRoles(ulong[] roleIds)
        {
            var roles = ImmutableArray.CreateBuilder<ulong>(roleIds.Length + 1);
            roles.Add(Guild.Id);
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
            => new ChannelPermissions(Permissions.ResolveChannel(Guild, this, channel, GuildPermissions.RawValue));

        /// <inheritdoc />
        public string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => GuildAvatarId is not null
                ? GetGuildAvatarUrl(format, size)
                : GetAvatarUrl(format, size);

        /// <inheritdoc />
        public string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetGuildUserAvatarUrl(Id, Guild.Id, GuildAvatarId, size, format);

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Guild)";

        internal new SocketGuildUser Clone()
        {
            var clone = MemberwiseClone() as SocketGuildUser;
            clone.GlobalUser = GlobalUser.Clone();
            return clone;
        }
        #endregion

        #region IGuildUser
        /// <inheritdoc />
        IGuild IGuildUser.Guild => Guild;
        /// <inheritdoc />
        ulong IGuildUser.GuildId => Guild.Id;
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => _roleIds;

        //IVoiceState
        /// <inheritdoc />
        IVoiceChannel IVoiceState.VoiceChannel => VoiceChannel;
        #endregion
    }
}
