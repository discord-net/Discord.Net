using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;
using OptionModel = Discord.API.AuditLogOptions;

namespace Discord.Rest
{
    internal static class AuditLogHelper
    {
        public static IAuditLogChange CreateChange(BaseDiscordClient discord, EntryModel entryModel, ChangeModel model)
        {
            switch (entryModel.Action)
            {
                case ActionType.MemberRoleUpdated:
                    return new MemberRoleAuditLogChange(discord, model);
                default:
                    throw new NotImplementedException($"{nameof(AuditLogHelper)} does not implement the {entryModel.Action} audit log action.");
            }
        }

        public static IAuditLogOptions CreateOptions(BaseDiscordClient discord, EntryModel entryModel, OptionModel model)
        {
            switch (entryModel.Action)
            {
                case ActionType.MessageDeleted:
                    return new MessageDeleteAuditLogOptions(discord, model);
                default:
                    throw new NotImplementedException($"{nameof(AuditLogHelper)} does not implement the {entryModel.Action} audit log action.");
            }
        }
    }
}
