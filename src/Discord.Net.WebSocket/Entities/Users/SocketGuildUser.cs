using Discord.API.Rest;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Model = Discord.API.GuildMember;
using PresenceModel = Discord.API.Presence;

namespace Discord.WebSocket
{
    internal class SocketGuildUser : SocketUser, IGuildUser
    {
        private long? _joinedAtTicks;
        private ImmutableArray<ulong> _roleIds;

        public string Nickname { get; private set; }
        public ulong GuildId { get; private set; }

        public IReadOnlyCollection<ulong> RoleIds => _roleIds;

        public DateTimeOffset? JoinedAt => DateTimeUtils.FromTicks(_joinedAtTicks);

        internal SocketGuildUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static SocketGuildUser Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketGuildUser(discord, model.User.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            _joinedAtTicks = model.JoinedAt.UtcTicks;
            if (model.Nick.IsSpecified)
                Nickname = model.Nick.Value;
            UpdateRoles(model.Roles);
        }
        private void UpdateRoles(ulong[] roleIds)
        {
            var roles = ImmutableArray.CreateBuilder<ulong>(roleIds.Length + 1);
            roles.Add(GuildId);
            for (int i = 0; i < roleIds.Length; i++)
                roles.Add(roleIds[i]);
            _roleIds = roles.ToImmutable();
        }

        public override async Task UpdateAsync()
            => Update(await UserHelper.GetAsync(this, Discord));
        public Task ModifyAsync(Action<ModifyGuildMemberParams> func)
            => UserHelper.ModifyAsync(this, Discord, func);
        public Task KickAsync()
            => UserHelper.KickAsync(this, Discord);

        public ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            throw new NotImplementedException(); //TODO: Impl
        }

        //IGuildUser
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => RoleIds;

        //IVoiceState
        bool IVoiceState.IsDeafened => false;
        bool IVoiceState.IsMuted => false;
        bool IVoiceState.IsSelfDeafened => false;
        bool IVoiceState.IsSelfMuted => false;
        bool IVoiceState.IsSuppressed => false;
        IVoiceChannel IVoiceState.VoiceChannel => null;
        string IVoiceState.VoiceSessionId => null;
    }
}
