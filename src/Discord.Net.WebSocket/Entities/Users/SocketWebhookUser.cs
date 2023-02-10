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
        #region SocketWebhookUser
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
        /// <inheritdoc />
        internal override SocketPresence Presence { get { return new SocketPresence(UserStatus.Offline, null, null); } set { } }
        internal override SocketGlobalUser GlobalUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
        #endregion

        #region IGuildUser
        /// <inheritdoc />
        IGuild IGuildUser.Guild => Guild;
        /// <inheritdoc />
        ulong IGuildUser.GuildId => Guild.Id;
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => ImmutableArray.Create<ulong>();
        /// <inheritdoc />
        DateTimeOffset? IGuildUser.JoinedAt => null;
        /// <inheritdoc />
        string IGuildUser.DisplayName => null;
        /// <inheritdoc />
        string IGuildUser.Nickname => null;
        /// <inheritdoc />
        string IGuildUser.DisplayAvatarId => null;
        /// <inheritdoc />
        string IGuildUser.GuildAvatarId => null;
        /// <inheritdoc />
        string IGuildUser.GetDisplayAvatarUrl(ImageFormat format, ushort size) => null;
        /// <inheritdoc />
        string IGuildUser.GetGuildAvatarUrl(ImageFormat format, ushort size) => null;
        /// <inheritdoc />
        DateTimeOffset? IGuildUser.PremiumSince => null;
        /// <inheritdoc />
        DateTimeOffset? IGuildUser.TimedOutUntil => null;
        /// <inheritdoc />
        bool? IGuildUser.IsPending => null;
        /// <inheritdoc />
        int IGuildUser.Hierarchy => 0;
        /// <inheritdoc />
        GuildPermissions IGuildUser.GuildPermissions => GuildPermissions.Webhook;
        /// <inheritdoc />
        GuildUserFlags IGuildUser.Flags => GuildUserFlags.None;

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
        Task IGuildUser.AddRoleAsync(ulong roleId, RequestOptions options) =>
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.AddRoleAsync(IRole role, RequestOptions options) =>
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options) =>
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options) =>
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.RemoveRoleAsync(ulong roleId, RequestOptions options) =>
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.RemoveRoleAsync(IRole role, RequestOptions options) =>
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.RemoveRolesAsync(IEnumerable<ulong> roles, RequestOptions options) =>
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Roles are not supported on webhook users.</exception>
        Task IGuildUser.RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options) =>
            throw new NotSupportedException("Roles are not supported on webhook users.");
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Timeouts are not supported on webhook users.</exception>
        Task IGuildUser.SetTimeOutAsync(TimeSpan span, RequestOptions options) =>
            throw new NotSupportedException("Timeouts are not supported on webhook users.");
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Timeouts are not supported on webhook users.</exception>
        Task IGuildUser.RemoveTimeOutAsync(RequestOptions options) =>
            throw new NotSupportedException("Timeouts are not supported on webhook users.");
        #endregion

        #region IVoiceState
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
        /// <inheritdoc />
        bool IVoiceState.IsStreaming => false;
        /// <inheritdoc />
        bool IVoiceState.IsVideoing => false;
        /// <inheritdoc />
        DateTimeOffset? IVoiceState.RequestToSpeakTimestamp => null;
        #endregion
    }
}
