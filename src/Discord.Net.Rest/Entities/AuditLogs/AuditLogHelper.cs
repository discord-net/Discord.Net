using System;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;
using OptionModel = Discord.API.AuditLogOptions;

namespace Discord.Rest
{
    internal static class AuditLogHelper
    {
        public static IAuditLogChanges CreateChange(BaseDiscordClient discord, Model log, EntryModel entry, ChangeModel[] models)
        {
            switch (entry.Action)
            {
                case ActionType.GuildUpdated:
                    break;
                case ActionType.ChannelCreated:
                    break;
                case ActionType.ChannelUpdated:
                    break;
                case ActionType.ChannelDeleted:
                    return ChannelDeleteChanges.Create(discord, models);
                case ActionType.OverwriteCreated:
                    break;
                case ActionType.OverwriteUpdated:
                    break;
                case ActionType.OverwriteDeleted:
                    return OverwriteDeleteChanges.Create(discord, log, entry, models);
                case ActionType.Prune:
                    break;
                case ActionType.Ban:
                    break;
                case ActionType.Unban:
                    break;
                case ActionType.MemberUpdated:
                    return MemberUpdateChanges.Create(discord, log, entry, models.FirstOrDefault());
                case ActionType.MemberRoleUpdated:
                    return MemberRoleChanges.Create(discord, log, entry, models);
                case ActionType.RoleCreated:
                    break;
                case ActionType.RoleUpdated:
                    break;
                case ActionType.RoleDeleted:
                    break;
                case ActionType.InviteCreated:
                    return InviteCreateChanges.Create(discord, log, models);
                case ActionType.InviteUpdated:
                    break;
                case ActionType.InviteDeleted:
                    break;
                case ActionType.WebhookCreated:
                    break;
                case ActionType.WebhookUpdated:
                    break;
                case ActionType.WebhookDeleted:
                    break;
                case ActionType.EmojiCreated:
                    return EmoteCreateChanges.Create(discord, entry, models.FirstOrDefault());
                case ActionType.EmojiUpdated:
                    return EmoteUpdateChanges.Create(discord, entry, models.FirstOrDefault());
                case ActionType.EmojiDeleted:
                    return EmoteDeleteChanges.Create(discord, entry, models.FirstOrDefault());
                case ActionType.MessageDeleted:
                case ActionType.Kick:
                default:
                    return null;
            }
            return null;
            //throw new NotImplementedException($"{nameof(AuditLogHelper)} does not implement the {entry.Action} audit log changeset.");
        }

        public static IAuditLogOptions CreateOptions(BaseDiscordClient discord, Model fullModel, EntryModel entryModel, OptionModel model)
        {
            switch (entryModel.Action)
            {
                case ActionType.MessageDeleted:
                    return new MessageDeleteAuditLogOptions(discord, model);
                default:
                    return null;
                    //throw new NotImplementedException($"{nameof(AuditLogHelper)} does not implement the {entryModel.Action} audit log action.");
            }
        }
    }
}
