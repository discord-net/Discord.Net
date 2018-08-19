using System.Linq;
using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class EmoteCreateAuditLogData : IAuditLogData
    {
        private EmoteCreateAuditLogData(ulong id, string name)
        {
            EmoteId = id;
            Name = name;
        }

        public ulong EmoteId { get; }
        public string Name { get; }

        internal static EmoteCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var emoteName = change.NewValue?.ToObject<string>();
            return new EmoteCreateAuditLogData(entry.TargetId.Value, emoteName);
        }
    }
}
