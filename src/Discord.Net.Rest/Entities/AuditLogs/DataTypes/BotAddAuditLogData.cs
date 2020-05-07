using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a adding a bot to a guild.
    /// </summary>
    public class BotAddAuditLogData : IAuditLogData
    {
        private BotAddAuditLogData(IUser bot)
        {
            Target = bot;
        }

        internal static BotAddAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var userInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            return new BotAddAuditLogData(RestUser.Create(discord, userInfo));
        }

        /// <summary>
        ///     Gets the bot that was added.
        /// </summary>
        /// <returns>
        ///     A user object representing the bot.
        /// </returns>
        public IUser Target { get; }
    }
}
