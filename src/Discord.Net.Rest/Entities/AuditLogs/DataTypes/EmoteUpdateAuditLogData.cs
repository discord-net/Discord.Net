using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class EmoteUpdateAuditLogData : IAuditLogData
    {
        private EmoteUpdateAuditLogData(ulong id, string oldName, string newName)
        {
            EmoteId = id;
            OldName = oldName;
            NewName = newName;
        }

        internal static EmoteUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var newName = change.NewValue?.ToObject<string>();
            var oldName = change.OldValue?.ToObject<string>();

            return new EmoteUpdateAuditLogData(entry.TargetId.Value, oldName, newName);
        }

        public ulong EmoteId { get; }
        public string NewName { get; }
        public string OldName { get; }
    }
}
