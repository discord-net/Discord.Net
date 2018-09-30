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
        private long? _joinedAtTicks;
        private ImmutableArray<ulong> _roleIds;

        /// <inheritdoc />
        public string Nickname { get; private set; }
        internal IGuild Guild { get; private set; }
        /// <inheritdoc />
        public bool IsDeafened { get; private set; }
        /// <inheritdoc />
        public bool IsMuted { get; private set; }

        /// <inheritdoc />
        public ulong GuildId => Guild.Id;

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

        internal RestGuildUser(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal static RestGuildUser Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestGuildUser(discord, guild, model.User.Id);
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
            if (model.Deaf.IsSpecified)
                IsDeafened = model.Deaf.Value;
            if (model.Mute.IsSpecified)
                IsMuted = model.Mute.Value;
            if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles.Value);
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
        public Task AddRoleAsync(IRole role, RequestOptions options = null)
            => AddRolesAsync(new[] { role }, options);
        /// <inheritdoc />
        public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
            => UserHelper.AddRolesAsync(this, Discord, roles, options);
        /// <inheritdoc />
        public Task RemoveRoleAsync(IRole role, RequestOptions options = null)
            => RemoveRolesAsync(new[] { role }, options);
        /// <inheritdoc />
        public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
            => UserHelper.RemoveRolesAsync(this, Discord, roles, options);

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
        public ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            var guildPerms = GuildPermissions;
            return new ChannelPermissions(Permissions.ResolveChannel(Guild, this, channel, guildPerms.RawValue));
        }

        //IGuildUser
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

        //IVoiceState
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
    }
}
