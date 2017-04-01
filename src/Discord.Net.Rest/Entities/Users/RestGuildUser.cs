using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildMember;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestGuildUser : RestUser, IGuildUser, IUpdateable
    {
        private long? _joinedAtTicks;
        private ImmutableArray<ulong> _roleIds;

        public string Nickname { get; private set; }
        internal IGuild Guild { get; private set; }
        public bool IsDeafened { get; private set; }
        public bool IsMuted { get; private set; }

        public ulong GuildId => Guild.Id;
        public GuildPermissions GuildPermissions
        {
            get
            {
                if (!Guild.Available)
                    throw new InvalidOperationException("Resolving permissions requires the parent guild to be downloaded.");
                return new GuildPermissions(Permissions.ResolveGuild(Guild, this));
            }
        }
        public IReadOnlyCollection<ulong> RoleIds => _roleIds;

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

        public override async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetGuildMemberAsync(GuildId, Id, options).ConfigureAwait(false);
            Update(model);
        }
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
        public Task KickAsync(RequestOptions options = null)
            => UserHelper.KickAsync(this, Discord, options);
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

        public ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            var guildPerms = GuildPermissions;
            return new ChannelPermissions(Permissions.ResolveChannel(Guild, this, channel, guildPerms.RawValue));
        }

        //IGuildUser
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
        bool IVoiceState.IsSelfDeafened => false;
        bool IVoiceState.IsSelfMuted => false;
        bool IVoiceState.IsSuppressed => false;
        IVoiceChannel IVoiceState.VoiceChannel => null;
        string IVoiceState.VoiceSessionId => null;
    }
}
