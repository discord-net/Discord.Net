using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildMember;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based guild user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestGuildUser : RestUser, IGuildUser
    {
        #region RestGuildUser
        private long? _premiumSinceTicks;
        private long? _timedOutTicks;
        private long? _joinedAtTicks;
        private ImmutableArray<ulong> _roleIds;
        /// <inheritdoc />
        public string DisplayName => Nickname ?? Username;
        /// <inheritdoc />
        public string Nickname { get; private set; }
        /// <inheritdoc/>
        public string DisplayAvatarId => GuildAvatarId ?? AvatarId;
        /// <inheritdoc/>
        public string GuildAvatarId { get; private set; }
        internal IGuild Guild { get; private set; }
        /// <inheritdoc />
        public bool IsDeafened { get; private set; }
        /// <inheritdoc />
        public bool IsMuted { get; private set; }
        /// <inheritdoc />
        public DateTimeOffset? PremiumSince => DateTimeUtils.FromTicks(_premiumSinceTicks);
        /// <inheritdoc />
        public ulong GuildId { get; }
        /// <inheritdoc />
        public bool? IsPending { get; private set; }
        /// <inheritdoc />
        public int Hierarchy
        {
            get
            {
                if (Guild.OwnerId == Id)
                    return int.MaxValue;

                var orderedRoles = Guild.Roles.OrderByDescending(x => x.Position);
                return orderedRoles.Where(x => RoleIds.Contains(x.Id)).Max(x => x.Position);
            }
        }

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

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException" accessor="get">Resolving permissions requires the parent guild to be downloaded.</exception>
        public GuildPermissions GuildPermissions
        {
            get
            {
                if (!Guild.Available)
                    throw new InvalidOperationException("Resolving permissions requires the parent guild to be downloaded.");
                return new GuildPermissions(Permissions.ResolveGuild(Guild, this));
            }
        }
        /// <inheritdoc />
        public IReadOnlyCollection<ulong> RoleIds => _roleIds;

        /// <inheritdoc />
        public DateTimeOffset? JoinedAt => DateTimeUtils.FromTicks(_joinedAtTicks);

        internal RestGuildUser(BaseDiscordClient discord, IGuild guild, ulong id, ulong? guildId = null)
            : base(discord, id)
        {
            if (guild is not null)
                Guild = guild;
            GuildId = guildId ?? Guild.Id;
        }
        internal static RestGuildUser Create(BaseDiscordClient discord, IGuild guild, Model model, ulong? guildId = null)
        {
            var entity = new RestGuildUser(discord, guild, model.User.Id, guildId);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            base.Update(model.User);
            if (model.JoinedAt.IsSpecified)
                _joinedAtTicks = model.JoinedAt.Value.UtcTicks;
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;
            if (model.Avatar.IsSpecified)
                GuildAvatarId = model.Avatar.Value;
            if (model.Deaf.IsSpecified)
                IsDeafened = model.Deaf.Value;
            if (model.Mute.IsSpecified)
                IsMuted = model.Mute.Value;
            if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles.Value);
            if (model.PremiumSince.IsSpecified)
                _premiumSinceTicks = model.PremiumSince.Value?.UtcTicks;
            if (model.TimedOutUntil.IsSpecified)
                _timedOutTicks = model.TimedOutUntil.Value?.UtcTicks;
            if (model.Pending.IsSpecified)
                IsPending = model.Pending.Value;
        }
        private void UpdateRoles(ulong[] roleIds)
        {
            var roles = ImmutableArray.CreateBuilder<ulong>(roleIds.Length + 1);
            roles.Add(GuildId);
            for (int i = 0; i < roleIds.Length; i++)
                roles.Add(roleIds[i]);
            _roleIds = roles.ToImmutable();
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetGuildMemberAsync(GuildId, Id, options).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public async Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null)
        {
            var args = await UserHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            if (args.Deaf.IsSpecified)
                IsDeafened = args.Deaf.Value;
            if (args.Mute.IsSpecified)
                IsMuted = args.Mute.Value;
            if (args.Nickname.IsSpecified)
                Nickname = args.Nickname.Value;
            if (args.Roles.IsSpecified)
                UpdateRoles(args.Roles.Value.Select(x => x.Id).ToArray());
            else if (args.RoleIds.IsSpecified)
                UpdateRoles(args.RoleIds.Value.ToArray());
        }
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
        /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
        public ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            var guildPerms = GuildPermissions;
            return new ChannelPermissions(Permissions.ResolveChannel(Guild, this, channel, guildPerms.RawValue));
        }

        /// <inheritdoc />
        public string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => GuildAvatarId is not null
                ? GetGuildAvatarUrl(format, size)
                : GetAvatarUrl(format, size);

        /// <inheritdoc />
        public string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetGuildUserAvatarUrl(Id, GuildId, GuildAvatarId, size, format);
#endregion

        #region IGuildUser
        /// <inheritdoc />
        IGuild IGuildUser.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }
        #endregion

        #region IVoiceState
        /// <inheritdoc />
        bool IVoiceState.IsSelfDeafened => false;
        /// <inheritdoc />
        bool IVoiceState.IsSelfMuted => false;
        /// <inheritdoc />
        bool IVoiceState.IsSuppressed => false;
        /// <inheritdoc />
        IVoiceChannel IVoiceState.VoiceChannel => null;
        /// <inheritdoc />
        string IVoiceState.VoiceSessionId => null;
        /// <inheritdoc />
        bool IVoiceState.IsStreaming => false;
        /// <inheritdoc />
        bool IVoiceState.IsVideoing => false;
        /// <inheritdoc />
        DateTimeOffset? IVoiceState.RequestToSpeakTimestamp => null;
        #endregion
    }
}
