using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class RestAuditLogEntry : RestEntity<ulong>, IAuditLogEntry
    {
        private RestAuditLogEntry(BaseDiscordClient discord, Model fullLog, EntryModel model, IUser user)
            : base(discord, model.Id)
        {
            Action = model.Action;

            if (model.Changes != null)
                Changes = AuditLogHelper.CreateChange(discord, fullLog, model, model.Changes);
            if (model.Options != null)
                Options = AuditLogHelper.CreateOptions(discord, fullLog, model, model.Options);

            TargetId = model.TargetId;
            User = user;

            Reason = model.Reason;
        }

        internal static RestAuditLogEntry Create(BaseDiscordClient discord, Model fullLog, EntryModel model)
        {
            var userInfo = fullLog.Users.FirstOrDefault(x => x.Id == model.UserId);
            IUser user = null;
            if (userInfo != null)
                user = RestUser.Create(discord, userInfo);

            return new RestAuditLogEntry(discord, fullLog, model, user);
        }

        public ActionType Action { get; }

        public IAuditLogChanges Changes { get; }
        public IAuditLogOptions Options { get; }

        public ulong? TargetId { get; } //TODO: if we're exposing this on the changes instead, do we need this?
        public IUser User { get; }

        public string Reason { get; }
    }
}
