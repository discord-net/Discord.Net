using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class EmoteDeleteAuditLogData : IAuditLogData
    {
        private EmoteDeleteAuditLogData(Emote emote)
        {
            Emote = emote;
        }

        internal static EmoteDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name"); //TODO: only change?

            var emoteName = change.OldValue?.ToObject<string>();
            var emote = new Emote(entry.TargetId.Value, emoteName);

            return new EmoteDeleteAuditLogData(emote);
        }

        public Emote Emote { get; }
    }
}
