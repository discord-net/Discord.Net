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
        public ulong WebhookId { get; }
        internal IGuild Guild { get; }

        public override bool IsWebhook => true;
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
        IGuild IGuildUser.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }
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
