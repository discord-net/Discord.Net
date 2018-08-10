using System;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based audit log entry.
    /// </summary>
    public class RestAuditLogEntry : RestEntity<ulong>, IAuditLogEntry
    {
        private RestAuditLogEntry(BaseDiscordClient discord, Model fullLog, EntryModel model, IUser user)
            : base(discord, model.Id)
        {
            Action = model.Action;
            Data = AuditLogHelper.CreateData(discord, fullLog, model);
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

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc/>
        public ActionType Action { get; }
        /// <inheritdoc/>
        public IAuditLogData Data { get; }
        /// <inheritdoc/>
        public IUser User { get; }
        /// <inheritdoc/>
        public string Reason { get; }
    }
}
