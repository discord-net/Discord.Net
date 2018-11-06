using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestWebhookUser : RestUser, IWebhookUser
    {
        /// <inheritdoc />
        public ulong WebhookId { get; }
        internal IGuild Guild { get; }

        /// <inheritdoc />
        public override bool IsWebhook => true;
        /// <inheritdoc />
        public ulong GuildId => Guild.Id;

        internal RestWebhookUser(BaseDiscordClient discord, IGuild guild, ulong id, ulong webhookId)
            : base(discord, id)
        {
            Guild = guild;
            WebhookId = webhookId;
        }
        internal static RestWebhookUser Create(BaseDiscordClient discord, IGuild guild, Model model, ulong webhookId)
        {
            var entity = new RestWebhookUser(discord, guild, model.Id, webhookId);
            entity.Update(model);
            return entity;
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
        Task IGuildUser.KickAsync(string reason, RequestOptions options) => 
            throw new NotSupportedException("Webhook users cannot be kicked.");

        /// <inheritdoc />
        Task IGuildUser.ModifyAsync(Action<GuildUserProperties> func, RequestOptions options) => 
            throw new NotSupportedException("Webhook users cannot be modified.");

        /// <inheritdoc />
        Task IGuildUser.AddRoleAsync(IRole role, RequestOptions options) => 
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        Task IGuildUser.AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options) => 
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
        Task IGuildUser.RemoveRoleAsync(IRole role, RequestOptions options) => 
            throw new NotSupportedException("Roles are not supported on webhook users.");

        /// <inheritdoc />
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
