using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a adding a bot to a guild.
    /// </summary>
    public class BotAddAuditLogData : IAuditLogData
    {
        private BotAddAuditLogData(ulong botId)
        {
            BotId = botId;
        }

        internal static BotAddAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            return new BotAddAuditLogData(entry.TargetId.Value);
        }

        /// <summary>
        ///     Gets the ID of the bot that was added.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the bot that was added.
        /// </returns>
        public ulong BotId { get; }
    }
}
