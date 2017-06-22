using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class EmoteCreateAuditLogData : IAuditLogData
    {
        private EmoteCreateAuditLogData(Emote emote)
        {
            Emote = emote;
        }

        internal static EmoteCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name"); //TODO: only change?

            var emoteName = change.NewValue?.ToObject<string>();
            var emote = new Emote(entry.TargetId.Value, emoteName);

            return new EmoteCreateAuditLogData(emote);
        }

        public Emote Emote { get; }
    }
}
