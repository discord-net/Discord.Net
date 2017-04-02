using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UserModel = Discord.API.User;
using MemberModel = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGuildUser : SocketUser, IGuildUser
    {
        private long? _joinedAtTicks;
        private ImmutableArray<ulong> _roleIds;

        internal override SocketGlobalUser GlobalUser { get; }
        public SocketGuild Guild { get; }
        public string Nickname { get; private set; }

        public override bool IsBot { get { return GlobalUser.IsBot; } internal set { GlobalUser.IsBot = value; } }
        public override string Username { get { return GlobalUser.Username; } internal set { GlobalUser.Username = value; } }
        public override ushort DiscriminatorValue { get { return GlobalUser.DiscriminatorValue; } internal set { GlobalUser.DiscriminatorValue = value; } }
        public override string AvatarId { get { return GlobalUser.AvatarId; } internal set { GlobalUser.AvatarId = value; } }
        public GuildPermissions GuildPermissions => new GuildPermissions(Permissions.ResolveGuild(Guild, this));
        internal override SocketPresence Presence { get; set; }

        public override bool IsWebhook => false;
        public bool IsSelfDeafened => VoiceState?.IsSelfDeafened ?? false;
        public bool IsSelfMuted => VoiceState?.IsSelfMuted ?? false;
        public bool IsSuppressed => VoiceState?.IsSuppressed ?? false;
        public bool IsDeafened => VoiceState?.IsDeafened ?? false;
        public bool IsMuted => VoiceState?.IsMuted ?? false;
        public DateTimeOffset? JoinedAt => DateTimeUtils.FromTicks(_joinedAtTicks);
        public IEnumerable<SocketRole> Roles 
            => _roleIds.Select(id => Guild.GetRole(id)).Where(x => x != null).ToReadOnlyCollection(() => _roleIds.Length);
        public SocketVoiceChannel VoiceChannel => VoiceState?.VoiceChannel;
        public string VoiceSessionId => VoiceState?.VoiceSessionId ?? "";
        public SocketVoiceState? VoiceState => Guild.GetVoiceState(Id);
        public AudioInStream AudioStream => Guild.GetAudioStream(Id);

        /// <summary> The position of the user within the role hirearchy. </summary>
        /// <remarks> The returned value equal to the position of the highest role the user has, 
        /// or int.MaxValue if user is the server owner. </remarks>
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
            return entity;
        }
        internal static SocketGuildUser Create(SocketGuild guild, ClientState state, PresenceModel model)
        {
            var entity = new SocketGuildUser(guild, guild.Discord.GetOrCreateUser(state, model.User));
            entity.Update(state, model, false);
            return entity;
        }
        internal void Update(ClientState state, MemberModel model)
        {
            base.Update(state, model.User);
            if (model.JoinedAt.IsSpecified)
                _joinedAtTicks = model.JoinedAt.Value.UtcTicks;
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;
            if (model.Roles.IsSpecified)
                UpdateRoles(model.Roles.Value);
        }
        internal override void Update(ClientState state, PresenceModel model)
            => Update(state, model, true);
        internal void Update(ClientState state, PresenceModel model, bool updatePresence)
        {
            if (updatePresence)
                base.Update(state, model);
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;
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
        
        public Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null)
            => UserHelper.ModifyAsync(this, Discord, func, options);
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
            => new ChannelPermissions(Permissions.ResolveChannel(Guild, this, channel, GuildPermissions.RawValue));

        internal new SocketGuildUser Clone() => MemberwiseClone() as SocketGuildUser;

        //IGuildUser
        IGuild IGuildUser.Guild => Guild;
        ulong IGuildUser.GuildId => Guild.Id;
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => _roleIds;

        //IUser
        Task<IDMChannel> IUser.GetDMChannelAsync(CacheMode mode, RequestOptions options) 
            => Task.FromResult<IDMChannel>(GlobalUser.DMChannel);

        //IVoiceState
        IVoiceChannel IVoiceState.VoiceChannel => VoiceChannel;
    }
}
