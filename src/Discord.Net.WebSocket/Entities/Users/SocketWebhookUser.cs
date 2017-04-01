using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketWebhookUser : SocketUser, IWebhookUser
    {
        public SocketGuild Guild { get; }
        public ulong WebhookId { get; }

        public override string Username { get; internal set; }
        public override ushort DiscriminatorValue { get; internal set; }
        public override string AvatarId { get; internal set; }
        public override bool IsBot { get; internal set; }

        public override bool IsWebhook => true;

        internal override SocketPresence Presence { get { return new SocketPresence(UserStatus.Offline, null); } set { } }
        internal override SocketGlobalUser GlobalUser { get { throw new NotSupportedException(); } }

        internal SocketWebhookUser(SocketGuild guild, ulong id, ulong webhookId)
            : base(guild.Discord, id)
        {
            WebhookId = webhookId;
        }
        internal static SocketWebhookUser Create(SocketGuild guild, ClientState state, Model model, ulong webhookId)
        {
            var entity = new SocketWebhookUser(guild, model.Id, webhookId);
            entity.Update(state, model);
            return entity;
        }

        internal new SocketWebhookUser Clone() => MemberwiseClone() as SocketWebhookUser;


        //IGuildUser
        IGuild IGuildUser.Guild => Guild;
        ulong IGuildUser.GuildId => Guild.Id;
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => ImmutableArray.Create<ulong>();
        DateTimeOffset? IGuildUser.JoinedAt => null;
        string IGuildUser.Nickname => null;
        GuildPermissions IGuildUser.GuildPermissions => GuildPermissions.Webhook;

        ChannelPermissions IGuildUser.GetPermissions(IGuildChannel channel) => Permissions.ToChannelPerms(channel, GuildPermissions.Webhook.RawValue);
        Task IGuildUser.KickAsync(RequestOptions options)
        {
            throw new NotSupportedException("Webhook users cannot be kicked.");
        }
        Task IGuildUser.ModifyAsync(Action<GuildUserProperties> func, RequestOptions options)
        {
            throw new NotSupportedException("Webhook users cannot be modified.");
        }

        Task IGuildUser.AddRoleAsync(IRole role, RequestOptions options)
        {
            throw new NotSupportedException("Roles are not supported on webhook users.");
        }
        Task IGuildUser.AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options)
        {
            throw new NotSupportedException("Roles are not supported on webhook users.");
        }
        Task IGuildUser.RemoveRoleAsync(IRole role, RequestOptions options)
        {
            throw new NotSupportedException("Roles are not supported on webhook users.");
        }
        Task IGuildUser.RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options)
        {
            throw new NotSupportedException("Roles are not supported on webhook users.");
        }

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
