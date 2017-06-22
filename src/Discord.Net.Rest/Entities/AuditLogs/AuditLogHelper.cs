using System;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    internal static class AuditLogHelper
    {
        public static IAuditLogData CreateData(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            switch (entry.Action)
            {
                case ActionType.GuildUpdated: //1
                    return GuildUpdateAuditLogData.Create(discord, log, entry);

                case ActionType.ChannelCreated: //10
                    return ChannelCreateAuditLogData.Create(discord, log, entry);
                case ActionType.ChannelUpdated:
                    return ChannelUpdateAuditLogData.Create(discord, log, entry);
                case ActionType.ChannelDeleted:
                    return ChannelDeleteAuditLogData.Create(discord, log, entry);
                case ActionType.OverwriteCreated:
                    return OverwriteCreateAuditLogData.Create(discord, log, entry);
                case ActionType.OverwriteUpdated:
                    return OverwriteUpdateAuditLogData.Create(discord, log, entry);
                case ActionType.OverwriteDeleted:
                    return OverwriteDeleteAuditLogData.Create(discord, log, entry);

                case ActionType.Kick: //20
                    return KickAuditLogData.Create(discord, log, entry);
                case ActionType.Prune:
                    return PruneAuditLogData.Create(discord, log, entry);
                case ActionType.Ban:
                    return BanAuditLogData.Create(discord, log, entry);
                case ActionType.Unban:
                    break;
                case ActionType.MemberUpdated:
                    return MemberUpdateAuditLogData.Create(discord, log, entry);
                case ActionType.MemberRoleUpdated:
                    return MemberRoleAuditLogData.Create(discord, log, entry);

                case ActionType.RoleCreated: //30
                    return RoleCreateAuditLogData.Create(discord, log, entry);
                case ActionType.RoleUpdated:
                    return RoleUpdateAuditLogData.Create(discord, log, entry);
                case ActionType.RoleDeleted:
                    return RoleDeleteAuditLogData.Create(discord, log, entry);

                case ActionType.InviteCreated: //40
                    return InviteCreateAuditLogData.Create(discord, log, entry);
                case ActionType.InviteUpdated:
                    break;
                case ActionType.InviteDeleted:
                    break;

                case ActionType.WebhookCreated: //50
                    break;
                case ActionType.WebhookUpdated:
                    break;
                case ActionType.WebhookDeleted:
                    break;

                case ActionType.EmojiCreated: //60
                    return EmoteCreateAuditLogData.Create(discord, log, entry);
                case ActionType.EmojiUpdated:
                    return EmoteUpdateAuditLogData.Create(discord, log, entry);
                case ActionType.EmojiDeleted:
                    return EmoteDeleteAuditLogData.Create(discord, log, entry);

                case ActionType.MessageDeleted: //72
                    return MessageDeleteAuditLogData.Create(discord, log, entry);
                default:
                    return null;
            }
            return null;
            //throw new NotImplementedException($"{nameof(AuditLogHelper)} does not implement the {entry.Action} audit log event.");
        }
    }
}
