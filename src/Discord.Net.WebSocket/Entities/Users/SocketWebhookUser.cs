using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based webhook user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketWebhookUser : SocketUser, IWebhookUser
    {
        /// <summary> Gets the guild of this webhook. </summary>
        public SocketGuild Guild { get; }
        /// <inheritdoc />
        public ulong WebhookId { get; }

        /// <inheritdoc />
        public override string Username { get; internal set; }
        /// <inheritdoc />
        public override ushort DiscriminatorValue { get; internal set; }
        /// <inheritdoc />
        public override string AvatarId { get; internal set; }
        /// <inheritdoc />
        public override bool IsBot { get; internal set; }

        /// <inheritdoc />
        public override bool IsWebhook => true;

        internal override SocketPresence Presence { get { return new SocketPresence(UserStatus.Offline, null); } set { } }
        internal override SocketGlobalUser GlobalUser => 
            throw new NotSupportedException();

        internal SocketWebhookUser(SocketGuild guild, ulong id, ulong webhookId)
            : base(guild.Discord, id)
        {
            Guild = guild;
            WebhookId = webhookId;
        }
        internal static SocketWebhookUser Create(SocketGuild guild, ClientState state, Model model, ulong webhookId)
        {
            var entity = new SocketWebhookUser(guild, model.Id, webhookId);
            entity.Update(state, model);
            return entity;
        }

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Webhook)";
        internal new SocketWebhookUser Clone() => MemberwiseClone() as SocketWebhookUser;


        //IGuildUser
        /// <inheritdoc />
        IGuild IGuildUser.Guild => Guild;
        /// <inheritdoc />
        ulong IGuildUser.GuildId => Guild.Id;
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => ImmutableArray.Create<ulong>();
        /// <inheritdoc />
        DateTimeOffset? IGuildUser.JoinedAt => null;
        /// <inheritdoc />
        string IGuildUser.Nickname => null;
        /// <inheritdoc />
        GuildPermissions IGuildUser.GuildPermissions => GuildPermissions.Webhook;

        /// <inheritdoc />
        ChannelPermissions IGuildUser.GetPermissions(IGuildChannel channel) => Permissions.ToChannelPerms(channel, GuildPermissions.Webhook.RawValue);
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Webhook users cannot be kicked.</exception>
        Task IGuildUser.KickAsync(string reason, RequestOptions options) => 
            throw new NotSupportedException("Webhook users cannot be kicked.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Webhook users cannot be modified.</exception>
        Task IGuildUser.ModifyAsync(Action<GuildUserProperties> func, RequestOptions options) => 
            throw new NotSupportedException("Webhook users cannot be modified.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.AddRoleAsync(IRole role, RequestOptions options) => 
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options) => 
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.RemoveRoleAsync(IRole role, RequestOptions options) => 
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options) => 
            throw new NotSupportedException("Roles are not supported on webhook users.");

        //IVoiceState
        /// <inheritdoc />
        bool IVoiceState.IsDeafened => false;
        /// <inheritdoc />
        bool IVoiceState.IsMuted => false;
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
