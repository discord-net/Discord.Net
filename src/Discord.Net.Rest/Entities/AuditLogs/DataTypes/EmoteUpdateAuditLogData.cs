using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class EmoteUpdateAuditLogData : IAuditLogData
    {
        private EmoteUpdateAuditLogData(Emote emote, string oldName)
        {
            Emote = emote;
            PreviousName = oldName;
        }

        internal static EmoteUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name"); //TODO: only change?

            var emoteName = change.NewValue?.ToObject<string>();
            var emote = new Emote(entry.TargetId.Value, emoteName);

            var oldName = change.OldValue?.ToObject<string>();

            return new EmoteUpdateAuditLogData(emote, oldName);
        }

        public Emote Emote { get; }
        public string PreviousName { get; }
    }
}
