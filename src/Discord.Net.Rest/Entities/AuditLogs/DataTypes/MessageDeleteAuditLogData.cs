using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class MessageDeleteAuditLogData : IAuditLogData
    {
        private MessageDeleteAuditLogData(ulong channelId, int count)
        {
            ChannelId = channelId;
            MessageCount = count;
        }

        internal static MessageDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            return new MessageDeleteAuditLogData(entry.Options.MessageDeleteChannelId.Value, entry.Options.MessageDeleteCount.Value);
        }

        public int MessageCount { get; }
        public ulong ChannelId { get; }
    }
}
