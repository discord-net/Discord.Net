using Discord.Rest;
using System;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a REST-based audit log entry.
    /// </summary>
    public class SocketAuditLogEntry : SocketEntity<ulong>, IAuditLogEntry
    {
        private SocketAuditLogEntry(DiscordSocketClient discord, EntryModel model)
            : base(discord, model.Id)
        {
            Action = model.Action;
            Data = AuditLogHelper.CreateData(discord, model);
            Reason = model.Reason;
        }

        internal static SocketAuditLogEntry Create(DiscordSocketClient discord, EntryModel model)
        {
            // var userInfo = model.UserId != null ? fullLog.Users.FirstOrDefault(x => x.Id == model.UserId) : null;
            // IUser user = null;
            // if (userInfo != null)
            //     user = RestUser.Create(discord, userInfo);

            return new SocketAuditLogEntry(discord, model);
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
