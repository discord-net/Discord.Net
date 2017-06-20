using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FullModel = Discord.API.AuditLog;
using Model = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RestAuditLogEntry : RestEntity<ulong>, IAuditLogEntry
    {
        internal RestAuditLogEntry(BaseDiscordClient discord, Model model, API.User user)
            : base(discord, model.Id)
        {
            Action = model.Action;
            if (model.Changes != null)
                Changes = model.Changes
                    .Select(x => AuditLogHelper.CreateChange(discord, model, x))
                    .ToReadOnlyCollection(() => model.Changes.Length);
            else
                Changes = ImmutableArray.Create<IAuditLogChange>();

            if (model.Options != null)
                Options = AuditLogHelper.CreateOptions(discord, model, model.Options);

            TargetId = model.TargetId;
            User = RestUser.Create(discord, user);

            Reason = model.Reason;
        }

        internal static RestAuditLogEntry Create(BaseDiscordClient discord, FullModel fullLog, Model model)
        {
            var user = fullLog.Users.FirstOrDefault(x => x.Id == model.UserId);

            return new RestAuditLogEntry(discord, model, user);
        }

        public ActionType Action { get; }
        public IReadOnlyCollection<IAuditLogChange> Changes { get; }
        public IAuditLogOptions Options { get; }
        public ulong TargetId { get; }
        public IUser User { get; }
        public string Reason { get; }
    }
}
